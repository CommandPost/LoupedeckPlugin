namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Collections.Generic;

    using Loupedeck.CommandPostPlugin.Models.Events;

    using Fleck;    

    public class WebSocketMessage
    {
        public String MessageType { get; set; }
        public String ActionName { get; set; }
        public String ActionValue { get; set; }
    }

    public class CommandPostPlugin : Plugin
    {
        public event EventHandler<ActionValueUpdatedEventArgs> ActionValueUpdatedEvents;

        //
        // Does not require an application to be in the foreground or any application to be running locally at all:
        //
        public override Boolean HasNoApplication => true;

        //
        // API-based actions, those that are controlling target application/service using dedicated API:
        //
        public override Boolean UsesApplicationApiOnly => true;

        public override void Load()
        {
            //
            // Load CommandPost Icon from the Embedded Resources:
            //
            this.LoadIcons();

            //
            // Start WebSocket Server:
            //
            this.SetupSocketServer();
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
                };
                socket.OnClose = () =>
                {
                    //
                    // WebSocket Closed:
                    //
                    allSockets.Remove(socket);
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
                        // Update Display:
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


