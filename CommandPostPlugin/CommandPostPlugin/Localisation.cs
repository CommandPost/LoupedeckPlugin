namespace Loupedeck.CommandPostPlugin.Localisation
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Collections.Generic;

    public class CPLocalisation
    {        
        public CPLocalisation()
        {        
        }

        /// 
        /// Get JSON Data:
        /// 
        private static Dictionary<String, String> GetJSONData(String LanguageCode, String FileName)
        {
            //
            // Read Group Names JSON:
            //
            var TheFileName = LanguageCode == null ? FileName : FileName + "-" + LanguageCode;
            var dataStream = EmbeddedResources.GetStream(EmbeddedResources.FindFile(TheFileName + ".json"));
            var reader = new StreamReader(dataStream, Encoding.UTF8);
            Dictionary<String, String> result = JsonSerializer.Deserialize<Dictionary<String, String>>(reader.ReadToEnd());
            return result;
        }

        //
        // Get Display Name:
        //
        public String GetDisplayName(String ActionID, String LanguageCode)
        {
            var DisplayNames = GetJSONData(LanguageCode, "displaynames");
            return DisplayNames[ActionID];
        }

        //
        // Get Group Name:
        //
        public String GetGroupName(String GroupID, String LanguageCode)
        {
            var GroupNames = GetJSONData(LanguageCode, "groupnames");
            return GroupNames[GroupID];
        }

        //
        /// Get Commands:
        /// 
        public Dictionary<String, String> GetCommands()
        {
            var Commands = GetJSONData(null, "commands");
            return Commands;
        }

        //
        /// Get Adjustments:
        /// 
        public Dictionary<String, String> GetAdjustments()
        {
            var Commands = GetJSONData(null, "adjustments");
            return Commands;
        }
    }
}
