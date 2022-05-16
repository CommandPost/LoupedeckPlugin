//
// TODO: Ignore this for now - it's a work in progress.
//
namespace Loupedeck.CommandPostPlugin.Models.Events
{
    using System;

    public abstract class NewCommandsEventArgs : EventArgs
    {
        public String Id { get; }

        protected NewCommandsEventArgs(String id) => this.Id = id;
    }
}
