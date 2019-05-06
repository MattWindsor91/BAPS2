using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Client.Common.Utils;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Messages;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Abstraction between a channel user interface and the BapsNet protocol.
    /// </summary>
    public class ChannelController : BapsNetControllerBase, IPlaybackController
    {
        private readonly byte _channelId;
        [NotNull] private readonly ConfigController _config;

        public ChannelController(byte channelId, [CanBeNull] IClientCore core, [CanBeNull] ConfigController config) :
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
        public IPlaylistServerUpdater PlaylistUpdater => Core.Updater;

        /// <summary>
        ///     An event interface that broadcasts playback server updates.
        ///     <para>
        ///         Note that the updates may include other channels; anything subscribing to this interface
        ///         must check incoming events to see if they affect the right channel.
        ///     </para>
        /// </summary>
        public IPlaybackServerUpdater PlaybackUpdater => Core.Updater;

        /// <summary>
        ///     Asks the server to set this channel's state to <see cref="state" />.
        /// </summary>
        /// <param name="state">The intended new state of the channel.</param>
        public void SetState(PlaybackState state)
        {
            var cmd = new PlaybackCommand(state.AsPlaybackOp(), _channelId);
            Send(new Message(cmd));
        }

        #region Command factories

        /// <summary>
        ///     Creates a playback command over this controller's channel.
        /// </summary>
        /// <param name="op">The playback op code for this command.</param>
        /// <param name="modeFlag">Optional mode flag (off by default).</param>
        /// <returns>A <see cref="PlaybackCommand"/> with this controller's channel and the given op and mode flag.</returns>
        private PlaybackCommand PlaybackCommand(PlaybackOp op, bool modeFlag = false)
        {
            return new PlaybackCommand(op, _channelId, modeFlag);
        }

        /// <summary>
        ///     Creates a playlist command over this controller's channel.
        /// </summary>
        /// <param name="op">The playlist op code for this command.</param>
        /// <param name="modeFlag">Optional mode flag (off by default).</param>
        /// <returns>A <see cref="PlaylistCommand"/> with this controller's channel and the given op and mode flag.</returns>
        private PlaylistCommand PlaylistCommand(PlaylistOp op, bool modeFlag = false)
        {
            return new PlaylistCommand(op, _channelId, modeFlag);
        }

        #endregion Command factories

        /// <summary>
        ///     Asks the BAPS server to move one of this channel's markers.
        /// </summary>
        /// <param name="type">The type of marker to move.</param>
        /// <param name="newValue">The requested new value.</param>
        public void SetMarker(MarkerType type, uint newValue)
        {
            var cmd = new PlaybackCommand(type.AsPlaybackOp(), _channelId);
            Send(new Message(cmd).Add(newValue));
        }

        public void Select(uint index)
        {
            var cmd = new PlaybackCommand(PlaybackOp.Load, _channelId);
            Send(new Message(cmd).Add(index));
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
            Send(MessageFactory.MakeAddFileItem(_channelId, file.DirectoryId, file.Description));
        }

        /// <summary>
        ///     Asks the BAPS server to delete the track-list item for this
        ///     channel at index <see cref="index" />.
        /// </summary>
        /// <param name="index">The 0-based index of the item to delete.</param>
        public void DeleteItemAt(uint index)
        {
            var cmd = PlaylistCommand(PlaylistOp.DeleteItem);
            Send(new Message(cmd).Add(index));
        }

        /// <summary>
        ///     Asks the BAPS server to reset this channel, deleting all
        ///     track-list items.
        /// </summary>
        public void Reset()
        {
            var cmd = PlaylistCommand(PlaylistOp.ResetPlaylist);
            Send(new Message(cmd));
        }

        /// <summary>
        ///     Asks the BAPS server to add a text item. 
        /// </summary>
        /// <param name="text">The body of the text item.</param>
        /// <param name="summary">The optional short title of the text item.</param>
        public void AddText(string text, string? summary = null)
        {
            summary ??= text.Summary();
            Send(MessageFactory.MakeAddTextItem(_channelId, summary, text));
        }
    }
}