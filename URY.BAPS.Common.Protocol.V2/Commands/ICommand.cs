namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Interface for unpacked command structures.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        ///     The packed representation of this command.
        /// </summary>
        ushort Packed { get; }

        /// <summary>
        ///     Calls the appropriate visit method on a visitor for this command.
        /// </summary>
        /// <param name="visitor">The visitor currently visiting this command.</param>
        void Accept(ICommandVisitor visitor);
    }
}