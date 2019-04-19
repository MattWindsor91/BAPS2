namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Interface for unpacked command structures.
    /// </summary>
    public interface ICommand
    {
        void Accept(ICommandVisitor? visitor);

        CommandWord Packed { get; }
    }

    public abstract class CommandBase<TOp> : ICommand
    {
        public TOp Op { get; }

        protected CommandBase(TOp op)
        {
            Op = op;
        }

        protected abstract CommandWord OpAsCommandWord(TOp op);

        public abstract void Accept(ICommandVisitor? visitor);

        public abstract CommandWord Packed { get; }
    }

    public abstract class ChannelCommandBase<TOp> : CommandBase<TOp>
    {
        public byte Channel { get; }
        public bool ModeFlag { get; }

        protected ChannelCommandBase(TOp op, byte channel, bool modeFlag) : base(op)
        {
            Channel = channel;
            ModeFlag = modeFlag;
        }

        public override CommandWord Packed => OpAsCommandWord(Op).WithChannelModeFlag(ModeFlag).WithChannel(Channel);
    }

    public class PlaybackCommand : ChannelCommandBase<PlaybackOp>
    {
        public PlaybackCommand(PlaybackOp op, byte channel, bool modeFlag) : base(op, channel, modeFlag)
        {
        }

        protected override CommandWord OpAsCommandWord(PlaybackOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }
    }


    public class PlaylistCommand : ChannelCommandBase<PlaylistOp>
    {
        public PlaylistCommand(PlaylistOp op, byte channel, bool modeFlag) : base(op, channel, modeFlag)
        {
        }

        protected override CommandWord OpAsCommandWord(PlaylistOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }
    }

    /// <summary>
    ///     Abstract base for commands that have a 'normal' layout of
    ///     mode mask and value.
    /// </summary>
    /// <typeparam name="TOp">The enumeration of allowed operations.</typeparam>
    public abstract class NormalCommandBase<TOp> : CommandBase<TOp>
    {
        public bool ModeFlag { get; }

        public byte Value { get; }

        public NormalCommandBase(TOp op, byte value, bool modeFlag) : base(op)
        {
            ModeFlag = modeFlag;
            Value = value;
        }

        public override CommandWord Packed => OpAsCommandWord(Op).WithModeFlag(ModeFlag).WithValue(Value);
    }

    public class DatabaseCommand : NormalCommandBase<DatabaseOp>
    {
        protected override CommandWord OpAsCommandWord(DatabaseOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        public DatabaseCommand(DatabaseOp op, byte value, bool modeFlag) : base(op, value, modeFlag)
        {
        }
    }

    public class SystemCommand : NormalCommandBase<SystemOp>
    {
        protected override CommandWord OpAsCommandWord(SystemOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        public SystemCommand(SystemOp op, byte value, bool modeFlag) : base(op, value, modeFlag)
        {
        }
    }

    public class ConfigCommand : CommandBase<ConfigOp>
    {
        public bool IsIndexed { get; }
        
        /// <summary>
        ///     The index of an indexed configuration option that this command concerns, if
        ///     <see cref="IsIndexed"/> is <code>true</code>.
        /// </summary>
        public byte Index { get; }

        public ConfigCommand(ConfigOp op, bool isIndexed, byte index) : base(op)
        {
            IsIndexed = isIndexed;
            Index = index;
        }

        protected override CommandWord OpAsCommandWord(ConfigOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        public override CommandWord Packed =>
            OpAsCommandWord(Op).WithConfigIndexedFlag(IsIndexed).WithConfigIndex(Index);
    }
}
