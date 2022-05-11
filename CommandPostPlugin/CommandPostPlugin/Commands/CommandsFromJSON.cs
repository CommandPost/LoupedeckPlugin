namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    using Loupedeck.CommandPostPlugin.Localisation;

    class CommandPostCommandsFromJSON : PluginDynamicCommand
    {
        private CommandPostPlugin plugin;
        static readonly CPLocalisation localisation = new CPLocalisation();

        public CommandPostCommandsFromJSON() : base()
        {
        }

        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as CommandPostPlugin;
            if (this.plugin is null)
            {
                return false;
            }

            //
            // Create a new Parameter for each Command:
            //
            var LanguageCode = this.plugin.GetLoupedeckLanguageCode();
            foreach (KeyValuePair<String, String> command in localisation.GetCommands())
            {
                var ActionID = command.Key;
                var GroupID = command.Value;

                var DisplayName = localisation.GetDisplayName(ActionID, LanguageCode);
                var GroupName = localisation.GetGroupName(GroupID, LanguageCode);

                this.AddParameter(ActionID, DisplayName, GroupName);
            }

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
            var LanguageCode = this.plugin.GetLoupedeckLanguageCode();
            var DisplayName = localisation.GetDisplayName(actionParameter, LanguageCode);
            return DisplayName;
        }
    }
}