namespace URY.BAPS.Common.Protocol.V2.Io
{
    /// <summary>
    ///     Aggregate interface that represents a bidirectional BapsNet connection:
    ///     it subsumes both <see cref="IPrimitiveSource" /> and <see cref="IPrimitiveSink" />.
    /// </summary>
    public interface IConnection : IPrimitiveSource, IPrimitiveSink
    {
    }
}