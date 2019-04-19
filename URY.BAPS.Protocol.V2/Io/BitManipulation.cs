using System;

namespace URY.BAPS.Protocol.V2.Io
{
    /// <summary>
    ///     Utilities for bit twiddling.
    /// </summary>
    public static class BitManipulation
    {
        /// <summary>
        ///     Shuffles the first <paramref name="count" /> bytes of <paramref name="buffer" />
        ///     from host order to network order (or back).
        /// </summary>
        /// <param name="buffer">The buffer to modify in-place.</param>
        /// <param name="count">The number of bytes to shuffle.</param>
        public static void ShuffleForNetworkOrder(byte[] buffer, int count)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(buffer, 0, count);
        }
    }
}
