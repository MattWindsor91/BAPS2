// This reflects Common/ConfigDialog.h.
// It *should* be kept in sync with it until one can be deprecated in favour of the other.

namespace BAPSClientCommon.ServerConfig
{
    public enum ConfigResult
    {
        Success,
        BadType,
        BadOption,
        IndexingError,
        IndexOutOfRange,
        ValidationError,
        NoPermission
    }

    public static class ConfigResultText
    {
        public static string[] Text =
        {
            "Success",
            "Incorrect type for option",
            "Option does not exist",
            "Incorrect indexing for option",
            "Specified index exceeds limit",
            "Setting failed validation check",
            "You do not have permission"
        };
    }
}