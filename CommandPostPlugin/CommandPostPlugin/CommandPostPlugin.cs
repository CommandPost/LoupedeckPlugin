namespace Loupedeck.CommandPostPlugin
{
    using Fleck;
    using Loupedeck.CommandPostPlugin.Localisation;
    using Loupedeck.CommandPostPlugin.Models.Events;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.Json;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a WebSocket Message.
    /// </summary>
    public class WebSocketMessage
    {
        public String MessageType { get; set; }
        public String ActionName { get; set; }
        public String ActionValue { get; set; }
    }

    /// <summary>
    /// The main Plugin class.
    /// </summary>
    public class CommandPostPlugin : Plugin
    {
        /// <summary>
        /// The localisation class.
        /// </summary>
        public CPLocalisation localisation;

        /// <summary>
        /// The WebSocket Server Port as a string.
        /// </summary>
        private readonly String WebSocketServerPort = "54475";

        // Event Handlers:
        public event EventHandler<ActionValueUpdatedEventArgs> ActionValueUpdatedEvents;
        public event EventHandler<NewCommandsValueEventArgs> NewCommandsEvents;

        /// <summary>
        /// Does not require an application to be in the foreground or any application to be running locally at all.
        /// </summary>
        public override Boolean HasNoApplication => true;

        /// <summary>
        /// API-based actions, those that are controlling target application/service using dedicated API:
        /// </summary>
        public override Boolean UsesApplicationApiOnly => true;

        /// <summary>
        /// Gets the language used by the LoupedeckConfig application.
        /// </summary>
        /// <returns>The language as a string.</returns>
        public String GetLoupedeckLanguage()
        {
            var LoupedeckLanguage = this.Localization.LoupedeckLanguage;
            return LoupedeckLanguage;
        }

        /// <summary>
        /// Gets the Loupedeck Language code used by the LoupedeckConfig application.
        /// </summary>
        /// <returns>The Loupedeck Language code as a string.</returns>
        public String GetLoupedeckLanguageCode()
        {
            var LoupedeckLanguage = this.GetLoupedeckLanguage();
            var cultureInfo = new CultureInfo(LoupedeckLanguage, false);
            var result = cultureInfo.TwoLetterISOLanguageName;
            return result;
        }

        /// <summary>
        /// Triggered when the Plugin loads.
        /// </summary>
        public override void Load()
        {            
            // Set the Language to the Language of the LoupedeckConfig app:         
            this.Localization.SetCurrentLanguage(this.Localization.LoupedeckLanguage);

            // Write a list of support languages for debugging purposes:
            var supportedLanguages = this.Localization.SupportedLanguages;
            foreach (var supportedLanguage in supportedLanguages)
            {
                Console.WriteLine("[CP] Supported Language: " + supportedLanguage);
            }

            // Write the current language for debugging purposes:
            Console.WriteLine("[CP] Current Language: " + this.Localization.CurrentLanguage);

            // Load the localisation class:
            this.localisation = new CPLocalisation(this);

            // Load CommandPost Icon from the Embedded Resources:
            this.LoadIcons();

            // Start WebSocket Server:
            this.SetupWebSocketServer();

            // Update the Plugin Status:
            this.UpdatePluginStatus();

            // Check every 10 seconds for language changes:
            this.PollForLanguageChanges();
        }

        /// <summary>
        /// Check every second for language changes to LoupedeckConfig.
        /// </summary>
        public void PollForLanguageChanges()
        {
            var delay = 1;
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var listener = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var LoupedeckLanguage = this.Localization.LoupedeckLanguage;
                    var PlugInLanguage = this.Localization.CurrentLanguage;
                    if (LoupedeckLanguage != PlugInLanguage)
                    {
                        Console.WriteLine($"[CP] Language has changed from '{PlugInLanguage}' to '{LoupedeckLanguage}'.");
                        this.Localization.SetCurrentLanguage(this.Localization.LoupedeckLanguage);

                        // Write the current language for debugging purposes:
                        Console.WriteLine("[CP] Current Language: " + this.Localization.CurrentLanguage);
                    } 

                    Thread.Sleep(delay);
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Update the Plugin Status.
        /// </summary>
        public void UpdatePluginStatus()
        {
            var CommandPostBundleID = "org.latenitefilms.CommandPost";
            var CommandPostPID = this.NativeMethods.GetProcessId(CommandPostBundleID);
            if (CommandPostPID == -1)
            {
                // CommmandPost is not running:                
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error,
                this.localisation.GetGeneralString("CommandPostNotRunningError"),
                this.localisation.GetGeneralString("CommandPostURL"));
            }
            else if (allSockets.Count == 0)
            {
                // CommandPost is not connected:                
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error,
                this.localisation.GetGeneralString("CommandPostWebSocketError"),
                this.localisation.GetGeneralString("CommandPostURL"));
            }
            else
            {
                // Everything is Normal:
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Normal, null, null);
            }
        }

        /// <summary>
        /// Loads the embedded icons.
        /// </summary>
        public void LoadIcons()
        {
            this.Info.Icon16x16 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-16.png"));
            this.Info.Icon32x32 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-32.png"));
            this.Info.Icon48x48 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-48.png"));
            this.Info.Icon256x256 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-256.png"));
        }

        /// <summary>
        /// The WebSocket Server instance.
        /// </summary>
        public static WebSocketServer socketServer;

        /// <summary>
        /// A List of all the connected clients to the WebSocket server.
        /// </summary>
        public static List<IWebSocketConnection> allSockets;

        /// <summary>
        /// Setup the WebSocket Server.
        /// </summary>
        public void SetupWebSocketServer()
        {
            // Setup the list of all the WebSocket clients:
            allSockets = new List<IWebSocketConnection>();


            // Create a new WebSocket Server that looks at all interfaces:
            socketServer = new WebSocketServer("ws://0.0.0.0:" + this.WebSocketServerPort);

            // Setup the WebSocket Events:
            socketServer.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    // WebSocket Open:                    
                    allSockets.Add(socket);
                    this.UpdatePluginStatus();

                    // Request a list of Commands:
                    var jsonString = JsonSerializer.Serialize(new
                    {
                        actionName = "RequestCommands"
                    });                    
                    this.SendWebSocketMessage(jsonString);
                };
                socket.OnClose = () =>
                {
                    // WebSocket Closed:                 
                    allSockets.Remove(socket);
                    this.UpdatePluginStatus();
                };
                socket.OnError = ex =>
                {
                    // WebSocket Error:                    
                    this.OnPluginStatusChanged(Loupedeck.PluginStatus.Warning,
                        this.localisation.GetGeneralString("CommandPostWebSocketSpecificError") + ex.ToString(),
                        this.localisation.GetGeneralString("CommandPostURL"));
                };
                socket.OnMessage = message =>
                {
                    // WebSocket Message Recieved:
                    WebSocketMessage incomingMessage = JsonSerializer.Deserialize<WebSocketMessage>(message);
                    switch (incomingMessage.MessageType)
                    {
                        case "UpdateDisplay":
                            // Send Event Notification to Adjustments Class:
                            var actionValueUpdatedEventArgs = new ActionValueUpdatedEventArgs(incomingMessage.ActionName, incomingMessage.ActionValue);
                            ActionValueUpdatedEvents?.Invoke(this, actionValueUpdatedEventArgs);
                            break;
                        case "UpdateCommands":
                            // Send Event Notification to classes that has generated commands:
                            var newCommandsValueEventArgs = new NewCommandsValueEventArgs(incomingMessage.ActionName, incomingMessage.ActionValue);
                            NewCommandsEvents?.Invoke(this, newCommandsValueEventArgs);
                            break;
                        case "Ping":
                            // Reply to Ping:
                            var jsonString = JsonSerializer.Serialize(new
                            {
                                actionName = "pong",
                            });
                            this.SendWebSocketMessage(jsonString);                            
                            break;
                        default:
                            // Unknown MessageType:
                            Console.WriteLine("[CP] Unknown WebSocket MessageType Received: " + incomingMessage.MessageType);
                            break;
                    };
                };
            });
        }

        /// <summary>
        /// Sends a WebSocket Message to all clients.
        /// </summary>
        /// <param name="message">The message to send as a string.</param>
        public void SendWebSocketMessage(String message)
        {
            if (message.IsNullOrEmpty()) {
                Console.WriteLine("[CP] ERROR: Tried to send an null or empty WebSocket message.");
            } else {
                var allSockets = CommandPostPlugin.allSockets;
                allSockets.ToList().ForEach(socket => socket.Send(message));
            }            
        }

        /// <summary>
        /// Triggers when the Plugin is unloaded. This is not used for this CommandPost Plugin.
        /// </summary>
        public override void Unload()
        {
        }

        /// <summary>
        /// Triggers when the Application is started. This is not used for this CommandPost Plugin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationStarted(Object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Triggers when the Application is stopped. This is not used for this CommandPost Plugin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationStopped(Object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Run Commmand. This is not used for this CommandPost Plugin.
        /// </summary>
        /// <param name="commandName">The command name as a string.</param>
        /// <param name="parameter">The parameter as a string.</param>
        public override void RunCommand(String commandName, String parameter)
        {
        }

        /// <summary>
        /// Apply Adjustment. This is not used for this CommandPost Plugin.
        /// </summary>
        /// <param name="adjustmentName">The adjustment name as a string.</param>
        /// <param name="parameter">The parameter as a string.</param>
        /// <param name="diff">The different value as an 32-bit interger.</param>
        public override void ApplyAdjustment(String adjustmentName, String parameter, Int32 diff)
        {
        }
    }
}