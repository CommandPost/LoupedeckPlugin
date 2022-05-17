namespace Loupedeck.CommandPostPlugin.Models.Events
{
    using System;

    public abstract class NewCommandsEventArgs : EventArgs
    {
        public String Id { get; }

        protected NewCommandsEventArgs(String id) => this.Id = id;
    }
}
