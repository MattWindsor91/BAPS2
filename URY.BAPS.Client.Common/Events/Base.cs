using System;
using URY.BAPS.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common.Events
{
    /// <summary>
    ///     Abstract base class of event payloads that derive from
    ///     BapsNet server messages.
    /// </summary>
    public abstract class ArgsBase : EventArgs
    {
    }

    public abstract class ChannelArgsBase : ArgsBase
    {
        protected ChannelArgsBase(ushort channelId)
        {
            ChannelId = channelId;
        }

        public ushort ChannelId { get; }
    }

    /// <summary>
    ///     Abstract base class of event payloads that reference
    ///     indices in channel track lists.
    /// </summary>
    public abstract class TrackIndexArgsBase : ChannelArgsBase
    {
        protected TrackIndexArgsBase(ushort channelId, uint index) : base(channelId)
        {
            Index = index;
        }

        /// <summary>
        ///     The track-list index being mentioned in the event.
        /// </summary>
        public uint Index { get; }
    }

    public class ErrorEventArgs : ArgsBase
    {
        public ErrorType Type { get; }
        public byte Code { get; }
        public string Description { get; }

        public ErrorEventArgs(ErrorType type, byte code, string description)
        {
            Type = type;
            Code = code;
            Description = description;
        }
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

    public class UnknownCommandArgs : ArgsBase
    {
        public CommandWord PackedCommand { get; }
        public string Description { get; }

        public UnknownCommandArgs(CommandWord packedCommand, string description)
        {
            PackedCommand = packedCommand;
            Description = description;
        }
    }
}