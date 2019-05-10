using System;
using System.ComponentModel;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.Playback;

namespace URY.BAPS.Common.Model.ServerConfig
{
    /// <summary>
    ///     String constants corresponding to choice descriptions used in the BapsNet
    ///     config system.
    /// </summary>
    public static class ChoiceKeys
    {
        public const string Yes = "Yes";
        public const string No = "No";
        public const string RepeatNone = "No repeat";
        public const string RepeatOne = "Repeat one";
        public const string RepeatAll = "Repeat all";

        /// <summary>
        ///     Converts a choice string to a Boolean.
        /// </summary>
        /// <param name="choice">
        ///     The choice to convert to a Boolean value.
        /// </param>
        /// <param name="fallback">
        ///     The value to return if <paramref name="choice" /> is neither <see cref="Yes" /> nor
        ///     <see cref="No" />.
        /// </param>
        /// <returns>
        ///     The Boolean equivalent of <paramref name="choice" />, or <paramref name="fallback" /> if the choice
        ///     doesn't correspond to a Boolean value.
        /// </returns>
        public static bool ChoiceToBoolean(string? choice, bool fallback)
        {
            return choice switch {
                Yes => true,
                No => false,
                _ => fallback
                };
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

        /// <summary>
        ///     Converts a string config choice key to a repeat mode.
        /// </summary>
        /// <param name="key">The choice key to convert.</param>
        /// <param name="fallback">The fallback mode to use if the choice key doesn't correspond to a repetition mode.</param>
        /// <returns>The corresponding mode, or <see cref="fallback" /> if <see cref="key" /> doesn't correspond to one.</returns>
        public static RepeatMode ChoiceToRepeatMode(string? key, RepeatMode fallback)
        {
            return key switch
                {
                RepeatNone => RepeatMode.None,
                RepeatOne => RepeatMode.One,
                RepeatAll => RepeatMode.All,
                _ => fallback
                };
        }

        /// <summary>
        ///     Converts a repeat mode enum to a string config choice key.
        /// </summary>
        /// <param name="mode">The mode to convert.</param>
        /// <returns>The corresponding choice key.</returns>
        public static string ToChoiceKey(this RepeatMode mode)
        {
            if (!Enum.IsDefined(typeof(RepeatMode), mode))
                throw new InvalidEnumArgumentException(nameof(mode), (int) mode, typeof(RepeatMode));
            switch (mode)
            {
                case RepeatMode.None:
                    return RepeatNone;
                case RepeatMode.One:
                    return RepeatOne;
                case RepeatMode.All:
                    return RepeatAll;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode,
                        "Internal error: this should be unreachable");
            }
        }
    }
}