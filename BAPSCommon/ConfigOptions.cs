// This reflects Common/ConfigDialog.h.
// It *should* be kept in sync with it until one can be deprecated in favour of the other.

namespace BAPSCommon
{
    // The options rendered in the config menu depend on this ordering.
    // Afaik these can be reordered to change the order they appear in the config menu.
    public enum ConfigOption
    {
        CHANNELCOUNT,
        DEVICE,
        CHANNELNAME,
        AUTOADVANCE,
        AUTOPLAY,
        REPEAT,
        DIRECTORYCOUNT,
        NICEDIRECTORYNAME,
        DIRECTORYLOCATION,
        SERVERID,
        PORT,
        MAXQUEUECONNS,
        CLIENTCONNLIMIT,
        DBSERVER,
        DBPORT,
        LIBRARYDBNAME,
        BAPSDBNAME,
        DBUSERNAME,
        DBPASSWORD,
        LIBRARYLOCATION,
        CLEANMUSICONLY,
        SAVEINTROPOSITIONS,
        STOREPLAYBACKEVENTS,
        LOGNAME,
        LOGSOURCE,
        SUPPORTADDRESS,
        SMTPSERVER,
        BAPSCONTROLLERENABLED,
        BAPSCONTROLLERPORT,
        BAPSCONTROLLERBUTTONCOUNT,
        BAPSCONTROLLERBUTTONCODE,
        BAPSPADDLEMODE,
        BAPSCONTROLLER2ENABLED,
        BAPSCONTROLLER2DEVICECOUNT,
        BAPSCONTROLLER2SERIAL,
        BAPSCONTROLLER2OFFSET,
        BAPSCONTROLLER2BUTTONCOUNT,
        BAPSCONTROLLER2BUTTONCODE,
        LASTOPTION
    };

    public enum ConfigType
    {
        INT = 0,
        STR = 1,
        CHOICE = 2
    };

    public enum ConfigResult
    {
        SUCCESS,
        BADTYPE,
        BADOPTION,
        INDEXINGERROR,
        INDEXOUTOFRANGE,
        VALIDATIONERROR,
        NOPERMISSION
    };

    public static class ConfigResultText
    {
        public static string[] text =
        {"Success",
         "Incorrect type for option",
         "Option does not exist",
         "Incorrect indexing for option",
         "Specified index exceeds limit",
         "Setting failed validation check",
         "You do not have permission"};
    }
};