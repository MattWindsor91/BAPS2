namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Interface for visitors that can traverse unpacked BapsNet commands.
    /// </summary>
    public interface ICommandVisitor
    {
        void Visit(PlaybackCommand command);
        void Visit(PlaylistCommand command);
        void Visit(DatabaseCommand command);
        void Visit(SystemCommand command);
        void Visit(ConfigCommand command);
    }
}