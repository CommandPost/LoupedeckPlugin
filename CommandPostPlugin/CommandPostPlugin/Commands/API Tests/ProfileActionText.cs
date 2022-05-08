namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Linq;
    using System.Text.Json;

    class ProfileActionText : PluginDynamicCommand
    {
        public ProfileActionText() : base("Profile Action Text", "Trigger a Profile Action", "API Tests") => this.MakeProfileAction("text;Enter message to send:");

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