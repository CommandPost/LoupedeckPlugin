namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Linq;
    using System.Text.Json;

    class CommandPostFavourites : PluginDynamicCommand
    {
        public CommandPostFavourites() : base()
        {
            for (var i = 1; i <= 20; i++)
            {
                var actionParameter = i.ToString();
                this.AddParameter(actionParameter, "Favourite " + i.ToString("D2"), "CommandPost Favourites");
            }
        }

        protected override void RunCommand(String actionParameter)
        {
            if (Int32.TryParse(actionParameter, out var i))
            {
                //            
                // Send WebSocket message when the action is triggered via button press:
                //
                var jsonString = JsonSerializer.Serialize(new
                {
                    actionName = "CommandPostFavourite" + i.ToString("D2"),                    
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
            if (Int32.TryParse(actionParameter, out var i))
            {
                return "Favourite " + i.ToString("D2");
            }
            else
            {
                return null;
            }
        }
    }
}