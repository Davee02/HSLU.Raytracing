using System.Numerics;

namespace Common
{
    public struct Light(Vector3 position, Color color, float a = 0.0f, float b = 0.0f, float c = 1.0f)
    {
        public Light()
            : this(Vector3.Zero, Color.White, c: 1.0f)
        {

        }

        public Vector3 Position { get; set; } = position;

        public Color Color { get; set; } = color;

        // Attenuation coefficients
        public float AttenuationA { get; set; } = a;

        public float AttenuationB { get; set; } = b;

        public float AttenuationC { get; set; } = c; // can also be called "intensity"
    }
}
