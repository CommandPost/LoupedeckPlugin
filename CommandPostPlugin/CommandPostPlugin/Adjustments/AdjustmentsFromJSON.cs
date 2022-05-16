namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;

    using Loupedeck.CommandPostPlugin.Localisation;

    /// <summary>
    /// A Loupedeck Adjustment Plugin that's populated from data on a JSON file.
    /// </summary>
    class CommandPostAdjustmentsFromJSON : PluginDynamicAdjustment
    {
        /// <summary>
        /// A link back to the main CommandPostPlugin.
        /// </summary>
        private CommandPostPlugin plugin;

        /// <summary>
        /// A link back to the localisation class.
        /// </summary>
        private CPLocalisation localisation;

        /// <summary>
        /// A dictionary of cached values for updating the physical Loupedeck displays
        /// </summary>
        Dictionary<String, String> cachedValues;

        /// <summary>
        /// A Loupedeck Adjustment Plugin that's populated from data on a JSON file.
        /// </summary>
        public CommandPostAdjustmentsFromJSON() : base(true)
        {
        }

        /// <summary>
        /// Triggered when the PluginDynamicAdjustment loads.
        /// </summary>
        /// <returns>A boolean which states if the PluginDynamicAdjustment loaded successfully or not.</returns>
        protected override Boolean OnLoad()
        {
            // Create a link to the main plugin:
            this.plugin = base.Plugin as CommandPostPlugin;
            if (this.plugin is null)
            {
                Console.WriteLine("[CP] ERROR: CommandPostAdjustmentsFromJSON failed to load CommandPostPlugin.");
                return false;
            }

            // Load the localisation class:
            this.localisation = new CPLocalisation(this.plugin);

            // Setup our display cache:
            this.cachedValues = new Dictionary<String, String>();

            // Create a new Parameter for each Command:
            foreach (KeyValuePair<String, String> command in this.localisation.GetAdjustments())
            {
                var ActionID = command.Key;
                var GroupID = command.Value;

                var DisplayName = this.localisation.GetDisplayName(ActionID);
                var GroupName = this.localisation.GetGroupName(GroupID);

                this.AddParameter(ActionID, DisplayName, GroupName);
            }

            // Get WebSocket Events:
            this.plugin.ActionValueUpdatedEvents += (sender, e) =>
            {                
                var actionValue = e.ActionValue;
                var actionID = e.Id;

                // Make sure the actionID and actionValue is valid:
                if (!String.IsNullOrEmpty(actionID) && !String.IsNullOrEmpty(actionValue)) {
                    this.cachedValues[e.Id] = actionValue;
                    this.ActionImageChanged();
                }
            };

            return true;
        }

        /// <summary>
        /// Triggered when an adjustment is change (i.e. a knob is turned).
        /// </summary>
        /// <param name="actionParameter">The action parameter as a string.</param>
        /// <param name="value">The parameter value as an Int32.</param>
        protected override void ApplyAdjustment(String actionParameter, Int32 value)
        {
            // Send WebSocket message when the action is triggered via knob turn:            
            var jsonString = JsonSerializer.Serialize(new
            {
                actionName = actionParameter,
                actionType = "turn",
                actionValue = value,
            });
            this.plugin.SendWebSocketMessage(jsonString);            
        }

        /// <summary>
        /// Triggered when an adjustment is changed (i.e. a knob button is pressed).
        /// </summary>
        /// <param name="actionParameter">The action parameter as a string.</param>
        protected override void RunCommand(String actionParameter)
        {            
            // Send WebSocket message when the action is triggered via knob press:           
            var jsonString = JsonSerializer.Serialize(new
            {
                actionName = actionParameter,
                actionType = "press",
                actionValue = "",
            });
            this.plugin.SendWebSocketMessage(jsonString);
        }

        /// <summary>
        /// Get the Adjustment Value.
        /// </summary>
        /// <param name="actionParameter">The action parameter as a string.</param>
        /// <returns>A string value that you want to display on the physical hardware screen or "?" if a value isn't already cached.</returns>
        protected override String GetAdjustmentValue(String actionParameter)
        {
            // Get the value of the adjustment from cache:         
            return this.cachedValues.ContainsKey(actionParameter)
                ? this.cachedValues[actionParameter]
                : "?";
        }
    }
}