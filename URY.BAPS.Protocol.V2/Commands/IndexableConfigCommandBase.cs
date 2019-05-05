namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base class for config commands that can take an index.
    /// </summary>
    public abstract class IndexableConfigCommandBase : CommandBase<ConfigOp>
    {
        protected IndexableConfigCommandBase(ConfigOp op, bool modeFlag) : base(op, modeFlag)
        {
        }

        protected override CommandWord OpAsCommandWord(ConfigOp op)
        {
            return op.AsCommandWord();
        }
        
        #region Convenience methods for working with indices
        
        /// <summary>
        ///     Whether this command carries an index.
        /// </summary>
        public abstract bool HasIndex { get; }
        
        /// <summary>
        ///     The index of this indexable config command.
        ///     <para>
        ///         This value is undefined if <see cref="HasIndex"/> is <c>false</c>.
        ///     </para>
        /// </summary>
        public abstract byte Index { get; }
        
        #endregion Convenience methods for working with indices
    }
}