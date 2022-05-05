namespace Loupedeck.CommandPostPlugin.Commands
{
    using System;

    public class ToggleSwitchButton : PluginDynamicCommand
    {
        private Boolean _toggleState = false;

        private readonly String _toggleSwitchButtonOff;
        private readonly String _toggleSwitchButtonOn;

        public ToggleSwitchButton() : base("Toggle Switch", "Triggers a toggle switch", "API Tests")
        {
            this._toggleSwitchButtonOff = EmbeddedResources.FindFile("ToggleSwitchButtonOff.png");
            this._toggleSwitchButtonOn = EmbeddedResources.FindFile("ToggleSwitchButtonOn.png");
        }

        protected override void RunCommand(String actionParameter)
        {
            this._toggleState = !this._toggleState;
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (this._toggleState)
            {
                return EmbeddedResources.ReadImage(this._toggleSwitchButtonOff);
            }
            else
            {
                return EmbeddedResources.ReadImage(this._toggleSwitchButtonOn);
            }
        }
    }
}