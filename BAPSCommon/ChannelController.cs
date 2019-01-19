using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

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
        private readonly ushort _channelID;
        private BlockingCollection<Message> _msgQueue;
        private ConfigCache _cache;

        public ChannelController(ushort channelID, BlockingCollection<Message> msgQueue, ConfigCache cache)
        {
            _channelID = channelID;
            _msgQueue = msgQueue;
            _cache = cache;
        }

        public void Play()
        {
            var cmd = Command.PLAYBACK | Command.PLAY | (Command)_channelID;
            _msgQueue.Add(new Message(cmd));
        }

        public void Pause()
        {
            var cmd = Command.PLAYBACK | Command.PAUSE | (Command)_channelID;
            _msgQueue.Add(new Message(cmd));
        }

        public void Stop()
        {
            var cmd = Command.PLAYBACK | Command.STOP | (Command)_channelID;
            _msgQueue.Add(new Message(cmd));
        }

        public void Select(uint index)
        {
            var cmd = Command.PLAYBACK | Command.LOAD;
            cmd |= (Command)(_channelID & 0x3f);
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
            else if (type.HasFlag(ChannelConfigChangeType.PlayOnLoad))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.Off) ||
                      type.HasFlag(ChannelConfigChangeType.On)))
                    throw new ArgumentOutOfRangeException("type", type, "PlayOnLoad must have Off or On flag");
                return ConfigDescriptions.PlayOnLoad;
            }
            else if (type.HasFlag(ChannelConfigChangeType.Repeat))
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

            _msgQueue.Add(_cache.MakeConfigChoiceMessage(optionDesc, choiceDesc, index: _channelID));
        }
    }
}
