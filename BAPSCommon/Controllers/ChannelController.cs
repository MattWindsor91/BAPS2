using System;
using System.Collections.Concurrent;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Model;
using BAPSClientCommon.ServerConfig;

namespace BAPSClientCommon.Controllers
{
    /// <summary>
    ///     Abstraction between a channel user interface and the BapsNet protocol.
    ///     <para>
    ///         This class uses a blocking queue to talk to the
    ///         <see cref="Sender" />, and thus should be thread-safe.
    ///     </para>
    /// </summary>
    public class ChannelController : BapsNetControllerBase
    {
        private readonly ConfigCache _configCache;
        private readonly ushort _channelId;

        public ChannelController(ushort channelId, BlockingCollection<Message> msgQueue, ConfigCache configCache) : base(msgQueue)
        {
            _channelId = channelId;
            _configCache = configCache;
        }

        /// <summary>
        ///     Asks the server to set this channel's state to 'playing'.
        /// </summary>
        public void Play()
        {
            SetState(ChannelState.Playing);
        }

        /// <summary>
        ///     Asks the server to set this channel's state to 'paused'.
        /// </summary>
        public void Pause()
        {
            SetState(ChannelState.Paused);
        }

        /// <summary>
        ///     Asks the server to set this channel's state to 'stopped'.
        /// </summary>
        public void Stop()
        {
            SetState(ChannelState.Stopped);
        }

        /// <summary>
        ///     Asks the server to set this channel's state to <see cref="state" />.
        /// </summary>
        /// <param name="state">The intended new state of the channel.</param>
        public void SetState(ChannelState state)
        {
            Send(new Message(state.AsCommand().WithChannel(_channelId)));
        }

        public void Select(uint index)
        {
            const Command cmd = Command.Playback | Command.Load;
            Send(new Message(cmd.WithChannel(_channelId)).Add(index));
        }

        private OptionKey GetChannelConfigOption(ChannelConfigChangeType type)
        {
            if (type.HasFlag(ChannelConfigChangeType.AutoAdvance))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.Off) ||
                      type.HasFlag(ChannelConfigChangeType.On)))
                    throw new ArgumentOutOfRangeException(nameof(type), type, "AutoAdvance must have Off or On flag");
                return OptionKey.AutoAdvance;
            }

            if (type.HasFlag(ChannelConfigChangeType.PlayOnLoad))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.Off) ||
                      type.HasFlag(ChannelConfigChangeType.On)))
                    throw new ArgumentOutOfRangeException(nameof(type), type, "PlayOnLoad must have Off or On flag");
                return OptionKey.AutoPlay;
            }

            if (type.HasFlag(ChannelConfigChangeType.Repeat))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.All) ||
                      type.HasFlag(ChannelConfigChangeType.One) ||
                      type.HasFlag(ChannelConfigChangeType.None)))
                    throw new ArgumentOutOfRangeException(nameof(type), type,
                        "Repeat must have None, One, or All flag");
                return OptionKey.Repeat;
            }

            throw new ArgumentOutOfRangeException(nameof(type), type, "No valid config category flag set");
        }

        private string GetChannelConfigChoice(ChannelConfigChangeType type)
        {
            if (type.HasFlag(ChannelConfigChangeType.On)) return "Yes";
            if (type.HasFlag(ChannelConfigChangeType.Off)) return "No";
            if (type.HasFlag(ChannelConfigChangeType.None)) return "No repeat";
            if (type.HasFlag(ChannelConfigChangeType.One)) return "Repeat one";
            if (type.HasFlag(ChannelConfigChangeType.All)) return "Repeat all";
            throw new ArgumentOutOfRangeException(nameof(type), type, "No valid config choice flag set");
        }

        public void Configure(ChannelConfigChangeType type)
        {
            var optionId = GetChannelConfigOption(type);
            var choiceDesc = GetChannelConfigChoice(type);

            Send(_configCache.MakeConfigChoiceMessage(optionId, choiceDesc, _channelId));
        }

        public void AddFile(DirectoryEntry file)
        {
            var cmd = (Command.Playlist | Command.AddItem).WithChannel(_channelId);
            Send(new Message(cmd).Add((uint) TrackType.File).Add(file.DirectoryId).Add(file.Description));
        }

        /// <summary>
        ///     Asks the BAPS server to delete the track-list item for this
        ///     channel at index <see cref="index" />.
        /// </summary>
        /// <param name="index">The 0-based index of the item to delete.</param>
        public void DeleteItemAt(uint index)
        {
            var cmd = (Command.Playlist | Command.DeleteItem).WithChannel(_channelId);
            Send(new Message(cmd).Add(index));
        }

        /// <summary>
        ///     Asks the BAPS server to reset this channel, deleting all
        ///     track-list items.
        /// </summary>
        public void Reset()
        {
            var cmd = (Command.Playlist | Command.ResetPlaylist).WithChannel(_channelId);
            Send(new Message(cmd));
        }

        /// <summary>
        ///     Asks the BAPS server to move one of this channel's markers.
        /// </summary>
        /// <param name="type">The type of marker to move.</param>
        /// <param name="newValue">The requested new value.</param>
        public void SetMarker(MarkerType type, uint newValue)
        {
            var cmd = (Command.Playback | type.AsCommand()).WithChannel(_channelId);
            Send(new Message(cmd).Add(newValue));
        }
    }
}