#if NET5_0_OR_GREATER || NETCOREAPP3_1 || NETSTANDARD2_1
#define NETSTANDARD2_1_COMPATIBLE //.NET 5.0 or .NET Core 3.1 or .NET Standard 2.1
#endif

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace BitmapTools
{
    /// <summary>
    /// Provides methods for creating <see cref="Color"/> instances.
    /// </summary>
    public static class ColorFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="Color"/> from a string in hexadecimal format.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        /// <exception cref="FormatException">Thrown if the <paramref name="hex"/> string does not match the required format.</exception>
        public static Color CreateColorFromHex(string hex)
        {
            if (hex.StartsWith("0x")) //0x89abcdef -> #89abcdef
            {
                hex = Regex.Replace(hex, @"^(0x)([0-9a-f]+)$", "$2");
            }

            if (!hex.StartsWith("#")) //89abcdef -> #89abcdef
            {
                hex = $"#{hex}";
            }
            
            if (!Regex.IsMatch(hex, @"^#([0-9a-f]{3}|[0-9a-f]{6}|[0-9a-f]{8})$"))
            {
                throw new FormatException($"Cannot parse color from \"{hex}\"");
            }

            return ColorTranslator.FromHtml(hex);
        }

        /// <summary>
        /// Creates a <see cref="Color"/> from RGB.
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static Color CreateColorFromRgb(int rgb)
        {
            var segments = ParseSegments((uint) rgb, 6, 2);
            return Color.FromArgb(segments[0], segments[1], segments[2]);
        }

        /// <summary>
        /// Creates a <see cref="Color"/> from ARGB.
        /// </summary>
        /// <param name="argb"></param>
        /// <returns></returns>
        public static Color CreateColorFromArgb(uint argb)
        {
            var segments = ParseSegments(argb, 8, 2);
            return Color.FromArgb(segments[0], segments[1], segments[2], segments[3]);
        }

        private static int[] ParseSegments(uint number, int numberLength, int segmentSize)
        {
            var numberString = number.ToString($"X{numberLength}");
            var stringSegments = new List<string>();
            for (var i = 0; i < numberLength; i += segmentSize)
            {
                var substring = numberString.Substring(i, segmentSize);
                stringSegments.Add(substring);
            }
            var segments = stringSegments
                .Select(segment => Convert.ToInt32(segment, 16))
                .ToArray();

            return segments;
        }
    }
}