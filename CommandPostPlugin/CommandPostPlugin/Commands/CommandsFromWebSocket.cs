namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;

    using Loupedeck.CommandPostPlugin.Localisation;

    /// <summary>
    /// A Loupedeck Commands Plugin that's populated data sent via WebSocket messages.
    /// </summary>
    class CommandsFromWebSocket : PluginDynamicCommand
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
        /// A dictionary of Commands.
        /// </summary>
        private Dictionary<String, String> Commands;

        /// <summary>
        /// A Loupedeck Commands Plugin that's populated data sent via WebSocket messages.
        /// </summary>
        public CommandsFromWebSocket() : base()
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
                Console.WriteLine("[CP] ERROR: CommandsFromWebSocket failed to load CommandPostPlugin.");
                return false;
            }

            // Load the localisation class:
            this.localisation = new CPLocalisation(this.plugin);

            // Get WebSocket Events:
            this.plugin.NewCommandsEvents += (sender, e) =>
            {
                // The actionID contains the plugin type (i.e. "audioEffect"):
                var actionID = e.Id;

                // The actionValue contains a JSON encoded table of plugin items (i.e. audio effects):
                var actionValue = e.ActionValue;                

                // Make sure the actionID and actionValue is valid:
                if (!String.IsNullOrEmpty(actionID) && !String.IsNullOrEmpty(actionValue))
                {
                    // Get the translated Group Name:
                    var GroupName = this.localisation.GetGroupName(actionID);

                    // Deseralize the actionValue data:
                    this.Commands = JsonSerializer.Deserialize<Dictionary<String, String>>(actionValue);
                    foreach (KeyValuePair<String, String> command in this.Commands)
                    {
                        // Add a new parameter to the LoupedeckConfig application:
                        this.AddParameter(command.Key, command.Value, GroupName);
                    }                                
                }
            };

            return true;
        }

        /// <summary>
        /// Triggered when a Command is changed (i.e. a button press).
        /// </summary>
        /// <param name="actionParameter">The action parameter as a string.</param>
        protected override void RunCommand(String actionParameter)
        {
            // Make sure the action parameter is actually valid:
            if (!actionParameter.IsNullOrEmpty())
            {
                // Send WebSocket message when the action is triggered via button press:                
                var jsonString = JsonSerializer.Serialize(new
                {
                    actionName = "ApplyWebSocketCommand",
                    actionValue = actionParameter,
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
        /// <returns>The display name or null if the actionParameter is null or empty.</returns>
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) {

            // Abort is the actionParameter is null or empty:
            if (actionParameter.IsNullOrEmpty()) { return null; }

            // Get Display Name from JSON:
            if (this.Commands.TryGetValue(actionParameter, out var value))
            {
                return value;
            }

            // Something went wrong - return null:
            return null;   
        }
    }
}