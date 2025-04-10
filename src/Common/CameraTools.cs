using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

namespace Common;
public static class CameraTools
{
    public static IEnumerable<Camera> CreateTrackingShotsAroundObject(Camera startCamera, int frames, int radius)
    {
        var angleStep = 2*MathF.PI / frames;
        var center = startCamera.LookAt;

        for (int i = 0; i < frames; i++)
        {
            var angle = i * angleStep;
            var x = center.X + radius * MathF.Cos(angle);
            var z = center.Z + radius * MathF.Sin(angle);
            var camera = new Camera(
                position: new Vector3(x, startCamera.Position.Y, z),
                lookAt: center,
                up: startCamera.Up,
                fieldOfView: startCamera.FieldOfView,
                imageWidth: startCamera.ImageWidth,
                imageHeight: startCamera.ImageHeight);
            yield return camera;
        }
    }

    public static IEnumerable<Image<Rgba32>> CreateImagesForCameras(Scene scene, IEnumerable<Camera> cameras)
    {
        int alreadyRendered = 1;
        foreach (var camera in cameras)
        {
            scene.Camera = camera;
            scene.Render(1, 2);
            yield return scene.Bitmap.Clone();

            Console.WriteLine($"Rendered camera {alreadyRendered++} of {cameras.Count()}");
        }
    }
}
