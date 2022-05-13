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

    public class WebSocketMessage
    {
        public String MessageType { get; set; }
        public String ActionName { get; set; }
        public String ActionValue { get; set; }
    }

    public class CommandPostPlugin : Plugin
    {
        private CPLocalisation localisation;

        public static String LoupedeckLanguageCode;

        public event EventHandler<ActionValueUpdatedEventArgs> ActionValueUpdatedEvents;

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

            commandpostWebSocketServer = new WebSocketServer("ws://0.0.0.0:54475");
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
                    if (incomingMessage.MessageType == "UpdateDisplay")
                    {
                        //
                        // Send Event Notification to Adjustments Class:
                        //
                        var actionValueUpdatedEventArgs = new ActionValueUpdatedEventArgs(incomingMessage.ActionName, incomingMessage.ActionValue);
                        ActionValueUpdatedEvents?.Invoke(this, actionValueUpdatedEventArgs);
                    } else if (incomingMessage.MessageType == "Ping") {
                        //
                        // Reply to Ping:
                        //
                        var jsonString = JsonSerializer.Serialize(new
                        {
                            actionName = "pong",                            
                        });                        
                        allSockets.ToList().ForEach(s => s.Send(jsonString));
                    }
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


