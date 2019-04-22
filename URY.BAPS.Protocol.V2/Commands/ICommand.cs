﻿namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Interface for unpacked command structures.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        ///     Calls the appropriate visit method on a visitor for this command.
        /// </summary>
        /// <param name="visitor">The visitor currently visiting this command.</param>
        void Accept(ICommandVisitor? visitor);

        /// <summary>
        ///     The packed representation of this command.
        /// </summary>
        CommandWord Packed { get; }
    }
}
