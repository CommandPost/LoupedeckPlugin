namespace Loupedeck.CommandPostPlugin
{
    using System;

    /// <summary>
    /// The Client Application Class.
    /// </summary>
    public class CommandPostApplication : ClientApplication
    {
        /// <summary>
        /// The Client Application Class.
        /// </summary>
        public CommandPostApplication()
        {}

        /// <summary>
        /// Gest the Display Name of the Client Application.
        /// </summary>
        /// <returns>The display name as a string</returns>
        public override String GetDisplayName() => "Final Cut Pro";

        /// <summary>
        /// Gets the Process Name of the Client Application.
        /// </summary>
        /// <returns>The Process Name as a string.</returns>
        protected override String GetProcessName() => "Final Cut Pro";

        /// <summary>
        /// The the Bundle Identifier(s) of the Client Application.
        /// </summary>
        /// <returns>An array of Bundle Identifiers.</returns>
        protected override String[] GetBundleNames() => new[] { "com.apple.FinalCut", "com.apple.FinalCutTrial" };
    }
}