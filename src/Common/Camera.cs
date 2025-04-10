using System.Numerics;

namespace Common;

public readonly struct Camera
{
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

    /// <summary>
    /// Creates a camera with specific position, look-at point, up vector, and view settings
    /// </summary>
    public Camera(Vector3 position, Vector3 lookAt, Vector3 up, float fieldOfView, float imageWidth, float imageHeight)
    {
        Position = position;
        FieldOfView = fieldOfView;
        LookAt = lookAt;

        // Calculate and normalize direction vector
        Direction = Vector3.Normalize(lookAt - position);

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


    /// <summary>
    /// Gets a ray for a specific pixel
    /// </summary>
    public Ray GetRayForPixel(int pixelX, int pixelY)
    {
        // Convert pixel coordinates to world coordinates (as shown in the slides)
        float xworld = (pixelX - ImageCenter.X) * (ViewWidth / ImageWidth);
        float yworld = (ImageCenter.Y - pixelY) * (ViewHeight / ImageHeight); // Invert Y axis

        // Calculate ray origin and direction
        Vector3 rayOrigin = Position;
        Vector3 rayDirection = Vector3.Normalize(
            Direction +
            xworld * Right +
            yworld * Up
        );

        return new Ray(rayOrigin, rayDirection);
    }
}