namespace Loupedeck.CommandPostPlugin.Models.Events
{
    using System;

    /// <summary>
    /// Language Changed Event Arguments.
    /// </summary>
    public class LanguageChangedValueEventArgs : LanguageChangedEventArgs
    {
        public String NewLanguage { get; set; }

        public LanguageChangedValueEventArgs(String id, String newLanguage)
            : base(id) => this.NewLanguage = newLanguage;
    }
}