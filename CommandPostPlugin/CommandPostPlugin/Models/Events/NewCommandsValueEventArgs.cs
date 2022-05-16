//
// TODO: Ignore this for now - it's a work in progress.
//
namespace Loupedeck.CommandPostPlugin.Models.Events
{
    using System;

    public class NewCommandsValueEventArgs : NewCommandsEventArgs
    {
        public String CommandName { get; set; }
        public String CommandType { get; set; }
        public String CommandCategory { get; set; }
        public String CommandTheme { get; set; }

        public NewCommandsValueEventArgs(String id, String commandName, String commandType, String commandCategory, String commandTheme)
            : base(id)
        {
            this.CommandName = commandName;
            this.CommandType = commandType;
            this.CommandCategory = commandCategory;
            this.CommandTheme = commandTheme;
        }
    }
}
