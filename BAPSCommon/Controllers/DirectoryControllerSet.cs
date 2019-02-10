using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace BAPSClientCommon.Controllers
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
        protected override DirectoryController MakeController(ushort directoryId)
        {
            return new DirectoryController(directoryId, Core);
        }
    }
}