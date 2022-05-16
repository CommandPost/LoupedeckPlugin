//
// TODO: Ignore this for now - it's a work in progress.
//
namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    using Loupedeck.CommandPostPlugin.Localisation;

    class VideoEffects : PluginDynamicCommand
    {
        // NewCommandsValueEventArgs
        private CommandPostPlugin plugin;
        private CPLocalisation localisation;

        public VideoEffects() : base()
        {
        }

        protected override Boolean OnLoad()
        {
            //
            // Create a link to the main plugin:
            //
            this.plugin = base.Plugin as CommandPostPlugin;
            if (this.plugin is null)
            {
                return false;
            }

            //
            // Load the localisation class:
            //
            this.localisation = new CPLocalisation(this.plugin);

            //
            // Create a new Parameter for each Command:
            //
            /*
            foreach (KeyValuePair<String, String> command in this.localisation.GetCommands())
            {
                var ActionID = command.Key;
                var GroupID = command.Value;

                var DisplayName = this.localisation.GetDisplayName(ActionID);
                var GroupName = this.localisation.GetGroupName(GroupID);

                this.AddParameter(ActionID, DisplayName, GroupName);
            }
            */

            //
            // Get WebSocket Events:
            //
            this.plugin.ActionValueUpdatedEvents += (sender, e) =>
            {
                var actionValue = e.ActionValue;
                var actionID = e.Id;

                if (!String.IsNullOrEmpty(actionID) && !String.IsNullOrEmpty(actionValue))
                {
                    //this.cachedValues[e.Id] = actionValue;
                    this.ActionImageChanged();
                }
            };

            return true;
        }

        protected override void RunCommand(String actionParameter)
        {
            if (actionParameter != "")
            {
                //            
                // Send WebSocket message when the action is triggered via button press:
                //
                var jsonString = JsonSerializer.Serialize(new
                {
                    actionName = actionParameter
                });
                var allSockets = CommandPostPlugin.allSockets;
                allSockets.ToList().ForEach(socket => socket.Send(jsonString));

                //
                // Inform Loupedeck that command display name and/or image has changed:
                //
                this.ActionImageChanged(actionParameter);
            }
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            //
            // Get Display Name from JSON:
            //
            var DisplayName = this.localisation.GetDisplayName(actionParameter);
            return DisplayName;
        }
    }
}