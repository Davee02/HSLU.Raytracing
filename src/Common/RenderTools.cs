using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Common;
public static class RenderTools
{
    public static IEnumerable<Camera> CreateTrackingShotsAroundObject(Camera startCamera, int frames, int radius)
    {
        var angleStep = 2*MathF.PI / frames;
        var center = startCamera.LookAt;

        for (int i = 0; i < frames; i++)
        {
            var angle = i * angleStep;
            var x = center.X + (radius * MathF.Cos(angle));
            var z = center.Z + (radius * MathF.Sin(angle));
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

    public static IEnumerable<Image<Rgba32>> CreateImagesForCameras(Scene scene, Camera[] cameras)
    {
        int alreadyRendered = 1;
        foreach (var camera in cameras)
        {
            scene.Camera = camera;
            scene.Render();
            yield return scene.Bitmap.Clone();

            Console.WriteLine($"Rendered camera {alreadyRendered++} of {cameras.Count()}");
        }
    }

    public static Image<Rgba32> CreateAnimatedGif(Image<Rgba32>[] images, int frameDelayMs = 20)
    {
        var firstFrame = images.First();
        var gif = new Image<Rgba32>(firstFrame.Width, firstFrame.Height);

        var gifMetaData = gif.Metadata.GetGifMetadata();
        gifMetaData.RepeatCount = 0; // 0 means loop indefinitely

        var metadata = gif.Frames.RootFrame.Metadata.GetGifMetadata();
        metadata.FrameDelay = frameDelayMs;
        metadata.ColorTableMode = GifColorTableMode.Local;

        foreach (var image in images)
        {
            metadata = image.Frames.RootFrame.Metadata.GetGifMetadata();
            metadata.FrameDelay = frameDelayMs;

            gif.Frames.AddFrame(image.Frames.RootFrame);
        }

        gif.Frames.RemoveFrame(0);
        return gif;
    }
}
