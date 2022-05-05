namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Linq;

    class SimpleCommand : PluginDynamicCommand
    {
        public SimpleCommand() : base("Simple Command", "Trigger a Simple Command", "API Tests")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            //
            // This is what's triggered when you run the "Simple Command" (i.e. you press a button):
            //
            Console.WriteLine("SimpleCommand");

            // Send message to sockets:            
            var allSockets = CommandPostPlugin.allSockets;
            allSockets.ToList().ForEach(s => {
                s.Send("SimpleCommand");
                Console.WriteLine("SimpleCommand sent to Socket");
            });

        }
    }
}