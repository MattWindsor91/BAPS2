// This reflects Common/ConfigDialog.h.
// It *should* be kept in sync with it until one can be deprecated in favour of the other.

namespace BAPSCommon
{
    /// <summary>
    /// String constants corresponding to descriptions used in the bapsnet
    /// config system.
    /// </summary>
    public static class ConfigDescriptions
    {
        public const string AutoAdvance = "Auto Advance";
        public const string PlayOnLoad = "Play on load";
        public const string Repeat = "Repeat";
    }

    /// <summary>
    /// String constants corresponding to choice descriptions used in the bapsnet
    /// config system.
    /// </summary>
    public static class ChoiceDescriptions
    {
        public const string Yes = "Yes";
        public const string No = "No";
        public const string RepeatNone = "None";
        public const string RepeatOne = "One";
        public const string RepeatAll = "All";
    }

    public enum ConfigType
    {
        Int = 0,
        Str = 1,
        Choice = 2
    }

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
        {"Success",
         "Incorrect type for option",
         "Option does not exist",
         "Incorrect indexing for option",
         "Specified index exceeds limit",
         "Setting failed validation check",
         "You do not have permission"};
    }
}