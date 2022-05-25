namespace Loupedeck.CommandPostPlugin
{
    using System;
    using System.Collections.Generic;

    class PluginDynamicAdjustmentWithParametersSetInOnLoad : PluginDynamicAdjustment
    {
        private readonly Dictionary<String, Int32> _values = new Dictionary<String, Int32>();

        public PluginDynamicAdjustmentWithParametersSetInOnLoad() : base(false)
        {
        }

        protected override Boolean OnLoad()
        {
            AddParameter("OnLoad R");
            AddParameter("OnLoad G");
            AddParameter("OnLoad B");

            void AddParameter(String name)
            {
                this.AddParameter(name, name, "Test");
                this._values[name] = 128;
            }

            return true;
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            var value = this._values[actionParameter] + diff;
            this._values[actionParameter] = Helpers.MinMax(value, 0, 255);

            this.AdjustmentValueChanged(actionParameter);
        }

        protected override String GetAdjustmentValue(String actionParameter) => $"{this._values[actionParameter]}";
    }
}