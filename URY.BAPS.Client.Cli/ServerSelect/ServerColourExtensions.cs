using System;
using URY.BAPS.Client.Common.ServerSelect;

namespace URY.BAPS.Client.Cli.ServerSelect
{
    /// <summary>
    ///     Static class for extensions on <see cref="ServerColour"/>.
    /// </summary>
    public static class ServerColourExtensions
    {
        public static ConsoleColor ToConsoleColor(this ServerColour colour)
        {
            return colour switch
            {
                ServerColour.None => Console.ForegroundColor,
                ServerColour.Blue => ConsoleColor.Blue,
                ServerColour.Green => ConsoleColor.Green,
                ServerColour.Red => ConsoleColor.Red,
                ServerColour.Yellow => ConsoleColor.Yellow,
                _ => ConsoleColor.Gray
            };
        }
    }
}