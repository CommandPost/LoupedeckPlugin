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
        private Dictionary<String, String> cachedValues;
        
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
            // Make sure the actionParameter is valid:
            if (!actionParameter.IsNullOrEmpty())
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
        }

        /// <summary>
        /// Triggered when an adjustment is changed (i.e. a knob button is pressed).
        /// </summary>
        /// <param name="actionParameter">The action parameter as a string.</param>
        protected override void RunCommand(String actionParameter)
        {
            // Make sure the actionParameter is valid:
            if (!actionParameter.IsNullOrEmpty())
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
        }

        /// <summary>
        /// Get the Adjustment Value.
        /// </summary>
        /// <param name="actionParameter">The action parameter as a string.</param>
        /// <returns>A string value that you want to display on the physical hardware screen or "?" if a value isn't already cached. If the actionParameter is null or empty it'll return null.</returns>
        protected override String GetAdjustmentValue(String actionParameter)
        {
            // Abort if the actionParameter is null or empty:
            if (actionParameter.IsNullOrEmpty()) { return null;  }

            // Get the value of the adjustment from cache:         
            return this.cachedValues.ContainsKey(actionParameter)
                ? this.cachedValues[actionParameter]
                : "?";
        }

        /// <summary>
        /// Get Adjustment Display Name.
        /// </summary>
        /// <param name="actionParameter">The action parameter as a string.</param>
        /// <param name="imageSize">The image size as a PluginImageSize.</param>
        /// <returns></returns>
        protected override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            // Abort is the actionParameter is null or empty:
            if (actionParameter.IsNullOrEmpty()) { return null; }

            // Get Display Name from JSON:
            var DisplayName = this.localisation.GetDisplayName(actionParameter);
            return DisplayName;            
        }
    }
}