namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;

    using Loupedeck.CommandPostPlugin.Localisation;

    /// <summary>
    /// A Loupedeck Commands Plugin that's populated from data on a JSON file.
    /// </summary>
    class CommandPostCommandsFromJSON : PluginDynamicCommand
    {
        /// <summary>
        /// A link back to the main CommandPostPlugin.
        /// </summary>
        private CommandPostPlugin plugin;

        /// <summary>
        /// A link back to the localisation class.
        /// </summary>
        private CPLocalisation localisation;

        public CommandPostCommandsFromJSON() : base()
        {
        }

        /// <summary>
        /// Triggered when the PluginDynamicCommand first loads.
        /// </summary>
        /// <returns>A boolean which states if the PluginDynamicCommand loaded successfully or not.</returns>
        protected override Boolean OnLoad()
        {
            // Create a link to the main plugin:
            this.plugin = base.Plugin as CommandPostPlugin;
            if (this.plugin is null)
            {
                Console.WriteLine("[CP] ERROR: CommandPostCommandsFromJSON failed to load CommandPostPlugin.");
                return false;
            }

            // Load the localisation class:
            this.localisation = new CPLocalisation(this.plugin);

            // Create a new Parameter for each Command:
            foreach (KeyValuePair<String, String> command in this.localisation.GetCommands())
            {
                var ActionID = command.Key;
                var GroupID = command.Value;

                var DisplayName = this.localisation.GetDisplayName(ActionID);
                var GroupName = this.localisation.GetGroupName(GroupID);

                // Make sure all the strings are valid before continuing:
                if (ActionID.IsNullOrEmpty() || DisplayName.IsNullOrEmpty() || GroupName.IsNullOrEmpty())
                {
                    // Write a debug message for developers:
                    Console.WriteLine($"[CP] Error: An ActionID ('{ActionID}'), DisplayName ('{DisplayName}') or GroupName ('{GroupName}') is null or empty.");
                } else {
                    this.AddParameter(ActionID, DisplayName, GroupName);
                }
            }

            return true;
        }

        /// <summary>
        /// Triggered when a Command is changed (i.e. a button press).
        /// </summary>
        /// <param name="actionParameter">The action parameter as a string.</param>
        protected override void RunCommand(String actionParameter)
        {
            // Make sure the action parameter is actually valid:
            if (actionParameter != "")
            {
                // Send WebSocket message when the action is triggered via button press:                
                var jsonString = JsonSerializer.Serialize(new
                {
                    actionName = actionParameter
                });
                this.plugin.SendWebSocketMessage(jsonString);

                // Inform Loupedeck that command display name and/or image has changed:
                this.ActionImageChanged(actionParameter);
            }
        }

        /// <summary>
        /// Get the Command Display Name.
        /// </summary>
        /// <param name="actionParameter">The action parameter as a string.</param>
        /// <param name="imageSize">The image size as a PluginImageSize.</param>
        /// <returns>The Display Name as a string or null if the actionParameter is null or empty.</returns>
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            // Abort is the actionParameter is null or empty:
            if (actionParameter.IsNullOrEmpty()) { return null; }

            // Get Display Name from JSON:
            var DisplayName = this.localisation.GetDisplayName(actionParameter);
            if (DisplayName.IsNullOrEmpty())
            {
                Console.WriteLine("[CP] ERROR: GetCommandDisplayName is null or empty: " + actionParameter);
            }
            return DisplayName;
        }
    }
}