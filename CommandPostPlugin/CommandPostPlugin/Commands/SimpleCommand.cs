﻿namespace Loupedeck.CommandPostPlugin
{
    using System;

    class SimpleCommand : PluginDynamicCommand
    {
        public SimpleCommand() : base("Simple Command", "Trigger a Simple Command", "API Tests")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            // This is what's triggered when you run the "Simple Command" (i.e. you press a button):
            this.Plugin.ClientApplication.SendKeyboardShortcut(VirtualKeyCode.VolumeMute);
        }
    }
}