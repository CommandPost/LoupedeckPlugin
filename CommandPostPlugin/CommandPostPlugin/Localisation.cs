namespace Loupedeck.CommandPostPlugin.Localisation
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Collections.Generic;

    public class CPLocalisation
    {
        private readonly String[] SupportedLanguageCodes = { "de", "en", "fr", "ja", "ko" };

        private Dictionary<String, String> Commands;
        private Dictionary<String, String> Adjustments;
        private Dictionary<String, Dictionary<String, String>> GeneralStrings;
        private Dictionary<String, Dictionary<String, String>> DisplayNames;
        private Dictionary<String, Dictionary<String, String>> GroupNames;
        


        private CommandPostPlugin plugin;

        public CPLocalisation(CommandPostPlugin thePlugin)
        {
            //
            // Create a "link" to the main Plugin:
            //
            this.plugin = thePlugin;

            //
            // Read JSON Files:
            //
            this.ReadJSONFiles();
        }

        //
        // Read JSON Files from Embedded Resources:
        //
        private void ReadJSONFiles()
        {
            //
            // Create Dictionaries:
            //
            this.Commands = new Dictionary<String, String>();
            this.Adjustments = new Dictionary<String, String>();
            this.GeneralStrings = new Dictionary<String, Dictionary<String, String>>();
            this.DisplayNames = new Dictionary<String, Dictionary<String, String>>();
            this.GroupNames = new Dictionary<String, Dictionary<String, String>>();

            //
            // Load Commands & Adjustments from JSON files:
            //
            this.Commands = GetJSONData(null, "commands");
            this.Adjustments = GetJSONData(null, "adjustments");

            //
            // Load DisplayNames JSON files:
            //
            foreach (var Code in this.SupportedLanguageCodes)
            {
                this.DisplayNames[Code] = GetJSONData(Code, "displaynames");
            }

            //
            // Load GroupNames JSON files:
            //
            foreach (var Code in this.SupportedLanguageCodes)
            {
                this.GroupNames[Code] = GetJSONData(Code, "groupnames");
            }

            //
            // Load General Strings from JSON files:
            //
            foreach (var Code in this.SupportedLanguageCodes)
            {
                this.GeneralStrings[Code] = GetJSONData(Code, "general");
            }
        }

        // 
        // Get JSON Data:
        // 
        private static Dictionary<String, String> GetJSONData(String LanguageCode, String FileName)
        {
            var TheFileName = LanguageCode == null ? FileName : FileName + "-" + LanguageCode;
            var dataStream = EmbeddedResources.GetStream(EmbeddedResources.FindFile(TheFileName + ".json"));
            var reader = new StreamReader(dataStream, Encoding.UTF8);
            Dictionary<String, String> result = JsonSerializer.Deserialize<Dictionary<String, String>>(reader.ReadToEnd());
            return result;
        }

        //
        // Get Current Language Code:
        //         
        public String GetCurrentLanguageCode() {
            var CurrentLanguageCode = this.plugin.GetLoupedeckLanguageCode();
            Console.WriteLine("[CP] Current Language Code: " + CurrentLanguageCode);
            return CurrentLanguageCode;
        }

        //
        // Get Display Name:
        //
        public String GetDisplayName(String ActionID)
        {
            var LanguageCode = this.GetCurrentLanguageCode();
            return this.DisplayNames[LanguageCode].ContainsKey(ActionID)
                    ? this.DisplayNames[LanguageCode][ActionID]
                    : this.DisplayNames["en"][ActionID];
        }

        //
        // Get Group Name:
        //
        public String GetGroupName(String GroupID)
        {
            var LanguageCode = this.GetCurrentLanguageCode();
            return this.GroupNames[LanguageCode].ContainsKey(GroupID)
                ? this.GroupNames[LanguageCode][GroupID]
                : this.GroupNames["en"][GroupID];
        }

        //
        // Get Commands:
        // 
        public Dictionary<String, String> GetCommands() => this.Commands;

        //
        // Get Adjustments:
        // 
        public Dictionary<String, String> GetAdjustments() => this.Adjustments;

        //
        // Get General String:
        //
        public String GetGeneralString(String GeneralStringsID)
        {
            var LanguageCode = this.GetCurrentLanguageCode();
            return this.GeneralStrings[LanguageCode].ContainsKey(GeneralStringsID)
                    ? this.GeneralStrings[LanguageCode][GeneralStringsID]
                    : this.GeneralStrings["en"][GeneralStringsID];
        }
    }
}
