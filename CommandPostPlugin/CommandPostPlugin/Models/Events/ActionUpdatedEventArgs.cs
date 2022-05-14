namespace Loupedeck.CommandPostPlugin.Models.Events
{
    using System;

    public abstract class ActionUpdatedEventArgs : EventArgs
    {
        public String Id { get; }

        protected ActionUpdatedEventArgs(String id)
        {
            this.Id = id;
        }
    }
}
