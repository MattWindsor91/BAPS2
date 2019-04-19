namespace URY.BAPS.Protocol.V2.Io
{
    /// <summary>
    ///     Aggregate interface that represents a bidirectional BapsNet connection:
    ///     it subsumes both <see cref="ISource"/> and <see cref="ISink"/>.
    /// </summary>
    public interface IConnection : ISource, ISink
    {
    }
}