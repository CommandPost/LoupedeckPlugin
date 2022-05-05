namespace Loupedeck.CommandPostPlugin
{
    using System;

    using Fleck;

    class ProfileActionText : PluginDynamicCommand
    {
        public ProfileActionText() : base("Profile Action Text", "Trigger a Profile Action", "API Tests") => this.MakeProfileAction("text;Enter message to send:");

        protected override void RunCommand(String actionParameter)
        {
            //
            // This is what's triggered when you run the "Simple Command" (i.e. you press a button):
            //
            foreach (IWebSocketConnection socket in Loupedeck.CommandPostPlugin.CommandPostPlugin.allSockets)
            {
                socket.Send(actionParameter);
            }
        }
    }
}