using JetBrains.Annotations;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Common.Protocol.V2.MessageIo;

namespace URY.BAPS.Client.Protocol.V2.Controllers
{
    /// <summary>
    ///     Contains and constructs <see cref="DirectoryController" />s.
    /// </summary>
    public class DirectoryControllerSet : ControllerSetBase<DirectoryController>
    {
        public DirectoryControllerSet(MessageConnectionManager? core) : base(core)
        {
        }

        [Pure]
        protected override DirectoryController MakeController(byte directoryId)
        {
            return new DirectoryController(directoryId, ConnectionManager);
        }
    }
}