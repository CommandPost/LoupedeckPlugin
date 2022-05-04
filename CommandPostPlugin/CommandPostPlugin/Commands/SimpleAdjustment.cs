namespace Loupedeck.CommandPostPlugin
{
    using System;

    class CounterAdjustment : PluginDynamicAdjustment
    {
        private Int32 _counter = 0;

        public CounterAdjustment() : base("Simple Adjustment", "Triggers a simple adjustment", "API Tests", true)
        {
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 ticks)
        {
            //
            // This is what's triggered when you turn a knob for the "Simple Adjustment"
            //
            this._counter += ticks; // increase or decrease counter on the number of ticks
            this.ActionImageChanged(actionParameter);
        }

        protected override void RunCommand(String actionParameter)
        {
            //
            // This is what's triggered when press a button for the "Simple Adjustment"
            //
            this._counter = 0; // reset counter to 0
            this.ActionImageChanged(actionParameter);
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            //
            // Get the value of the adjustment:
            //
            return this._counter.ToString();
        }
    }
}