namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Text.Json;

    using Loupedeck.Devices.Loupedeck2Devices;
    using Loupedeck.CommandPostPlugin.Localisation;

    /// <summary>
    /// Color Wheel Adjustments for the Loupedeck CT Touch Wheel.
    /// </summary>
    public abstract class WheelToolColorWheelBase : WheelTool
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
        /// Color Wheel Adjustments for the Loupedeck CT Touch Wheel.
        /// </summary>
        /// <param name="templateDisplayName">The name of the Touch Wheel tool that appears in the LoupedeckConfig application.</param>
        public WheelToolColorWheelBase(String TemplateDisplayName) : base(TemplateDisplayName, "CP Color Wheels")
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
                Console.WriteLine("[CP] ERROR: WheelToolColorWheelBase failed to load CommandPostPlugin.");
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
        /// Triggered when the Wheel Tool Stops.
        /// </summary>
        protected override void OnStop()
        {
        }

        /// <summary>
        /// Gets the Embedded Image
        /// </summary>
        /// <returns></returns>
        private BitmapImage GetEmbeddedImage()
        {
            using (var backgroundImage = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("WheelToolColorWheelBase.png")))
            {
                using (var bitmapBuilder = this.CreateBitmapBuilder())
                {
                    // Get the translated display name:
                    var translatedDisplayName = this.localisation.GetDisplayName(this.TemplateDisplayName);

                    bitmapBuilder.SetBackgroundImage(backgroundImage);
                    bitmapBuilder.DrawText(translatedDisplayName, 0, 80, 240, 80, new BitmapColor(220, 220, 220), 26);

                    return bitmapBuilder.ToImage();
                }
            }
        }

        /// <summary>
        /// Triggers when Loupedeck needs a Demo Image.
        /// </summary>
        /// <returns>A bitmap image.</returns>
        protected override BitmapImage CreateDemoImage() => this.GetEmbeddedImage();

        /// <summary>
        /// Triggers when Loupedeck needs an image for the Touch Wheel.
        /// </summary>
        /// <returns>A bitmap image.</returns>
        protected override BitmapImage CreateImage() => this.GetEmbeddedImage();

        /// <summary>
        /// Triggers when a Touch Event takes place.
        /// </summary>
        /// <param name="deviceTouchEvent">The Touch Event data.</param>
        protected override void OnTouchEvent(DeviceTouchEvent deviceTouchEvent)
        {
            // Double Tap Event - Reset the Color Wheel:
            if (DeviceTouchEventType.DoubleTap == deviceTouchEvent.EventType)
            {                
                // Send WebSocket message when the action is triggered via knob press:           
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
                // Send WebSocket message when the user moves their finger on the Touch Screen:
                var jsonString = JsonSerializer.Serialize(new
                {
                    actionName      = this.TemplateDisplayName,
                    actionType      = "move",
                    deltaX          = deviceTouchEvent.DeltaX,
                    deltaY          = deviceTouchEvent.DeltaY,
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
                actionName          = this.TemplateDisplayName,
                actionType          = "wheel",
                actionValue         = deviceEncoderEvent.Clicks.ToString(),
                functionPressed     = deviceEncoderEvent.IsFnPressed(),
            });
            this.plugin.SendWebSocketMessage(jsonString);            
        }
    }

    /// <summary>
    /// Touch Wheel controls for the Color Wheel Master.
    /// </summary>
    public class WheelToolColorWheelMaster : WheelToolColorWheelBase
    {
        public WheelToolColorWheelMaster() : base("FCP Global")
        {
        }
    }

    /// <summary>
    /// Touch Wheel controls for the Color Wheel Shadows.
    /// </summary>
    public class WheelToolColorWheelShadows : WheelToolColorWheelBase
    {
        public WheelToolColorWheelShadows() : base("FCP Shadows")
        {
        }
    }

    /// <summary>
    /// Touch Wheel controls for the Color Wheel Midtones.
    /// </summary>
    public class WheelToolColorWheelMidtones : WheelToolColorWheelBase
    {
        public WheelToolColorWheelMidtones() : base("FCP Midtones")
        {
        }
    }

    /// <summary>
    /// Touch Wheel controls for the Color Wheel Highlights.
    /// </summary>
    public class WheelToolColorWheelHighlights : WheelToolColorWheelBase
    {
        public WheelToolColorWheelHighlights() : base("FCP Highlights")
        {
        }
    }
    
}