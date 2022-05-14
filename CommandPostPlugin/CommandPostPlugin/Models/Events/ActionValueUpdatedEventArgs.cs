namespace Loupedeck.CommandPostPlugin.Models.Events
{
    using System;

    public class ActionValueUpdatedEventArgs : ActionUpdatedEventArgs
    {
        public String ActionValue { get; set; }

        public ActionValueUpdatedEventArgs(String id, String actionValue)
            : base(id) => this.ActionValue = actionValue;
    }
}