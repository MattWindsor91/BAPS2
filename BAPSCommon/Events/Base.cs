using BAPSClientCommon.Model;
using BAPSClientCommon.ServerConfig;

namespace BAPSClientCommon.Events
{
    // This file contains the abstract base classes shared between the
    // 'updates' events (from server to client) and the 'requests'
    // events (from client to server).

    public abstract class ChannelEventArgs
    {
        protected ChannelEventArgs(ushort channelId)
        {
            ChannelId = channelId;
        }

        public ushort ChannelId { get; }
    }

    /// <summary>
    ///     Abstract base class of event payloads that reference
    ///     indices in channel track lists.
    /// </summary>
    public abstract class TrackIndexEventArgs : ChannelEventArgs
    {
        protected TrackIndexEventArgs(ushort channelId, uint index) : base(channelId)
        {
            Index = index;
        }

        /// <summary>
        ///     The track-list index being mentioned in the event.
        /// </summary>
        public uint Index { get; }
    }

    /// <summary>
    ///     Abstract base class of event payloads that reference track-list
    ///     items, and contain track data.
    /// </summary>
    public abstract class TrackEventArgs : TrackIndexEventArgs
    {
        protected TrackEventArgs(ushort channelId, uint index, Track track) : base(channelId, index)
        {
            Track = track;
        }

        /// <summary>
        ///     The track being mentioned in the event.
        /// </summary>
        public Track Track { get; }
    }

    /// <summary>
    ///     Abstract base class of event payloads over config options.
    /// </summary>
    public abstract class ConfigArgs
    {
        protected ConfigArgs(uint optionId)
        {
            OptionId = optionId;
        }

        /// <summary>The ID of the option to update.</summary>
        public uint OptionId { get; }
    }

    /// <summary>
    ///     Abstract base class of event payloads over config options that
    ///     contain a type and an index.
    /// </summary>
    public abstract class ConfigTypeIndexArgs : ConfigArgs
    {
        protected ConfigTypeIndexArgs(uint optionId, ConfigType type, int index = -1) : base(optionId)
        {
            Type = type;
            Index = index;
        }

        /// <summary>The BAPSNet type of the value.</summary>
        public ConfigType Type { get; }

        /// <summary>If present and non-negative, the index of the option to set.</summary>
        public int Index { get; }
    }

    /// <summary>
    ///     Base class for all directory update event payloads.
    /// </summary>
    public abstract class DirectoryArgs
    {
        protected DirectoryArgs(ushort directoryId)
        {
            DirectoryId = directoryId;
        }

        /// <summary>
        ///     The ID of the directory this update concerns.
        /// </summary>
        public ushort DirectoryId { get; }
    }

    /// <summary>
    ///     Payload for a channel marker (position/cue/intro) server update.
    /// <para>
    ///     Two concrete classes extend this one: <see cref="Updates.MarkerEventArgs"/>
    ///     for when a server is telling the client that a marker has moved, and
    ///     <see cref="Requests.MarkerEventArgs"/> for when a client is telling the
    ///     server to move a marker.
    /// </para>
    /// </summary>
    public abstract class MarkerEventArgs : ChannelEventArgs
    {
        /// <summary>
        ///     Constructs a channel marker server update.
        /// </summary>
        /// <param name="channelId">The ID of the channel whose marker is being moved.</param>
        /// <param name="marker">The marker being moved.</param>
        /// <param name="newValue">The new value of the marker.</param>
        protected MarkerEventArgs(ushort channelId, MarkerType marker, uint newValue) : base(channelId)
        {
            Marker = marker;
            NewValue = newValue;
        }

        /// <summary>
        ///     The specific marker being changed.
        /// </summary>
        public MarkerType Marker { get; }

        /// <summary>
        ///     The new value of the position marker being set.
        /// </summary>
        public uint NewValue { get; }
    }
}
