using JetBrains.Annotations;

namespace BAPSClientCommon.ServerConfig
{
    /// <summary>
    ///     String constants corresponding to choice descriptions used in the bapsnet
    ///     config system.
    /// </summary>
    public static class ChoiceKeys
    {
        public const string Yes = "Yes";
        public const string No = "No";
        public const string RepeatNone = "None";
        public const string RepeatOne = "One";
        public const string RepeatAll = "All";

        /// <summary>
        ///     Converts a choice string to a Boolean.
        /// </summary>
        /// <param name="choice">
        ///     The choice to convert to a Boolean value.
        /// </param>
        /// <param name="fallback">
        ///     The value to return if <paramref name="choice" /> is neither <see cref="ChoiceKeys.Yes" /> nor
        ///     <see cref="ChoiceKeys.No" />.
        /// </param>
        /// <returns>
        ///     The Boolean equivalent of <paramref name="choice" />, or <paramref name="fallback" /> if the choice
        ///     doesn't correspond to a Boolean value.
        /// </returns>
        public static bool ChoiceToBoolean([CanBeNull] string choice, bool fallback)
        {
            switch (choice)
            {
                case Yes:
                    return true;
                case No:
                    return false;
                default:
                    return fallback;
            }
        }

        /// <summary>
        ///     Converts a Boolean to a BapsNet choice string.
        /// </summary>
        /// <param name="value">The Boolean to convert.</param>
        /// <returns><see cref="Yes" /> if <paramref name="value" /> is true; else <see cref="No" />.</returns>
        [NotNull]
        public static string BooleanToChoice(bool value)
        {
            return value ? Yes : No;
        }
    }
}