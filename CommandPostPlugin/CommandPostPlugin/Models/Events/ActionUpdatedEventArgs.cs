using System;

namespace Loupedeck.CommandPostPlugin.Models.Events
{
    public abstract class ActionUpdatedEventArgs : EventArgs
    {
        public string Id { get; }

        protected ActionUpdatedEventArgs(string id)
        {
            Id = id;
        }
    }
}
