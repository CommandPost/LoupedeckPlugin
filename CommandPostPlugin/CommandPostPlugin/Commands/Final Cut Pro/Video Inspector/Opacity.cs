namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Linq;
    using System.Text.Json;

    class VideoInspectorOpacity : PluginDynamicAdjustment
    {
        public VideoInspectorOpacity() : base("Opacity", "Controls the Opacity Slider in the Final Cut Pro Video Inspector", "FCP: Video Inspector", true)
        {
        }

        private CommandPostPlugin plugin;

        //
        // Cached Screen Value:
        // 
        private String cachedValue = "?";

        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as CommandPostPlugin;
            if (this.plugin is null)
            {
                return false;
            }

            //
            // Get WebSocket Events:
            //
            this.plugin.ActionValueUpdatedEvents += (sender, e) =>
            {
                var actionValue = e.ActionValue;
                this.cachedValue = actionValue;
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
                actionName      = "VideoInspectorOpacity",
                actionType      = "turn",
                actionValue     = value,
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
                actionName = "VideoInspectorOpacity",
                actionType = "press",
                actionValue = "",
            });
            var allSockets = CommandPostPlugin.allSockets;
            allSockets.ToList().ForEach(socket => socket.Send(jsonString));
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            //
            // Get the value of the adjustment from cache:
            //
            return this.cachedValue;            
        }
    }
}