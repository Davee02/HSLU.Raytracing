using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

namespace Common
{
    public struct Color(float r, float g, float b)
    {
        public static readonly Color Black = new(0, 0, 0);
        public static readonly Color White = new(1, 1, 1);
        public static readonly Color Red = new(1, 0, 0);
        public static readonly Color Green = new(0, 1, 0);
        public static readonly Color Blue = new(0, 0, 1);
        public static readonly Color Yellow = new(1, 1, 0);
        public static readonly Color Cyan = new(0, 1, 1);
        public static readonly Color Magenta = new(1, 0, 1);
        public static readonly Color Gray = new(0.5f, 0.5f, 0.5f);
        public static readonly Color LightGray = new(0.75f, 0.75f, 0.75f);
        public static readonly Color DarkGray = new(0.25f, 0.25f, 0.25f);

        public float R { get; set; } = r;

        public float G { get; set; } = g;

        public float B { get; set; } = b;

        public static Color operator +(Color a, Color b) => new(a.R + b.R, a.G + b.G, a.B + b.B);

        public Rgba32 ToRgba32() => new (R, G, B);

        public Vector4 ToVector4() => new(R, G, B, 1);

        public static Color operator *(Color color, float scalar) => new(color.R * scalar, color.G * scalar, color.B * scalar);

        public static Color operator *(Color color, int scalar) => new(color.R * scalar, color.G * scalar, color.B * scalar);

        public static Color operator *(Color color1, Color color2) => new(color1.R * color2.R, color1.G * color2.G, color1.B * color2.B);

        public static bool operator ==(Color left, Color right) => left.R == right.R && left.G == right.G && left.B == right.B;

        public static bool operator !=(Color left, Color right) => !(left == right);

        /// <summary>
        /// Performs linear interpolation between two colors.
        /// </summary>
        /// <param name="color1">The first color.</param>
        /// <param name="color2">The second color.</param>
        /// <param name="amount">The amount to interpolate (0.0 to 1.0).</param>
        /// <returns>The interpolated color.</returns>
        public static Color Lerp(Color color1, Color color2, float amount)
        {
            // Clamp amount to range [0, 1]
            amount = Math.Clamp(amount, 0f, 1f);

            return new Color(
                color1.R + ((color2.R - color1.R) * amount),
                color1.G + ((color2.G - color1.G) * amount),
                color1.B + ((color2.B - color1.B) * amount)
            );
        }


        /// <summary>
        /// Calculates the brightness of the color using the formula:
        /// 0.299 * R + 0.587 * G + 0.114 * B
        /// </summary>
        /// <returns></returns>
        public float GetBrightness()
        {
            // Human perception weights: R=0.299, G=0.587, B=0.114
            return (0.299f * R) + (0.587f * G) + (0.114f * B);
        }
    }
}
