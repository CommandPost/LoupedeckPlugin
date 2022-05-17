namespace Loupedeck.CommandPostPlugin.Models.Events
{
    using System;

    /// <summary>
    /// New Commands Value Event Arguments.
    /// </summary>
    public class NewCommandsValueEventArgs : NewCommandsEventArgs
    {
        public String ActionValue { get; set; }

        public NewCommandsValueEventArgs(String id, String actionValue)
            : base(id) => this.ActionValue = actionValue;
    }
}