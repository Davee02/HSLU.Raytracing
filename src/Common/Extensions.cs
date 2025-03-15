namespace Common
{
    public static class Extensions
    {
        public static float ToRadians(this float degrees)
            => degrees * MathF.PI / 180;

        public static float ToDegrees(this float radians)
            => radians * 180 / MathF.PI;
    }
}
