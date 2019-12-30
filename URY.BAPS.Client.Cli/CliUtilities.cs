using System;
using System.Xml.Linq;

namespace URY.BAPS.Client.Cli
{
    public static class CliUtilities
    {
        public static string GetText(string prompt, string defaultValue)
        {
            var fullPrompt = FullPrompt(prompt, defaultValue);
            var input = ReadLine.Read(fullPrompt, defaultValue);
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }

        private static string FullPrompt(string prompt, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(defaultValue) ? $"{prompt}: " : $"{prompt} (default: {defaultValue}): ";
        }
    }
}