namespace Common
{
    public interface ITraceableObject
    {
        const float eps = 1e-2f;

        bool TryIntersect(Ray ray, ref Hit hit);
    }
}
