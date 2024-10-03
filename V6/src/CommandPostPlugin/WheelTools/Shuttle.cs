namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Text.Json;

    using Loupedeck.Devices.Loupedeck2Devices;
    using Loupedeck.CommandPostPlugin.Localisation;    

    /// <summary>
    /// Shuttle Controls for the Loupedeck CT Touch Wheel.
    /// </summary>
    public class CPWheelToolShuttle : WheelTool
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
        /// Shuttle Controls for the Loupedeck CT Touch Wheel.
        /// </summary>
        public CPWheelToolShuttle() : base("FCP Shuttle", "FCP: Timeline")
        {
        }

        /// <summary>
        /// Triggered when the WheelToolColorWheelBase class initalises:
        /// </summary>
        protected override void OnInit()
        {
            // Create a link to the main plugin:
            this.plugin = base.Plugin as CommandPostPlugin;
            if (this.plugin is null)
            {
                Console.WriteLine("[CP] ERROR: CPWheelToolShuttle failed to load CommandPostPlugin.");
            }

            // Load the localisation class:
            this.localisation = new CPLocalisation(this.plugin);

            // Trigger the Base Initilisation:
            base.OnInit();
        }

        /// <summary>
        /// Triggered when the Wheel Tool Starts.
        /// </summary>
        protected override void OnStart() => this.Draw();

        /// <summary>
        /// Creates the Bitmap Image for the Touch Wheel.
        /// </summary>
        /// <returns></returns>
        protected override BitmapImage CreateImage() => EmbeddedResources.ReadImage(EmbeddedResources.FindFile("WheelToolShuttle.png"));

        /// <summary>
        /// Triggers when a Touch Event takes place.
        /// </summary>
        /// <param name="deviceTouchEvent">The Touch Event data.</param>
        protected override void OnTouchEvent(DeviceTouchEvent deviceTouchEvent)
        {
            // Double Tap Event - Reset the Color Wheel:
            if (DeviceTouchEventType.Tap == deviceTouchEvent.EventType)
            {
                // Send WebSocket message when the action is triggered via knob press:           
                var jsonString = JsonSerializer.Serialize(new
                {
                    actionName = this.TemplateDisplayName,
                    actionType = "tap",
                    functionPressed = deviceTouchEvent.IsFnPressed(),
                });
                this.plugin.SendWebSocketMessage(jsonString);
            }
            else if (DeviceTouchEventType.TwoFingerTap == deviceTouchEvent.EventType)
            {
                // Send WebSocket message when the action is triggered:
                var jsonString = JsonSerializer.Serialize(new
                {
                    actionName = this.TemplateDisplayName,
                    actionType = "twoFingerTap",
                    functionPressed = deviceTouchEvent.IsFnPressed(),
                });
                this.plugin.SendWebSocketMessage(jsonString);
            }
            else if (DeviceTouchEventType.DoubleTap == deviceTouchEvent.EventType)
            {
                // Send WebSocket message when the action is triggered:
                var jsonString = JsonSerializer.Serialize(new
                {
                    actionName = this.TemplateDisplayName,
                    actionType = "doubleTap",
                    functionPressed = deviceTouchEvent.IsFnPressed(),
                });
                this.plugin.SendWebSocketMessage(jsonString);
            }
            else if (DeviceTouchEventType.Move == deviceTouchEvent.EventType)
            {
                // Send WebSocket message when the action is triggered:
                var jsonString = JsonSerializer.Serialize(new
                {
                    actionName = this.TemplateDisplayName,
                    actionType = "move",
                    deltaX = deviceTouchEvent.DeltaX,
                    deltaY = deviceTouchEvent.DeltaY,
                    functionPressed = deviceTouchEvent.IsFnPressed(),
                });
                this.plugin.SendWebSocketMessage(jsonString);
            }
        }

        /// <summary>
        /// Triggeres when the Touch Wheel changes value (i.e. a user rotates the outer wheel).
        /// </summary>
        /// <param name="deviceEncoderEvent">The device encoder event./param>
        protected override void OnEncoderEvent(DeviceEncoderEvent deviceEncoderEvent)
        {
            // Send WebSocket message when the wheel is rotated:
            var jsonString = JsonSerializer.Serialize(new
            {
                actionName = this.TemplateDisplayName,
                actionType = "wheel",
                actionValue = deviceEncoderEvent.Clicks,
                functionPressed = deviceEncoderEvent.IsFnPressed(),
            });
            this.plugin.SendWebSocketMessage(jsonString);
        }
    }
}