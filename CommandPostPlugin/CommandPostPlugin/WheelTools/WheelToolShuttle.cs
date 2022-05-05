namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Timers;

    using Loupedeck.Devices.Loupedeck2Devices;

    public class WheelToolShuttle : WheelTool
    {
        public WheelToolShuttle() : base("CP Shuttle", "CP Navigation")
        {
        }

        protected override void OnStart() => this.Draw();

        protected override BitmapImage CreateImage() => EmbeddedResources.ReadImage("Loupedeck.CommandPostPlugin.WheelTools.WheelToolShuttle.png");

        protected override void OnTouchEvent(DeviceTouchEvent deviceTouchEvent)
        {
            if (DeviceTouchEventType.Tap == deviceTouchEvent.EventType)
            {
                this.ExecuteAction("Shortcuts", "PlayPause", 0);
            }
        }

        private DateTime _lastTickTime = DateTime.Now;
        private readonly EverySecondTick _everySecondTick = new EverySecondTick(2);
        private readonly EverySecondTick _everySecondTickFn = new EverySecondTick(2);
        private readonly Int32 fastSpeedUseNth = 2;
        private Int32 fastSpeedTickCounter = 2;


        protected override void OnEncoderEvent(DeviceEncoderEvent deviceEncoderEvent)
        {
            var now = DateTime.Now;
            var delay = (now - this._lastTickTime).TotalMilliseconds;
            this._lastTickTime = now;

            var clicks = deviceEncoderEvent.Clicks;

            if (deviceEncoderEvent.IsFnPressed())
            {
                // TODO: uncomment line below and remove all other lines when per action wheel sensitivity is in place
                //this.ExecuteAction("Step1", null, ticks);

                if (this._everySecondTickFn.UpdateTicks(ref clicks))
                {
                    this.ExecuteAction("Navigate1Frame", null, clicks);
                }
            }
            else if (delay < this._delay1)
            {
                if (this._everySecondTick.UpdateTicks(ref clicks))
                {
                    if (this.fastSpeedTickCounter == this.fastSpeedUseNth)
                    {
                        this.ExecuteAction("ChangeClip", null, clicks);
                        this.fastSpeedTickCounter = 0;
                    }
                    else
                    {
                        this.fastSpeedTickCounter++;
                    }
                }
            }
            else if (delay < this._delay2)
            {
                this.ExecuteAction("Navigate10Frames", null, clicks);
            }
            else
            {
                this.ExecuteAction("Navigate1Frame", null, clicks);
            }
        }

#if ENABLE_FILE_WATCHDOG
        private FileSystemWatchdog _watchdog = new FileSystemWatchdog(1000);
#endif
        private Int32 _delay1 = 50;
        private Int32 _delay2 = 100;
        private String _delaysFileName;

        protected override void OnInit()
        {
            this._delaysFileName = System.IO.Path.Combine(this.Plugin.GetPluginDataDirectory(), "shuttle.txt");
            this.SetDelays();

#if ENABLE_FILE_WATCHDOG
            this._watchdog.AddFile(this._delaysFileName);
            this._watchdog.FileChanged += (s, e) => this.SetDelays();
            this._watchdog.StartWatching();
#endif
        }

#if ENABLE_FILE_WATCHDOG
        protected override void OnStop() => this._watchdog.StopWatching();
#endif
        private void SetDelays()
        {
            try
            {
                if (!System.IO.File.Exists(this._delaysFileName))
                {
                    SetDefaultDelays();
                    return;
                }

                var lines = System.IO.File.ReadAllLines(this._delaysFileName);
                if (lines.Length < 2)
                {
                    Tracer.Warning($"Not enough lines in shuttle delays file '{this._delaysFileName}'");
                    SetDefaultDelays();
                    return;
                }

                if (!Int32.TryParse(lines[0], out this._delay1) || !Int32.TryParse(lines[1], out this._delay2))
                {
                    Tracer.Warning($"Invalid lines in shuttle delays file '{this._delaysFileName}': '{lines[0]}' and '{lines[1]}'");
                    SetDefaultDelays();
                    return;
                }

                Tracer.Trace($"New shuttle delays from file '{this._delaysFileName}': '{this._delay1}' and '{this._delay2}'");
            }
            catch (Exception ex)
            {
                Tracer.Warning(ex, $"Cannot read shuttle delays from file '{this._delaysFileName}'");
                SetDefaultDelays();
            }

            void SetDefaultDelays()
            {
                this._delay1 = 50;
                this._delay2 = 100;
                Tracer.Trace($"Default shuttle delays : '{this._delay1}' and '{this._delay2}'");
            }
        }
    }
}
