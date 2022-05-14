namespace Loupedeck.CommandPostPlugin
{
    using System;    
    using System.Linq;
    using System.Text.Json;
    using System.Collections.Generic;

    using Loupedeck.CommandPostPlugin.Models.Events;

    using Loupedeck.CommandPostPlugin.Localisation;

    using Fleck;
    using System.Globalization;
    
    //
    // WebSocket Message Class:
    //
    public class WebSocketMessage
    {
        public String MessageType { get; set; }
        public String ActionName { get; set; }
        public String ActionValue { get; set; }

        public String CommandName { get; set; }
        public String CommandType { get; set; }
        public String CommandCategory { get; set; }
        public String CommandTheme { get; set; }
    }

    public class CommandPostPlugin : Plugin
    {
        //
        // Private:
        //
        private CPLocalisation localisation;
        private readonly String WebSocketServerPort = "54475";

        //
        // Public:
        //
        public static String LoupedeckLanguageCode;

        //
        // Event Handlers:
        //
        public event EventHandler<ActionValueUpdatedEventArgs> ActionValueUpdatedEvents;
        public event EventHandler<NewCommandsEventArgs> NewCommandsEvents;

        //
        // Does not require an application to be in the foreground or any application to be running locally at all:
        //
        public override Boolean HasNoApplication => true;

        //
        // API-based actions, those that are controlling target application/service using dedicated API:
        //
        public override Boolean UsesApplicationApiOnly => true;

        //
        // Get the LoupedeckConfig Language:
        //
        public String GetLoupedeckLanguage()
        {
            var LoupedeckLanguage = this.Localization.LoupedeckLanguage;
            return LoupedeckLanguage;
        }

        //
        // Get the LoupedeckConfig Language Code:
        //
        public String GetLoupedeckLanguageCode()
        {
            var LoupedeckLanguage = this.GetLoupedeckLanguage();
            var cultureInfo = new CultureInfo(LoupedeckLanguage, false);
            var result = cultureInfo.TwoLetterISOLanguageName;
            return result;
        }

        //
        // On Plugin Load:
        //
        public override void Load()
        {
            //
            // Load the localisation class:
            //
            this.localisation = new CPLocalisation(this);

            //
            // Work out what are the supported languages:
            //
            Console.WriteLine("[CP] Supported Languages: " + this.Localization.SupportedLanguages);

            //
            // Detect Language Changes:
            //
            this.Localization.LanguageChanged += (sender, e) =>
            {
                Console.WriteLine("[CP] LANGUAGE CHANGED! " + this.GetLoupedeckLanguageCode());
            };

            this.PluginActionsChangedRequest += (sender, e) =>
            {
                Console.WriteLine("[CP] PluginActionsChangedRequest");
            };

            //
            // Load CommandPost Icon from the Embedded Resources:
            //
            this.LoadIcons();

            //
            // Start WebSocket Server:
            //
            this.SetupSocketServer();

            //
            // Update the Plugin Status:
            //
            this.UpdatePluginStatus();
        }

        //
        // Update the CommandPost Plugin Status:
        //
        public void UpdatePluginStatus()
        {            
            var CommandPostBundleID = "org.latenitefilms.CommandPost";
            var CommandPostPID = this.NativeMethods.GetProcessId(CommandPostBundleID);
            if (CommandPostPID == -1)
            {
                //
                // CommmandPost is not running:
                //
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error,
                this.localisation.GetGeneralString("CommandPostNotRunningError"),
                this.localisation.GetGeneralString("CommandPostURL"));
            } else if (allSockets.Count == 0) {
                //
                // CommandPost is not connected:
                //
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error,
                this.localisation.GetGeneralString("CommandPostWebSocketError"),
                this.localisation.GetGeneralString("CommandPostURL"));
            }
            else {
                //
                // Everything is Normal:
                //
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Normal, null, null);
            }
        }

        ///
        /// Load Icons:
        ///         
        public void LoadIcons()
        {
            this.Info.Icon16x16 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-16.png"));
            this.Info.Icon32x32 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-32.png"));
            this.Info.Icon48x48 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-48.png"));
            this.Info.Icon256x256 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-256.png"));
        }

        /// 
        /// WebSocket Server:
        /// 
        public static WebSocketServer commandpostWebSocketServer;
        public static List<IWebSocketConnection> allSockets;
        public void SetupSocketServer()
        {
            allSockets = new List<IWebSocketConnection>();

            commandpostWebSocketServer = new WebSocketServer("ws://0.0.0.0:" + this.WebSocketServerPort);
            commandpostWebSocketServer.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    //
                    // WebSocket Open:
                    //
                    allSockets.Add(socket);
                    this.UpdatePluginStatus();
                };
                socket.OnClose = () =>
                {
                    //
                    // WebSocket Closed:
                    //
                    allSockets.Remove(socket);
                    this.UpdatePluginStatus();
                };
                socket.OnError = ex =>
                {
                    //
                    // An error has occurred:
                    //
                    this.OnPluginStatusChanged(Loupedeck.PluginStatus.Warning,
                        this.localisation.GetGeneralString("CommandPostWebSocketSpecificError") + ex.ToString(),
                        this.localisation.GetGeneralString("CommandPostURL"));
                };
                socket.OnMessage = message =>
                {                    
                    //
                    // WebSocket Message Recieved:
                    //
                    WebSocketMessage incomingMessage = JsonSerializer.Deserialize<WebSocketMessage>(message);
                    switch (incomingMessage.MessageType)
                    {
                        case "UpdateDisplay":
                            //
                            // Send Event Notification to Adjustments Class:
                            //
                            var actionValueUpdatedEventArgs = new ActionValueUpdatedEventArgs(incomingMessage.ActionName, incomingMessage.ActionValue);
                            ActionValueUpdatedEvents?.Invoke(this, actionValueUpdatedEventArgs);
                            break;
                        case "UpdateCommands":
                            //
                            // Send Event Notification to classes that has generated commands:
                            //
                            var newCommandsValueEventArgs = new NewCommandsValueEventArgs(incomingMessage.ActionName, incomingMessage.CommandName, incomingMessage.CommandType, incomingMessage.CommandCategory, incomingMessage.CommandTheme);
                            NewCommandsEvents?.Invoke(this, newCommandsValueEventArgs);
                            break;
                        case "Ping":                    
                            //
                            // Reply to Ping:
                            //
                            var jsonString = JsonSerializer.Serialize(new
                            {
                                actionName = "pong",                            
                            });                        
                            allSockets.ToList().ForEach(s => s.Send(jsonString));
                            break;
                        default:
                            //
                            // Unknown MessageType:
                            //
                            Console.WriteLine("[CP] Unknown WebSocket MessageType Received: " + incomingMessage.MessageType);
                            break;
                    };
                };
            });
        }

        public override void Unload()
        {
        }   

        private void OnApplicationStarted(Object sender, EventArgs e)
        {
        }

        private void OnApplicationStopped(Object sender, EventArgs e)
        {
        }

        public override void RunCommand(String commandName, String parameter)
        {
        }

        public override void ApplyAdjustment(String adjustmentName, String parameter, Int32 diff)
        {
        }
    }   
}


