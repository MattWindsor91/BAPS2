using System;

namespace BAPSCommon
{
    /// <summary>
    /// Types of channel config change.
    /// </summary>
    public enum ChannelConfigChangeType : byte
    {
        // NB: These bit patterns are arbitrary, and don't correspond to anything in
        // BAPSNet.  The main goal is that each valid change type has a unique
        // bit pattern.

        // Categories
        AutoAdvance = 0b001_00000,
        PlayOnLoad  = 0b010_00000,
        Repeat      = 0b100_00000,
        // Values for AutoAdvance and PlayOnLoad
        Off         = 0b000_00001,
        On          = 0b000_00010,
        // Values for Repeat
        None        = 0b000_00100,
        One         = 0b000_01000,
        All         = 0b000_10000,

        // Shorthand for all the different valid values
        AutoAdvanceOn  = AutoAdvance | On,
        AutoAdvanceOff = AutoAdvance | Off,
        PlayOnLoadOn   = PlayOnLoad  | On,
        PlayOnLoadOff  = PlayOnLoad  | Off,
        RepeatNone     = Repeat      | None,
        RepeatOne      = Repeat      | One,
        RepeatAll      = Repeat      | All
    }

    /// <summary>
    /// Argument class for sending events on channel config changes.
    /// </summary>
    public class ChannelConfigChangeArgs : EventArgs
    {
        public ushort ChannelId { get; }
        public ChannelConfigChangeType Type { get; }

        public ChannelConfigChangeArgs(ushort channelId, ChannelConfigChangeType type)
        {
            ChannelId = channelId;
            Type = type;
        }
    }

    /// <summary>
    /// Event handler delegate for channel config changes.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The arguments of the event.</param>
    public delegate void ChannelConfigChangeHandler(object sender, ChannelConfigChangeArgs e);
}
