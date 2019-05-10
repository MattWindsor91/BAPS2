namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Interface for visitors that can traverse unpacked BapsNet commands.
    /// </summary>
    public interface ICommandVisitor
    {
        /// <summary>
        ///     Visits a playback command.
        /// </summary>
        /// <param name="command">The command to visit.</param>
        void Visit(PlaybackCommand command);

        /// <summary>
        ///     Visits a playlist command.
        /// </summary>
        /// <param name="command">The command to visit.</param>
        void Visit(PlaylistCommand command);

        /// <summary>
        ///     Visits a database command.
        /// </summary>
        /// <param name="command">The command to visit.</param>
        void Visit(DatabaseCommand command);

        /// <summary>
        ///     Visits a system command.
        /// </summary>
        /// <param name="command">The command to visit.</param>
        void Visit(SystemCommand command);

        /// <summary>
        ///     Visits a non-indexed config command.
        /// </summary>
        /// <param name="command">The command to visit.</param>
        void Visit(NonIndexedConfigCommand command);

        /// <summary>
        ///     Visits a indexed config command.
        /// </summary>
        /// <param name="command">The command to visit.</param>
        void Visit(IndexedConfigCommand command);
        
        /// <summary>
        ///     Visits a value-carrying config command.
        /// </summary>
        /// <param name="command">The command to visit.</param>
        void Visit(ValueConfigCommand command);
       
    }
}