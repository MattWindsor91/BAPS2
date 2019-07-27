﻿using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Unpacked BapsNet config command containing an index.
    /// </summary>
    /// <seealso cref="NonIndexedConfigCommand" />
    public class IndexedConfigCommand : IndexableConfigCommandBase
    {
        public IndexedConfigCommand(ConfigOp op, byte index, bool modeFlag) : base(op, modeFlag)
        {
            Index = index;
        }

        public override bool HasIndex => true;

        public override byte Index { get; }

        public override CommandWord Packed =>
            OpAsCommandWord(Op).WithConfigIndexedFlag(true).WithModeFlag(ModeFlag).WithConfigIndex(Index);

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }
    }
}