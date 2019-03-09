using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Wpf.ViewModel;
using URY.BAPS.Client.Wpf.Walls;

namespace URY.BAPS.Client.Wpf.Services
{
    /// <summary>
    ///     A service that receives requests to open and close audio walls, and handles them appropriately.
    /// </summary>
    [UsedImplicitly]
    public class AudioWallService : WallServiceBase<AudioWall, IChannelViewModel>
    {
        protected override AudioWall MakeWall(IChannelViewModel model)
        {
            return new AudioWall {DataContext = model};
        }
    }
}