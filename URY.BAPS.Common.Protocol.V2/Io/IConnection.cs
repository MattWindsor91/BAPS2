namespace URY.BAPS.Common.Protocol.V2.Io
{
    /// <summary>
    ///     Aggregate interface that represents a bidirectional BapsNet connection:
    ///     it subsumes both <see cref="IBapsNetSource"/> and <see cref="ISink"/>.
    /// </summary>
    public interface IConnection : IBapsNetSource, ISink
    {
    }
}