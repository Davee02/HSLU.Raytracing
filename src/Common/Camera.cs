using System.Numerics;
namespace Common;
public struct Camera
{
    public int SampleCount { get; } = 1;

    // Camera properties
    public Vector3 LookAt { get; }

    public Vector3 Position { get; }

    public Vector3 Direction { get; }

    public Vector3 Up { get; }

    public Vector3 Right { get; }

    public float FieldOfView { get; }

    // View properties
    public float ViewWidth { get; }

    public float ViewHeight { get; }

    // Image properties
    public float ImageWidth { get; }

    public float ImageHeight { get; }

    public Vector2 ImageCenter { get; }

    private Camera(Vector3 position, Vector3 direction, Vector3 up, Vector3 lookAt, float fieldOfView, float imageWidth, float imageHeight, int sampleCount)
    {
        SampleCount = sampleCount;
        Position = position;
        Direction = direction;
        LookAt = lookAt;
        FieldOfView = fieldOfView;

        // Calculate and normalize right vector using cross product
        Right = Vector3.Normalize(Vector3.Cross(Direction, up));

        // Recalculate the true up vector to ensure orthogonality
        Up = Vector3.Normalize(Vector3.Cross(Right, Direction));

        // View settings
        var aspectRatio = imageWidth / imageHeight;

        // Calculate view dimensions
        ViewHeight = 2.0f * MathF.Tan(fieldOfView * MathF.PI / 360.0f); // half angle in radians
        ViewWidth = ViewHeight * aspectRatio;

        // Image settings
        ImageWidth = imageWidth;
        ImageHeight = imageHeight;
        ImageCenter = new Vector2(imageWidth / 2.0f, imageHeight / 2.0f);
    }

    public static Camera FromLookAt(Vector3 position, Vector3 lookAt, Vector3 up, float fieldOfView, float imageWidth, float imageHeight, int sampleCount = 1)
    {
        Vector3 direction = Vector3.Normalize(lookAt - position);
        return new Camera(position, direction, up, lookAt, fieldOfView, imageWidth, imageHeight, sampleCount);
    }

    public static Camera FromDirection(Vector3 position, Vector3 direction, Vector3 up, float fieldOfView, float imageWidth, float imageHeight, int sampleCount = 1)
    {
        Vector3 normalizedDirection = Vector3.Normalize(direction);
        Vector3 lookAt = position + normalizedDirection; // Virtual lookAt point
        return new Camera(position, normalizedDirection, up, lookAt, fieldOfView, imageWidth, imageHeight, sampleCount);
    }

    /// <summary>
    /// Gets a ray for a specific pixel
    /// </summary>
    public readonly IEnumerable<Ray> GetRaysForPixel(int pixelX, int pixelY)
    {
        for (int i = 0; i < SampleCount; i++)
        {
            // Generate random jitter offset within the pixel
            var jitter = Vector2.Zero;
            if (SampleCount > 1)
            {
                jitter = GetRandomVectorInUnitSquare();
            }
            // Convert pixel coordinates to world coordinates
            float xworld = ((pixelX + 0.5f + jitter.X) - ImageCenter.X) * (ViewWidth / ImageWidth);
            float yworld = (ImageCenter.Y - (pixelY + 0.5f + jitter.Y)) * (ViewHeight / ImageHeight); // Invert Y axis
            // Calculate ray origin and direction
            Vector3 rayOrigin = Position;
            Vector3 rayDirection = Vector3.Normalize(
                Direction +
                (xworld * Right) +
                (yworld * Up)
            );
            yield return new Ray(rayOrigin, rayDirection);
        }
    }

    private static Vector2 GetRandomVectorInUnitSquare()
    {
        // Generate a random point within a unit square (-0.5 to 0.5 in both dimensions)
        return new Vector2(
            Random.Shared.NextSingle() - 0.5f,
            Random.Shared.NextSingle() - 0.5f
        );
    }
}