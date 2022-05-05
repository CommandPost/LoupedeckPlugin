namespace Loupedeck.CommandPostPlugin
{
    using System;

    using Loupedeck.Devices.Loupedeck2Devices;

    public abstract class WheelToolColorWheelBase : WheelTool
    {
        private readonly String _adjustmentName = "ChangeColorWheel";
        private readonly String _commandName = "ResetChangeColorWheel";

        private readonly String _adjustmentParameter;
        private readonly String _adjustmentParameterZ;

        public WheelToolColorWheelBase(String templateDisplayName, String adjustmentParameter, String adjustmentParameterZ) : base(templateDisplayName, "CP Color Wheels")
        {
            this.LockOnStart = true;

            this._adjustmentParameter = adjustmentParameter;
            this._adjustmentParameterZ = adjustmentParameterZ;
        }

        protected override void OnStart()
        {
            this.Draw();
        }

        protected override void OnStop()
        {
        }

        private BitmapImage GetEmbeddedImage()
        {
            using (var backgroundImage = EmbeddedResources.ReadImage(EmbeddedResources.FindFile("WheelToolColorWheelBase.png")))
            {
                using (var bitmapBuilder = this.CreateBitmapBuilder())
                {
                    bitmapBuilder.SetBackgroundImage(backgroundImage);
                    bitmapBuilder.DrawText(this.Plugin.Localization.GetString(this.TemplateDisplayName), 0, 80, 240, 80, new BitmapColor(220, 220, 220), 26);

                    return bitmapBuilder.ToImage();
                }
            }
        }

        protected override BitmapImage CreateDemoImage() => this.GetEmbeddedImage();

        protected override BitmapImage CreateImage() => this.GetEmbeddedImage();

        protected override void OnTouchEvent(DeviceTouchEvent deviceTouchEvent)
        {
            // RESET
            if (DeviceTouchEventType.DoubleTap == deviceTouchEvent.EventType)
            {                
                Tracer.Trace($"Wheel Double tap: reset {this._commandName}");
                this.ExecuteAction(this._commandName, this._adjustmentParameter, 0);

            }
            else if (DeviceTouchEventType.Move == deviceTouchEvent.EventType)
            {
                // Encode both variables to single diff

                var bytesX = BitConverter.GetBytes((Int16)deviceTouchEvent.DeltaX);
                var bytesY = BitConverter.GetBytes((Int16)(-deviceTouchEvent.DeltaY));

                var bytes = new Byte[16];
                bytes[0] = bytesX[0];
                bytes[1] = bytesX[1];
                bytes[2] = bytesY[0];
                bytes[3] = bytesY[1];
                Tracer.Trace($"Raw deltaX: {deviceTouchEvent.DeltaX}, deltaY: {-deviceTouchEvent.DeltaY}, bytes: {bytes} ");

                this.ExecuteAction(this._adjustmentName, this._adjustmentParameter, BitConverter.ToInt32(bytes, 0));
            }
        }

        protected override void OnEncoderEvent(DeviceEncoderEvent deviceEncoderEvent)
        {
            if (!String.IsNullOrEmpty(this._adjustmentParameterZ))
            {
                var parameter = !deviceEncoderEvent.IsFnPressed() ? this._adjustmentParameterZ : this._adjustmentParameterZ + "_FN";
                this.ExecuteAction(this._adjustmentName, parameter, deviceEncoderEvent.Clicks);
            }
        }
    }

    public class WheelToolColorWheelMaster : WheelToolColorWheelBase
    {
        public WheelToolColorWheelMaster() : base("CP Global", "Master.XY", "Master.Wheel")
        {
        }
    }

    public class WheelToolColorWheelShadows : WheelToolColorWheelBase
    {
        public WheelToolColorWheelShadows() : base("CP Shadows", "Shadows.XY", "Shadows.Wheel")
        {
        }
    }


    public class WheelToolColorWheelMidtones : WheelToolColorWheelBase
    {
        public WheelToolColorWheelMidtones() : base("CP Midtones", "Midtones.XY", "Midtones.Z")
        {
        }
    }
    public class WheelToolColorWheelHighlights : WheelToolColorWheelBase
    {
        public WheelToolColorWheelHighlights() : base("CP Highlights", "Highlights.XY", "Highlights.Z")
        {
        }
    }
    
}