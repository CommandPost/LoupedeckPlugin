namespace Loupedeck.CommandPostPlugin.Models.Events
{
    using System;

    public abstract class LanguageChangedEventArgs : EventArgs
    {
        public String Id { get; }

        protected LanguageChangedEventArgs(String id) => this.Id = id;
    }
}
