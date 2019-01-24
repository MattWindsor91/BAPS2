using System;
using System.Collections.Concurrent;

namespace BAPSCommon
{
    /// <summary>
    /// Abstraction between a channel user interface and the bapsnet protocol.
    /// <para>
    /// This class uses a blocking queue to talk to the
    /// <see cref="Sender"/>, and thus should be thread-safe.
    /// </para>
    /// </summary>
    public class ChannelController
    {
        private readonly ushort _channelId;
        private BlockingCollection<Message> _msgQueue;
        private ConfigCache _cache;

        public ChannelController(ushort channelId, BlockingCollection<Message> msgQueue, ConfigCache cache)
        {
            _channelId = channelId;
            _msgQueue = msgQueue;
            _cache = cache;
        }

        public void Play()
        {
            var cmd = Command.Playback | Command.Play | (Command)_channelId;
            _msgQueue.Add(new Message(cmd));
        }

        public void Pause()
        {
            var cmd = Command.Playback | Command.Pause | (Command)_channelId;
            _msgQueue.Add(new Message(cmd));
        }

        public void Stop()
        {
            var cmd = Command.Playback | Command.Stop | (Command)_channelId;
            _msgQueue.Add(new Message(cmd));
        }

        public void Select(uint index)
        {
            var cmd = Command.Playback | Command.Load;
            cmd |= (Command)(_channelId & 0x3f);
            _msgQueue.Add(new Message(cmd).Add(index));
        }

        private string GetChannelConfigOption(ChannelConfigChangeType type)
        {
            if (type.HasFlag(ChannelConfigChangeType.AutoAdvance))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.Off) ||
                      type.HasFlag(ChannelConfigChangeType.On)))
                    throw new ArgumentOutOfRangeException("type", type, "AutoAdvance must have Off or On flag");
                return ConfigDescriptions.AutoAdvance;
            }

            if (type.HasFlag(ChannelConfigChangeType.PlayOnLoad))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.Off) ||
                      type.HasFlag(ChannelConfigChangeType.On)))
                    throw new ArgumentOutOfRangeException("type", type, "PlayOnLoad must have Off or On flag");
                return ConfigDescriptions.PlayOnLoad;
            }

            if (type.HasFlag(ChannelConfigChangeType.Repeat))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.All) ||
                      type.HasFlag(ChannelConfigChangeType.One) ||
                      type.HasFlag(ChannelConfigChangeType.None)))
                    throw new ArgumentOutOfRangeException("type", type, "Repeat must have None, One, or All flag");
                return ConfigDescriptions.Repeat;
            }
            throw new ArgumentOutOfRangeException("type", type, "No valid config category flag set");
        }

        private string GetChannelConfigChoice(ChannelConfigChangeType type)
        {
            if (type.HasFlag(ChannelConfigChangeType.On)) return "Yes";
            if (type.HasFlag(ChannelConfigChangeType.Off)) return "No";
            if (type.HasFlag(ChannelConfigChangeType.None)) return "No repeat";
            if (type.HasFlag(ChannelConfigChangeType.One)) return "Repeat one";
            if (type.HasFlag(ChannelConfigChangeType.All)) return "Repeat all";
            throw new ArgumentOutOfRangeException("type", type, "No valid config choice flag set");
        }

        public void Configure(ChannelConfigChangeType type)
        {
            var optionDesc = GetChannelConfigOption(type);
            var choiceDesc = GetChannelConfigChoice(type);

            _msgQueue.Add(_cache.MakeConfigChoiceMessage(optionDesc, choiceDesc, index: _channelId));
        }
    }
}
