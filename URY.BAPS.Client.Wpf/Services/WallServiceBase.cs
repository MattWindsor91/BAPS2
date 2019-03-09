using System;
using System.Windows;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using URY.BAPS.Client.Wpf.ViewModel;
using URY.BAPS.Client.Wpf.Walls;

namespace URY.BAPS.Client.Wpf.Services
{
    public abstract class WallServiceBase<TWall, TViewModel> where TWall : Window
        where TViewModel : class
    {
        /// <summary>
        ///     The view model being displayed in the current wall, if any.
        /// </summary>
        [CanBeNull] private TViewModel _modelOfCurrentWall;

        /// <summary>
        ///     The open wall dialog, if any.
        /// </summary>
        [CanBeNull] private TWall _wall;

        /// <summary>
        ///     Constructs a wall for the given view model.
        /// </summary>
        /// <param name="model">The view model to display in the wall.</param>
        /// <returns>The constructed wall.</returns>
        [CanBeNull]
        protected abstract TWall MakeWall(TViewModel model);

        /// <summary>
        ///     Opens an wall for the given view model.
        ///     <para>
        ///         If a wall is currently open, it will be closed.
        ///     </para>
        /// </summary>
        /// <param name="model">The view model to display in the wall.</param>
        public void OpenAudioWall(TViewModel model)
        {
            if (_modelOfCurrentWall == model) return;
            if (_modelOfCurrentWall != null) CloseWall();
            _modelOfCurrentWall = model;

            _wall = MakeWall(model) ?? throw new InvalidOperationException();
            _wall.Closed += HandleWallClosing;
            _wall.Show();
        }

        private void HandleWallClosing(object sender, EventArgs e)
        {
            if (_wall is TWall w) w.Closed -= HandleWallClosing;
            _wall = null;
            _modelOfCurrentWall = null;
        }

        /// <summary>
        ///     Closes the currently-open wall.
        /// </summary>
        public void CloseWall()
        {
            _wall?.Close();
        }
    }
}