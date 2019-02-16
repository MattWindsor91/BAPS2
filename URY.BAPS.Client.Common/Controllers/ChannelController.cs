﻿using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.Updaters;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Abstraction between a channel user interface and the BapsNet protocol.
    /// </summary>
    public class ChannelController : BapsNetControllerBase, IPlaybackController
    {
        private readonly ushort _channelId;
        [NotNull] private readonly ConfigController _config;

        public ChannelController(ushort channelId, [CanBeNull] IClientCore core, [CanBeNull] ConfigController config) :
            base(core)
        {
            _channelId = channelId;
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        ///     An event interface that broadcasts playlist server updates.
        ///     <para>
        ///         Note that the updates may include other channels; anything subscribing to this interface
        ///         must check incoming events to see if they affect the right channel.
        ///     </para>
        /// </summary>
        public IPlaylistServerUpdater PlaylistUpdater => Core;

        /// <summary>
        ///     An event interface that broadcasts playback server updates.
        ///     <para>
        ///         Note that the updates may include other channels; anything subscribing to this interface
        ///         must check incoming events to see if they affect the right channel.
        ///     </para>
        /// </summary>
        public IPlaybackServerUpdater PlaybackUpdater => Core;

        /// <summary>
        ///     Asks the server to set this channel's state to <see cref="state" />.
        /// </summary>
        /// <param name="state">The intended new state of the channel.</param>
        public void SetState(PlaybackState state)
        {
            SendAsync(new Message(state.AsCommand().WithChannel(_channelId)));
        }

        /// <summary>
        ///     Asks the BAPS server to move one of this channel's markers.
        /// </summary>
        /// <param name="type">The type of marker to move.</param>
        /// <param name="newValue">The requested new value.</param>
        public void SetMarker(MarkerType type, uint newValue)
        {
            var cmd = (Command.Playback | type.AsCommand()).WithChannel(_channelId);
            SendAsync(new Message(cmd).Add(newValue));
        }

        public void Select(uint index)
        {
            const Command cmd = Command.Playback | Command.Load;
            SendAsync(new Message(cmd.WithChannel(_channelId)).Add(index));
        }

        public void SetFlag(ChannelFlag flag, bool value)
        {
            var optionKey = flag.ToOptionKey();
            var choiceDesc = ChoiceKeys.BooleanToChoice(value);
            _config.SetChoice(optionKey, choiceDesc, _channelId);
        }

        public void SetRepeatMode(RepeatMode newMode)
        {
            _config.SetChoice(OptionKey.Repeat, newMode.ToChoiceKey(), _channelId);
        }

        public void AddFile(DirectoryEntry file)
        {
            var cmd = (Command.Playlist | Command.AddItem).WithChannel(_channelId);
            SendAsync(new Message(cmd).Add((uint) TrackType.File).Add(file.DirectoryId).Add(file.Description));
        }

        /// <summary>
        ///     Asks the BAPS server to delete the track-list item for this
        ///     channel at index <see cref="index" />.
        /// </summary>
        /// <param name="index">The 0-based index of the item to delete.</param>
        public void DeleteItemAt(uint index)
        {
            var cmd = (Command.Playlist | Command.DeleteItem).WithChannel(_channelId);
            SendAsync(new Message(cmd).Add(index));
        }

        /// <summary>
        ///     Asks the BAPS server to reset this channel, deleting all
        ///     track-list items.
        /// </summary>
        public void Reset()
        {
            var cmd = (Command.Playlist | Command.ResetPlaylist).WithChannel(_channelId);
            SendAsync(new Message(cmd));
        }
    }
}