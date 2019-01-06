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
    }
}
