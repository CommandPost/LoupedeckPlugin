namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Linq;
    using System.Text.Json;

    class SimpleCommand : PluginDynamicCommand
    {
        public class WebSocketMessage
        {            
            public String ActionName { get; set; }
        }

        public SimpleCommand() : base("Simple Command", "Trigger a Simple Command", "API Tests")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            //            
            // Send WebSocket message when the action is triggered via button press:
            //
            var message = new WebSocketMessage
            {                
                ActionName = "SimpleCommand",
            };
            var jsonString = JsonSerializer.Serialize(message);
            var allSockets = CommandPostPlugin.allSockets;
            allSockets.ToList().ForEach(socket => {
                Console.WriteLine("SimpleCommand");
                socket.Send(jsonString);               
            });
        }
    }
}