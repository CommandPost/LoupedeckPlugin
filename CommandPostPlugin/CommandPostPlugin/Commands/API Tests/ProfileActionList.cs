namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Linq;
    using System.Text.Json;

    class ProfileActionList : PluginDynamicCommand
    {
        public ProfileActionList() : base("Profile Action List", "Trigger a Profile Action", "API Tests") => this.MakeProfileAction("list;Select something from list:");

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