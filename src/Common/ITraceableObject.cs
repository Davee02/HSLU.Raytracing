namespace Common
{
    public interface ITraceableObject
    {
        bool Intersect(Ray ray, out float lambda);

        Vector3 SurfaceNormal(Vector3 intersectionPoint);

        Color Color { get; }
    }
}
