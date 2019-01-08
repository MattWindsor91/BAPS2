using System.Collections.Generic;
using System.Linq;

namespace BAPSCommon
{
    /**
    This stores all the info there is about a given option in hashtable
    **/
    public class OptionCacheInfo
    {
        public int optionid;
        public ConfigType type;
        public bool isIndexed;
        public string description;
        public int intValue;
        public string strValue;
        public Dictionary<int, object> valueList;
        public Dictionary<string, int> choiceList;

        public int? IntValueAt(int index) =>
            valueList.TryGetValue(index, out var x) ? x as int? : null;

        public string StrValueAt(int index) =>
            valueList.TryGetValue(index, out var x) ? x as string : null;
    };

    /** The config cache is designed as a quick access to config variables,
        You tell it what config item is expected next and then ask for that
        option from the server. It will then associate the result with the
        name you asked for.
    **/
    public class ConfigCache
    {
        /** This is a static class so initialize all the static members **/
        public static void initConfigCache()
        {
            descLookup = new Dictionary<string, OptionCacheInfo>();
            idLookup = new Dictionary<int, OptionCacheInfo>();
        }
        /** Nothing to destroy really **/
        public static void closeConfigCache()
        {
        }
        /** add an option **/
        public static void addOptionDescription(int optionid, int type, string description, bool isIndexed)
        {
            if (descLookup.ContainsKey(description)) return;

            /** Make a new option info object **/
            var oci = new OptionCacheInfo
            {
                /** Set all the info **/
                optionid = optionid,
                type = (ConfigType)type,
                description = description,
                isIndexed = isIndexed
            };
            if (isIndexed)
            {
                /** for indexed options we need a hashtable of values **/
                oci.valueList = new Dictionary<int, object>();
            }
            if (oci.type == ConfigType.CHOICE)
            {
                /** for choice based options we need a hashtable for the choices **/
                oci.choiceList = new Dictionary<string, int>();
            }
            /** place the option in the description hashtable **/
            descLookup[description] = oci;
            /** also place it in the optionid indexed hashtable so the 
                results can be added without hassle
            **/
            idLookup[optionid] = oci;
        }
        public static void addOptionChoice(int optionid, int choiceid, string description)
        {
            if (!idLookup.TryGetValue(optionid, out var option)) return;
            option.choiceList[description] = choiceid;
        }
        public static int findChoiceIndexFor(string optionDesc, string description) =>
            descLookup.TryGetValue(optionDesc, out var x) ? x.choiceList[description] : -1;

        public static void addOptionValue(int optionid, int index, string value)
        {
            if (!idLookup.TryGetValue(optionid, out var option)) return;

            if (index == -1)
            {
                option.strValue = value;
            }
            else
            {
                option.valueList[index] = value;
            }
        }

        public static void addOptionValue(int optionid, int index, int value)
        {
            if (!idLookup.TryGetValue(optionid, out var option)) return;

            if (index == -1)
            {
                option.intValue = value;
            }
            else
            {
                option.valueList[index] = value;
            }
        }
        public static OptionCacheInfo getOption(string optionDescription) =>
            descLookup.TryGetValue(optionDescription, out var x) ? x : null;

        public static int getValueInt(string optionDescription) =>
            getOption(optionDescription)?.intValue ?? 0;

        public static int getValueInt(string optionDescription, int index) =>            
            getOption(optionDescription)?.IntValueAt(index) ?? 0;

        public static string getValueStr(string optionDescription) =>
            getOption(optionDescription)?.strValue ?? "";

        public static string getValueStr(string optionDescription, int index) =>
            getOption(optionDescription)?.StrValueAt(index) ?? "";

        private static Dictionary<string, OptionCacheInfo> descLookup;
	    private static Dictionary<int, OptionCacheInfo> idLookup;
    }
}
