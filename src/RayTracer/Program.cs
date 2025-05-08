using Common;
using SixLabors.ImageSharp;
using System.Diagnostics;

const string filePath = "rastered_image.png";

var scene = SceneCreator.CreateContestScene();

scene.Render();

await scene.Bitmap.SaveAsPngAsync(filePath);

//Open the image with the default image viewer
Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

scene.Dispose();