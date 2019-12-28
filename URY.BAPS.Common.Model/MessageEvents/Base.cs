using System;

namespace URY.BAPS.Common.Model.MessageEvents
{
    /// <summary>
    ///     Abstract base class of event payloads that derive from
    ///     BapsNet server messages.
    /// </summary>
    public abstract class MessageArgsBase : EventArgs
    {
    }

    public abstract class ChannelArgsBase : MessageArgsBase
    {
        protected ChannelArgsBase(ushort channelId)
        {
            ChannelId = channelId;
        }

        public ushort ChannelId { get; }
    }

    public class ErrorEventArgs : MessageArgsBase
    {
        public ErrorEventArgs(ErrorType type, byte code, string description)
        {
            Type = type;
            Code = code;
            Description = description;
        }

        public ErrorType Type { get; }
        public byte Code { get; }
        public string Description { get; }
    }

    /// <summary>
    ///     Enumeration of different errors a BAPS server can send.
    /// </summary>
    public enum ErrorType
    {
        Library,
        BapsDb,
        Config
    }

    public class UnknownCommandArgs : MessageArgsBase
    {
        public UnknownCommandArgs(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
    
    /// <summary>
    ///     Abstract base class of event payloads that reference
    ///     indices in channel track lists.
    /// </summary>
    public abstract class TrackIndexArgsBase : ChannelArgsBase
    {
        protected TrackIndexArgsBase(TrackIndex index) : base(index.ChannelId)
        {
            Index = index;
        }

        /// <summary>
        ///     The track-list index being mentioned in the event.
        /// </summary>
        public TrackIndex Index { get; }
    }
}