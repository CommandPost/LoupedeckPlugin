namespace Loupedeck.CommandPostPlugin.Models.Events
{
    public class ActionValueUpdatedEventArgs : ActionUpdatedEventArgs
    {
        public string ActionValue { get; set; }

        public ActionValueUpdatedEventArgs(string id, string actionValue)
            : base(id)
        {
            ActionValue = actionValue;
        }
    }
}