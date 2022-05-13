namespace Loupedeck.CommandPostPlugin.Localisation
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Collections.Generic;

    public class CPLocalisation
    {
        private Dictionary<String, String> Commands;
        private Dictionary<String, String> Adjustments;
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
            this.DisplayNames["de"] = GetJSONData("de", "displaynames");
            this.DisplayNames["en"] = GetJSONData("en", "displaynames");
            this.DisplayNames["fr"] = GetJSONData("fr", "displaynames");
            this.DisplayNames["ja"] = GetJSONData("ja", "displaynames");
            this.DisplayNames["ko"] = GetJSONData("ko", "displaynames");

            //
            // Load GroupNames JSON files:
            //
            this.GroupNames["de"] = GetJSONData("de", "groupnames");
            this.GroupNames["en"] = GetJSONData("en", "groupnames");
            this.GroupNames["fr"] = GetJSONData("fr", "groupnames");
            this.GroupNames["ja"] = GetJSONData("ja", "groupnames");
            this.GroupNames["ko"] = GetJSONData("ko", "groupnames");
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
    }
}
