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
        public System.Collections.Hashtable valueList;
        public System.Collections.Hashtable choiceList;
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
            descLookup = new System.Collections.Hashtable();
            idLookup = new System.Collections.Hashtable();
        }
        /** Nothing to destroy really **/
        public static void closeConfigCache()
        {
        }
        /** add an option **/
        public static void addOptionDescription(int optionid, int type, string description, bool isIndexed)
        {
            if (descLookup[description] != null) return;
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
                oci.valueList = new System.Collections.Hashtable();
            }
            if (oci.type == ConfigType.CHOICE)
            {
                /** for choice based options we need a hashtable for the choices **/
                oci.choiceList = new System.Collections.Hashtable();
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
            /** Make sure we know about the option this data refers to **/
            if (idLookup[optionid] != null)
            {
                /** enter the setting in the hashtable **/
                ((OptionCacheInfo)idLookup[optionid]).choiceList[description] = choiceid;
            }
        }
        public static int findChoiceIndexFor(string optionDesc, string description)
        {
            if (descLookup[optionDesc] != null)
            {
                /** fetch the id from the choice hashtable **/
                return (int)((OptionCacheInfo)descLookup[optionDesc]).choiceList[description];
            }
            else
            {
                return -1;
            }
        }
        public static void addOptionValue(int optionid, int index, string value)
        {
            /** Make sure we know about the option this data refers to **/
            if (idLookup[optionid] != null)
            {
                if (index == -1)
                {
                    /** Non indexed setting **/
                    ((OptionCacheInfo)idLookup[optionid]).strValue = value;
                }
                else
                {
                    /** enter the setting in the hashtable **/
                    ((OptionCacheInfo)idLookup[optionid]).valueList[index] = value;
                }
            }
        }

        public static void addOptionValue(int optionid, int index, int value)
        {
            /** Make sure we know azbout the option this data refers to **/
            if (idLookup[optionid] != null)
            {
                if (index == -1)
                {
                    /** Non indexed setting **/
                    ((OptionCacheInfo)idLookup[optionid]).intValue = value;
                }
                else
                {
                    /** enter the setting in the hashtable **/
                    ((OptionCacheInfo)idLookup[optionid]).valueList[index] = value;
                }
            }
        }
        public static OptionCacheInfo getOption(string optionDescription)
        {
            /** Gets an int from the cache **/
            if (descLookup[optionDescription] != null)
            {
                return (OptionCacheInfo)descLookup[optionDescription];
            }
            else
            {
                return null;
            }
        }
        public static int getValueInt(string optionDescription)
        {
            /** Gets an int from the cache **/
            if (descLookup[optionDescription] != null)
            {
                return ((OptionCacheInfo)descLookup[optionDescription]).intValue;
            }
            else
            {
                return 0;
            }
        }
        public static int getValueInt(string optionDescription, int index)
        {
            /** Gets an int from the cache **/
            if (descLookup[optionDescription] != null)
            {
                return (int)((OptionCacheInfo) descLookup[optionDescription]).valueList[index];
            }
            else
            {
                return 0;
            }
        }
        public static string getValueStr(string optionDescription)
        {
            /** Gets an int from the cache **/
            if (descLookup[optionDescription] != null)
            {
                return ((OptionCacheInfo)descLookup[optionDescription]).strValue;
            }
            else
            {
                return "";
            }
        }
        public static string getValueStr(string optionDescription, int index)
        {
            /** Gets an int from the cache **/
            if (descLookup[optionDescription] != null)
            {
                return (string)((OptionCacheInfo)descLookup[optionDescription]).valueList[index];
            }
            else
            {
                return "";
            }
        }

        private static System.Collections.Hashtable descLookup;
	    private static System.Collections.Hashtable idLookup;
    }
}
