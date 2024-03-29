﻿namespace Loupedeck.CommandPostPlugin.Localisation
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
        /// A dictionary of programatically generated Commands (i.e. buttons).
        /// </summary>
        private Dictionary<String, String> CommandsFromWebSocket;

        /// <summary>
        /// A dictionary of Adjustments (i.e. knobs).
        /// </summary>
        private Dictionary<String, String> Adjustments;

        /// <summary>
        /// A dictionary of Adjustments (i.e. knobs) without Reset.
        /// </summary>
        private Dictionary<String, String> AdjustmentsWithoutReset;

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
            this.CommandsFromWebSocket = new Dictionary<String, String>();
            this.Adjustments = new Dictionary<String, String>();
            this.AdjustmentsWithoutReset = new Dictionary<String, String>();
            this.GeneralStrings = new Dictionary<String, Dictionary<String, String>>();
            this.DisplayNames = new Dictionary<String, Dictionary<String, String>>();
            this.GroupNames = new Dictionary<String, Dictionary<String, String>>();
            
            // Load Commands & Adjustments from JSON files:
            this.Commands = GetJSONData(null, "commands");
            this.CommandsFromWebSocket = GetJSONData(null, "commandsfromwebsocket");
            this.Adjustments = GetJSONData(null, "adjustments");
            this.AdjustmentsWithoutReset = GetJSONData(null, "adjustments-noreset");

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
            //var CurrentLanguageCode = this.plugin.GetLoupedeckLanguageCode();            
            //return CurrentLanguageCode;

            var CurrentLanguageCode = this.plugin.Localization.CurrentLanguage;

            //Console.WriteLine("[CP] Current Language Code: " + CurrentLanguageCode);

            var twoCharacterLanguageCode = CurrentLanguageCode.Substring(0, 2);

            //Console.WriteLine("[CP] Current Language Code (Two Characters): " + twoCharacterLanguageCode);
          
            return twoCharacterLanguageCode;
        }

        /// <summary>
        /// Gets a list of available Commands from the embedded JSON files.
        /// </summary>
        /// <returns>A dictionary containing the available Commands.</returns>
        public Dictionary<String, String> GetCommands() => this.Commands;

        /// <summary>
        /// Gets a list of available Commands From WebSocket from the embedded JSON files.
        /// </summary>
        /// <returns>A dictionary containing the available Commands.</returns>
        public Dictionary<String, String> GetCommandsFromWebSocket() => this.CommandsFromWebSocket;

        /// <summary>
        /// Gets a list of available Adjustments from the embedded JSON files.
        /// </summary>
        /// <returns>A dictionary containing the available Adjustments.</returns>
        public Dictionary<String, String> GetAdjustments() => this.Adjustments;

        /// <summary>
        /// Gets a list of available Adjustments without Reset from the embedded JSON files.
        /// </summary>
        /// <returns>A dictionary containing the available Adjustments without Reset.</returns>
        public Dictionary<String, String> GetAdjustmentsWithoutReset() => this.AdjustmentsWithoutReset;

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
            if (this.DisplayNames[LanguageCode].TryGetValue(ActionID, out var ActionIDResult))
            {
                return ActionIDResult;
            }

            // Second Attempt: Try English:
            if (this.DisplayNames["en"].TryGetValue(ActionID, out var EnglishResult))
            {
                return EnglishResult;
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
            if (this.GroupNames[LanguageCode].TryGetValue(GroupID, out var GroupIDResult))
            {
                return GroupIDResult;
            }

            // Second Attempt: Try English:
            if (this.GroupNames["en"].TryGetValue(GroupID, out var EnglishResult))
            {
                return EnglishResult;
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
            if (this.GeneralStrings[LanguageCode].TryGetValue(GeneralStringsID, out var GeneralStringsIDResult))
            {
                return GeneralStringsIDResult;
            }

            // Second Attempt: Try English:
            if (this.GeneralStrings["en"].TryGetValue(GeneralStringsID, out var EnglishResult))
            {
                return EnglishResult;
            }

            // Third Attempt: Return the ID and write an error message to Console:
            Console.WriteLine("[CP] ERROR: Missing General String with ID: '" + GeneralStringsID + "'.");
            return GeneralStringsID;
        }
    }
}
