using System.Numerics;

namespace Common;
public readonly struct Camera
{
    private readonly float zDistance = 0f;

    public Camera(Vector3 position, Vector2 viewPort, Vector2 imageSize, float fieldOfView)
    {
        Position = position;
        ViewWidth = viewPort.X;
        ViewHeight = viewPort.Y;
        ImageWidth = imageSize.X;
        ImageHeight = imageSize.Y;
        FieldOfView = fieldOfView;

        zDistance = (ViewHeight / 2.0f) / MathF.Tan(fieldOfView.ToRadians() / 2); // Convert degrees to radians and half the angle
    }

    public Vector3 Position { get; }

    public float ViewWidth { get; }

    public float ViewHeight { get; }

    public float ImageWidth { get; }

    public float ImageHeight { get; }

    public float FieldOfView { get; }

    public Ray GetRay(int pixelX, int pixelY, bool usePerspectiveProjection = true)
    {
        // Calculate the normalized device coordinates (NDC) in [0,1] range
        float ndcX = pixelX / ImageWidth;
        float ndcY = pixelY / ImageHeight;

        // Convert to [-0.5, 0.5] range centered at the middle of the image
        float screenX = (ndcX - 0.5f) * ViewWidth;
        float screenY = (ndcY - 0.5f) * ViewHeight;

        if (usePerspectiveProjection)
        {
            // Create a point on the image plane
            var pointOnImagePlane = new Vector3(screenX, screenY, zDistance);

            // Calculate the direction from camera position to this point
            var direction = Vector3.Normalize(pointOnImagePlane);

            // Return ray starting from camera position going in the calculated direction
            return new Ray(Position, direction);
        }
        else
        {
            // Orthographic projection
            var rayOrigin = new Vector3(Position.X + screenX, Position.Y + screenY, Position.Z);
            var direction = new Vector3(0, 0, 1); // Looking straight down Z-axis
            return new Ray(rayOrigin, direction);
        }
    }
}
