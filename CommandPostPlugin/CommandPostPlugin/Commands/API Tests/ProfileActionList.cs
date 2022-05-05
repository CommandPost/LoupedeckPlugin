namespace Loupedeck.CommandPostPlugin
{
    using System;

    using Fleck;

    class ProfileActionList : PluginDynamicCommand
    {
        public ProfileActionList() : base("Profile Action List", "Trigger a Profile Action", "API Tests") => this.MakeProfileAction("list;Select something from list:");

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