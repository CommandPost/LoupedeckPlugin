namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    using Loupedeck.CommandPostPlugin.Localisation;

    class CommandPostAdjustmentsFromJSON : PluginDynamicAdjustment
    {
        private CommandPostPlugin plugin;
        Dictionary<String, String> cachedValues;

        static readonly CPLocalisation localisation = new CPLocalisation();

        public CommandPostAdjustmentsFromJSON() : base(true)
        {         
        }

        protected override Boolean OnLoad()
        {
            this.cachedValues = new Dictionary<String, String>();

            this.plugin = base.Plugin as CommandPostPlugin;
            if (this.plugin is null)
            {
                return false;
            }

            //
            // Create a new Parameter for each Command:
            //
            var LanguageCode = this.plugin.GetLoupedeckLanguageCode();
            foreach (KeyValuePair<String, String> command in localisation.GetAdjustments())
            {
                var ActionID = command.Key;
                var GroupID = command.Value;

                var DisplayName = localisation.GetDisplayName(ActionID, LanguageCode);
                var GroupName = localisation.GetGroupName(GroupID, LanguageCode);

                this.AddParameter(ActionID, DisplayName, GroupName);
            }

            //
            // Get WebSocket Events:
            //
            this.plugin.ActionValueUpdatedEvents += (sender, e) =>
            {
                var actionValue = e.ActionValue;
                this.cachedValues[e.Id] = actionValue;
                this.ActionImageChanged();
            };

            return true;
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 value)
        {
            //            
            // Send WebSocket message when the action is triggered via knob turn:
            //            
            var jsonString = JsonSerializer.Serialize(new
            {
                actionName = actionParameter,
                actionType = "turn",
                actionValue = value,
            });
            var allSockets = CommandPostPlugin.allSockets;
            allSockets.ToList().ForEach(socket => socket.Send(jsonString));
        }

        protected override void RunCommand(String actionParameter)
        {
            //            
            // Send WebSocket message when the action is triggered via knob press:
            //
            var jsonString = JsonSerializer.Serialize(new
            {
                actionName = actionParameter,
                actionType = "press",
                actionValue = "",
            });
            var allSockets = CommandPostPlugin.allSockets;
            allSockets.ToList().ForEach(socket => socket.Send(jsonString));
        }

        protected override String GetAdjustmentValue(String actionParameter) =>
           //
           // Get the value of the adjustment from cache:
           //
           this.cachedValues[actionParameter];           
    }
}