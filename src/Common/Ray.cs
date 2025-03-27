namespace Common
{
    public readonly struct Ray(System.Numerics.Vector3 origin, System.Numerics.Vector3 direction)
    {
        public readonly System.Numerics.Vector3 Origin => origin;

        public readonly System.Numerics.Vector3 Direction => direction;
    }
}
