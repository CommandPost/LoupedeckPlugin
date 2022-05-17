namespace Loupedeck.CommandPostPlugin.Localisation
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Collections.Generic;

    public class CPLocalisation
    {
        /// <summary>
        /// A string array of all the supported language codes by the LoupedeckConfig application. This should be manually updated when Loupedeck add more languages.
        /// </summary>
        private readonly String[] SupportedLanguageCodes = { "de", "en", "fr", "ja", "ko" };

        /// <summary>
        /// A "link" back to the main Loupedeck Plugin.
        /// </summary>
        private readonly CommandPostPlugin plugin;

        /// <summary>
        /// A dictionary of Commands (i.e. buttons).
        /// </summary>
        private Dictionary<String, String> Commands;

        /// <summary>
        /// A dictionary of Adjustments (i.e. knobs).
        /// </summary>
        private Dictionary<String, String> Adjustments;

        /// <summary>
        /// A dictionary of General Strings (i.e. error messages).
        /// </summary>
        private Dictionary<String, Dictionary<String, String>> GeneralStrings;

        /// <summary>
        /// A dictionary of Display Names.
        /// </summary>
        private Dictionary<String, Dictionary<String, String>> DisplayNames;

        /// <summary>
        /// A dictionary of Group Names.
        /// </summary>
        private Dictionary<String, Dictionary<String, String>> GroupNames;
              
        /// <summary>
        /// A support class for reading and processing the translation data contained within JSON files.
        /// </summary>
        /// <param name="thePlugin">The main Loupedeck Plugin Class.</param>
        public CPLocalisation(CommandPostPlugin thePlugin)
        {
            // Create a "link" to the main Plugin:
            this.plugin = thePlugin;

            // Write the Current Language Code to Console for Debug Purposes:
            // Console.WriteLine("[CP] Current Language Code: " + this.GetCurrentLanguageCode());

            // Read JSON Files:
            this.ReadJSONFiles();
        }
       
        /// <summary>
        /// Reads the JSON Files from Embedded Resources.
        /// </summary>
        private void ReadJSONFiles()
        {
            // Create Dictionaries:
            this.Commands = new Dictionary<String, String>();
            this.Adjustments = new Dictionary<String, String>();
            this.GeneralStrings = new Dictionary<String, Dictionary<String, String>>();
            this.DisplayNames = new Dictionary<String, Dictionary<String, String>>();
            this.GroupNames = new Dictionary<String, Dictionary<String, String>>();

            // Load Commands & Adjustments from JSON files:
            this.Commands = GetJSONData(null, "commands");
            this.Adjustments = GetJSONData(null, "adjustments");

            // Load DisplayNames JSON files:
            foreach (var Code in this.SupportedLanguageCodes)
            {
                this.DisplayNames[Code] = GetJSONData(Code, "displaynames");
            }

            // Load GroupNames JSON files:
            foreach (var Code in this.SupportedLanguageCodes)
            {
                this.GroupNames[Code] = GetJSONData(Code, "groupnames");
            }

            // Load General Strings from JSON files:
            foreach (var Code in this.SupportedLanguageCodes)
            {
                this.GeneralStrings[Code] = GetJSONData(Code, "general");
            }
        }
        
        /// <summary>
        /// Reads JSON data from an embedded resource.
        /// </summary>
        /// <param name="LanguageCode">A two character language code (i.e. "en")</param>
        /// <param name="FileName">The filename of the JSON file (i.e. "general")</param>
        /// <returns>A dictionary with the JSON data. If the resource file cannot be found, it'll return an empty dictionary instead and write an error to the Console.</returns>
        private static Dictionary<String, String> GetJSONData(String LanguageCode, String FileName)
        {
            var TheFileName = LanguageCode == null ? FileName : FileName + "-" + LanguageCode;
            var dataStream = EmbeddedResources.GetStream(EmbeddedResources.FindFile(TheFileName + ".json"));

            // Check that the JSON data actually exists:
            if (dataStream.Length > 0) {
                var reader = new StreamReader(dataStream, Encoding.UTF8);
                Dictionary<String, String> result = JsonSerializer.Deserialize<Dictionary<String, String>>(reader.ReadToEnd());
                return result;
            } else {
                Console.WriteLine("[CP] ERROR: Failed to load resource: '" + TheFileName + "'");
                var emptyDictionary = new Dictionary<String, String>();
                return emptyDictionary;
            }
        }

        /// <summary>
        /// Gets the current Language Code used by the LoupedeckConfig application.
        /// </summary>
        /// <returns>The language code as a two character string.</returns>
        public String GetCurrentLanguageCode() {
            var CurrentLanguageCode = this.plugin.GetLoupedeckLanguageCode();            
            return CurrentLanguageCode;
        }

        /// <summary>
        /// Gets a list of available Commands from the embedded JSON files.
        /// </summary>
        /// <returns>A dictionary containing the available Commands.</returns>
        public Dictionary<String, String> GetCommands()
        {
            return this.Commands;
        }

        /// <summary>
        /// Gets a list of available Adjustments from the embedded JSON files.
        /// </summary>
        /// <returns>A dictionary containing the available Adjustments.</returns>
        public Dictionary<String, String> GetAdjustments() {
            return this.Adjustments;
        }

        /// <summary>
        /// Gets a Display Name from the JSON data.
        /// </summary>
        /// <param name="ActionID">The ActionID as a string.</param>
        /// <returns>The display name as a string. First it will try find it in the current language, then try English, and failing that will simply return the supplied ActionID  (whilst writing an error in the Console for developers).</returns>
        public String GetDisplayName(String ActionID)
        {
            // Get the current language code:
            var LanguageCode = this.GetCurrentLanguageCode();

            // First Attempt: Try the current language:
            if (this.DisplayNames[LanguageCode].ContainsKey(ActionID))
            {
                return this.DisplayNames[LanguageCode][ActionID];
            }

            // Second Attempt: Try English:
            if (this.DisplayNames["en"].ContainsKey(ActionID))
            {
                return this.DisplayNames["en"][ActionID];
            }

            // Third Attempt: Return the ID and write an error message to Console:
            Console.WriteLine("[CP] ERROR: Missing DisplayName with ActionID: '" + ActionID + "'.");
            return ActionID;
        }

        /// <summary>
        /// Gets the Group Name from the JSON data.
        /// </summary>
        /// <param name="GroupID">The GroupID as a string.</param>
        /// <returns>The GroupName as a string. First it will try find it in the current language, then try English, and failing that will simply return the supplied GroupID (whilst writing an error in the Console for developers).</returns>
        public String GetGroupName(String GroupID)
        {
            // Get the current language code:            
            var LanguageCode = this.GetCurrentLanguageCode();

            // First Attempt: Try the current language:
            if (this.GroupNames[LanguageCode].ContainsKey(GroupID))
            {
                return this.GroupNames[LanguageCode][GroupID];
            }

            // Second Attempt: Try English:
            if (this.GroupNames["en"].ContainsKey(GroupID))
            {
                return this.GroupNames["en"][GroupID];
            }

            // Third Attempt: Return the ID and write an error message to Console:
            Console.WriteLine("[CP] ERROR: Missing GroupName with GroupID: '" + GroupID + "'.");
            return GroupID;
        }
       
        /// <summary>
        /// Gets a general string from the embedded JSON files.
        /// </summary>
        /// <param name="GeneralStringsID">The GeneralStringsID as a string.</param>
        /// <returns>The General String as a string. First it will try find it in the current language, then try English, and failing that will simply return the supplied GeneralStringsID (whilst writing an error in the Console for developers).</returns>
        public String GetGeneralString(String GeneralStringsID)
        {
            // Get the current language code:            
            var LanguageCode = this.GetCurrentLanguageCode();

            // First Attempt: Try the current language:
            if (this.GeneralStrings[LanguageCode].ContainsKey(GeneralStringsID))
            {
                return this.GeneralStrings[LanguageCode][GeneralStringsID];
            }

            // Second Attempt: Try English:
            if (this.GeneralStrings["en"].ContainsKey(GeneralStringsID))
            {
                return this.GeneralStrings["en"][GeneralStringsID];
            }

            // Third Attempt: Return the ID and write an error message to Console:
            Console.WriteLine("[CP] ERROR: Missing General String with ID: '" + GeneralStringsID + "'.");
            return GeneralStringsID;
        }
    }
}
