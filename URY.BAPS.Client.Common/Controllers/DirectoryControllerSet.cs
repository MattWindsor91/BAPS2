using JetBrains.Annotations;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Contains and constructs <see cref="DirectoryController" />s.
    /// </summary>
    public class DirectoryControllerSet : ControllerSetBase<DirectoryController>
    {
        public DirectoryControllerSet([CanBeNull] IClientCore core) : base(core)
        {
        }

        [Pure]
        protected override DirectoryController MakeController(byte directoryId)
        {
            return new DirectoryController(directoryId, Core);
        }
    }
}