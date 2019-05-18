using URY.BAPS.Client.Wpf.ViewModel;
using URY.BAPS.Client.Wpf.Walls;

namespace URY.BAPS.Client.Wpf.Services
{
    /// <summary>
    ///     A service that receives requests to open and close text walls, and handles them appropriately.
    /// </summary>
    [UsedImplicitly]
    public class TextWallService : WallServiceBase<TextWall, ITextViewModel>
    {
        protected override TextWall MakeWall(ITextViewModel model)
        {
            return new TextWall {DataContext = model};
        }
    }
}