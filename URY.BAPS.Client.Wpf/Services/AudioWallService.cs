using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf.Services
{
    /// <summary>
    ///     A service that receives requests to open and close audio walls,
    ///     and handles them appropriately.
    /// </summary>
    public class AudioWallService
    {
        [CanBeNull] private IChannelViewModel _channelOfCurrentAudioWall;
        [CanBeNull] private AudioWall _wall;

        /// <summary>
        ///     Opens an audio wall for the given channel, represented by
        ///     its view model.
        /// </summary>
        /// <param name="channel">The channel to open an audio wall for.</param>
        public void OpenAudioWall(IChannelViewModel channel)
        {
            if (_channelOfCurrentAudioWall == channel) return;
            if (_channelOfCurrentAudioWall != null) CloseAudioWall();

            _channelOfCurrentAudioWall = channel;
            _wall = new AudioWall
            {
                DataContext = channel
            };
            _wall.Closed += HandleWallClosing;

            _wall.Show();
        }

        private void HandleWallClosing(object sender, EventArgs e)
        {
            _wall = null;
            _channelOfCurrentAudioWall = null;
        }

        /// <summary>
        ///     Closes the currently-open audio wall.
        /// </summary>
        public void CloseAudioWall()
        {
            _wall?.Close();
        }
    }
}