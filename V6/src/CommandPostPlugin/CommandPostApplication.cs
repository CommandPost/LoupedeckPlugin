namespace Loupedeck.CommandPostPlugin
{
    using System;

    /// <summary>
    /// The Client Application Class. This is not used by the CommandPost Plugin.
    /// </summary>
    public class CommandPostApplication : ClientApplication
    {
        /// <summary>
        /// The Client Application Class. This is not used by the CommandPost Plugin.
        /// </summary>
        public CommandPostApplication()
        {

        }

        /// <summary>
        /// The Process Name. This is not used by the CommandPost Plugin.
        /// </summary>
        /// <returns>The Process Name.</returns>
        protected override String GetProcessName() => "";

        /// <summary>
        /// The Bundle Idenitfier. This is not used by the CommandPost Plugin.
        /// </summary>
        /// <returns>The Bundle Identifier.</returns>
        protected override String GetBundleName() => "";
    }
}