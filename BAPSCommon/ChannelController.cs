using System;
using System.Collections.Concurrent;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Model;

namespace BAPSClientCommon
{
    /// <summary>
    ///     Abstraction between a channel user interface and the BAPSNet protocol.
    ///     <para>
    ///         This class uses a blocking queue to talk to the
    ///         <see cref="Sender" />, and thus should be thread-safe.
    ///     </para>
    /// </summary>
    public class ChannelController
    {
        private readonly ConfigCache _cache;
        private readonly ushort _channelId;
        private readonly BlockingCollection<Message> _msgQueue;

        public ChannelController(ushort channelId, BlockingCollection<Message> msgQueue, ConfigCache cache)
        {
            _channelId = channelId;
            _msgQueue = msgQueue;
            _cache = cache;
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
            _msgQueue.Add(new Message(state.AsCommand().WithChannel(_channelId)));
        }

        public void Select(uint index)
        {
            const Command cmd = Command.Playback | Command.Load;
            _msgQueue.Add(new Message(cmd.WithChannel(_channelId)).Add(index));
        }

        private string GetChannelConfigOption(ChannelConfigChangeType type)
        {
            if (type.HasFlag(ChannelConfigChangeType.AutoAdvance))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.Off) ||
                      type.HasFlag(ChannelConfigChangeType.On)))
                    throw new ArgumentOutOfRangeException(nameof(type), type, "AutoAdvance must have Off or On flag");
                return ConfigDescriptions.AutoAdvance;
            }

            if (type.HasFlag(ChannelConfigChangeType.PlayOnLoad))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.Off) ||
                      type.HasFlag(ChannelConfigChangeType.On)))
                    throw new ArgumentOutOfRangeException(nameof(type), type, "PlayOnLoad must have Off or On flag");
                return ConfigDescriptions.PlayOnLoad;
            }

            if (type.HasFlag(ChannelConfigChangeType.Repeat))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.All) ||
                      type.HasFlag(ChannelConfigChangeType.One) ||
                      type.HasFlag(ChannelConfigChangeType.None)))
                    throw new ArgumentOutOfRangeException(nameof(type), type,
                        "Repeat must have None, One, or All flag");
                return ConfigDescriptions.Repeat;
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
            var optionDesc = GetChannelConfigOption(type);
            var choiceDesc = GetChannelConfigChoice(type);

            _msgQueue.Add(_cache.MakeConfigChoiceMessage(optionDesc, choiceDesc, _channelId));
        }

        public void AddFile(DirectoryEntry file)
        {
            var cmd = (Command.Playlist | Command.AddItem).WithChannel(_channelId);
            _msgQueue.Add(new Message(cmd).Add((uint)TrackType.File).Add(file.DirectoryId).Add(file.Description));
        }

        /// <summary>
        ///     Asks the BAPS server to delete the track-list item for this
        ///     channel at index <see cref="index"/>.
        /// </summary>
        /// <param name="index">The 0-based index of the item to delete.</param>
        public void DeleteItemAt(uint index)
        {
            var cmd = (Command.Playlist | Command.DeleteItem).WithChannel(_channelId);
            _msgQueue.Add(new Message(cmd).Add(index));
        }

        /// <summary>
        ///     Asks the BAPS server to reset this channel, deleting all
        ///     track-list items.
        /// </summary>
        public void Reset()
        {
            var cmd = (Command.Playlist | Command.ResetPlaylist).WithChannel(_channelId);
            _msgQueue.Add(new Message(cmd));
        }
    }
}