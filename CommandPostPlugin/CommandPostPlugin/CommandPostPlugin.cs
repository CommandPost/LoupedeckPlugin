namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Fleck;

    public class CommandPostPlugin : Plugin
    {
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
            this.Info.Icon16x16 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-16.png"));
            this.Info.Icon32x32 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-32.png"));
            this.Info.Icon48x48 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-48.png"));
            this.Info.Icon256x256 = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("icon-256.png"));

            //
            // Start WebSocket Server:
            //
            this.SetupSocketServer();
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
                    Console.WriteLine("Open!");
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("Close!");
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    Console.WriteLine("Client Says: " + message);
                    socket.Send(message);
                    allSockets.ToList().ForEach(s => s.Send("Echo: " + message));
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


