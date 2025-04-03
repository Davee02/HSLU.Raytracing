using System.Numerics;

namespace Common
{
    public readonly struct Ray(Vector3 origin, Vector3 direction)
    {
        public readonly Vector3 Origin => origin;

        public readonly Vector3 Direction => direction;
    }
}
