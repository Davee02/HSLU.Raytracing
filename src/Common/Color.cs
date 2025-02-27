using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

namespace Common
{
    public readonly struct Color(float r, float g, float b)
    {
        public readonly float R => r;

        public readonly float G => g;

        public readonly float B => b;

        public static Color operator +(Color a, Color b) => new(a.R + b.R, a.G + b.G, a.B + b.B);

        public Rgba32 ToRgba32() => new (R, G, B);

        public Vector4 ToVector4() => new(R, G, B, 1);
    }
}
