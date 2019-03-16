using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf.Services
{
    /// <summary>
    ///     A service that builds <see cref="DirectoryViewModel"/>s and their dependencies.
    /// </summary>
    [UsedImplicitly]
    public class DirectoryFactoryService
    {
        [NotNull] private readonly DirectoryControllerSet _controllerSet;

        /// <summary>
        ///     Constructs a directory factory.
        /// </summary>
        /// <param name="controllerSet">
        ///     The directory set from which we get controllers for each directory (to talk to the BAPS server).
        /// </param>
        public DirectoryFactoryService(
            [CanBeNull] DirectoryControllerSet controllerSet)
        {
            _controllerSet = controllerSet ?? throw new ArgumentNullException(nameof(controllerSet));
        }
        
        /// <summary>
        ///     Creates a directory view model.
        /// </summary>
        /// <param name="id">The ID of the channel whose view model is being created.</param>
        /// <returns>A <see cref="IChannelViewModel"/> over channel <see cref="id"/>.</returns>
        [Pure]
        public DirectoryViewModel Make(ushort id)
        {
            var controller = _controllerSet.ControllerFor(id);
            return new DirectoryViewModel(id, controller);
        }
    }
}