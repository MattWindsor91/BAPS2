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

        public ChannelController(ushort channelID, BlockingCollection<Message> msgQueue)
        {
            _channelID = channelID;
            _msgQueue = msgQueue;
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

        private OptionCacheInfo GetChannelConfigOption(ChannelConfigChangeType type)
        {
            if (type.HasFlag(ChannelConfigChangeType.AutoAdvance))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.Off) ||
                      type.HasFlag(ChannelConfigChangeType.On)))
                    throw new ArgumentOutOfRangeException("type", type, "AutoAdvance must have Off or On flag");
                return ConfigCache.getOption("Auto Advance");
            }
            else if (type.HasFlag(ChannelConfigChangeType.PlayOnLoad))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.Off) ||
                      type.HasFlag(ChannelConfigChangeType.On)))
                    throw new ArgumentOutOfRangeException("type", type, "PlayOnLoad must have Off or On flag");
                return ConfigCache.getOption("Play on load");
            }
            else if (type.HasFlag(ChannelConfigChangeType.Repeat))
            {
                if (!(type.HasFlag(ChannelConfigChangeType.All) ||
                      type.HasFlag(ChannelConfigChangeType.One) ||
                      type.HasFlag(ChannelConfigChangeType.None)))
                    throw new ArgumentOutOfRangeException("type", type, "Repeat must have None, One, or All flag");
                return ConfigCache.getOption("Repeat");
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
            var msg = new Message(Command.CONFIG | Command.SETCONFIGVALUE | Command.CONFIG_USEVALUEMASK | (Command)_channelID);

            var oci = GetChannelConfigOption(type);
            msg.Add((uint)oci.optionid);
            msg.Add((uint)oci.type);

            var choice = GetChannelConfigChoice(type);
            msg.Add((uint)(int)oci.choiceList[choice]);

            _msgQueue.Add(msg);
        }
    }
}
