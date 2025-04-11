using Common;
using SixLabors.ImageSharp;
using System.Diagnostics;
using System.Numerics;
using RayTracer;

const string filePath = "rastered_image.png";
var outputDimensions = new Vector2(1280 / 2, 720 / 2);

using var scene = Scene.CreateCornellBoxScene();
//using var scene = new Scene(outputDimensions)
//{
//    DiffusedLights =
//    [
//        new Light
//        {
//            Color = new Common.Color(1, 0.9f, 0.8f), // Warm light
//            Position = new Vector3(1000, 200, -300),
//            Intensity = 0.7f,
//        },
//        new Light
//        {
//            Color = new Common.Color(0.8f, 0.8f, 1f), // Cool light
//            Position = new Vector3(300, 300, -200),
//            Intensity = 0.5f
//        },
//        new Light
//        {
//            Color = new Common.Color(1, 0, 0), // Red light
//            Position = new Vector3(750, -1000, 200),
//            Intensity = 0.2f
//        }
//    ],
//    AmbientLight = new Light
//    {
//        Color = new Common.Color(1, 1, 1),
//        Intensity = 0.2f
//    },
//    BackgroundColor = new Common.Color(0.1f, 0.1f, 0.2f),
//    Camera = new Camera(position: new Vector3(960, 540, -500), viewPort: new Vector2(1920f, 1080f), imageSize: outputDimensions, fieldOfView: 60),
//};

//scene.AddSphere(new Vector3(900, 500, 300), 200, new Material(Common.Color.White, 0f, 20, 0.7f, 1.33f));

//scene.AddSphere(new Vector3(1000, 500, 800), 100, new Material(Common.Color.Red, 0.5f, 20));
////scene.AddSphere(new Vector3(1100, 550, 700), 50, new Material(Common.Color.Green, 0.5f, 20));
//scene.AddCube(new Vector3(900, 550, 700), 50, new Vector3(15, 15, 15), new Material(Common.Color.Yellow, 0f, 20));

//scene.AddSphere(new Vector3(200, 500, 800), 100, new Material(Common.Color.Red, 0.5f, 20));
//scene.AddSphere(new Vector3(300, 550, 700), 50, new Material(Common.Color.Green, 0.5f, 20));


//scene.AddPlane(new Vector3(0, 900, 0), new Vector3(0, 0, 0), new Material(new Common.Color(0.8f, 0.8f, 0.8f), 0.1f, 20)); // gray floor in the front
//scene.AddPlane(new Vector3(0, 0, 1000), new Vector3(0, 0, 90), new Material(new Common.Color(0.7f, 0.7f, 0.9f), 0.1f, 20)); // blue wall on the left
//scene.AddPlane(new Vector3(0, 0, 10000), new Vector3(0, 90, 90), new Material(new Common.Color(0.7f, 0.9f, 0.7f), 0.1f, 20)); // green wall at the back

//scene.AddSphere(new Vector3(1100, 200, 310), 200, new Material(new Common.Color(1, 0.9f, 0.2f), 0.9f, 20));
//scene.AddSphere(new Vector3(1500, 500, 200), 200, new Material(new Common.Color(1, 0.9f, 0.2f), 0.9f, 20));
//scene.AddCube(new Vector3(1000, 150, 30), 30, new Vector3(80, 10, 30), new Material(new Common.Color(1, 0.9f, 0.2f), 0.1f, 20));

//scene.AddSphere(new Vector3(700, 400, 350), 80, new Material(new Common.Color(1, 0.3f, 0.3f), 0.1f, 20));
//scene.AddSphere(new Vector3(850, 450, 300), 70, new Material(new Common.Color(0.3f, 1, 0.3f), 0.1f, 20));
//scene.AddSphere(new Vector3(750, 550, 400), 90, new Material(new Common.Color(0.3f, 0.3f, 1), 0.1f, 20));
//scene.AddSphere(new Vector3(600, 500, 250), 60, new Material(new Common.Color(1, 1, 0.3f), 0.1f, 20));

//scene.AddCube(new Vector3(750, 700, 350), 120, new Vector3(25, 45, 10), new Material(new Common.Color(0.8f, 0.4f, 0.8f), 0.1f, 20));

//scene.AddSphere(new Vector3(1100, 800, 250), 10, new Material(new Common.Color(0, 0.8f, 0.8f), 0.1f, 20));
//scene.AddSphere(new Vector3(550, 850, 250), 40, new Material(new Common.Color(0.8f, 0.8f, 0), 0.1f, 20));

//scene.AddCube(new Vector3(900, 820, 150), 60, new Vector3(0, 30, 45), new Material(new Common.Color(0.5f, 0.5f, 1), 0.1f, 20));
//scene.AddCube(new Vector3(960, 750, 100), 20, new Vector3(45, 45, 45), new Material(new Common.Color(0.5f, 0.5f, 1), 0.1f, 20));
//scene.AddCube(new Vector3(1000, 600, 100), 80, new Vector3(45, 60, 15), new Material(new Common.Color(0.5f, 1, 0.5f), 0f, 20));
//scene.AddCube(new Vector3(500, 200, 300), 120, new Vector3(70, 60, 15), new Material(new Common.Color(0, 0, 0), 1f, 50));

//var defaultMaterial = new Material(new Common.Color(1, 1, 1), 0.5f, 20);
//var triangles = ObjImporter.ImportObj(@"C:\Users\David\Downloads\Untitled.obj", defaultMaterial);
//scene.AddTriangles(triangles);

//var scene = new Scene(outputDimensions)
//{
//    DiffusedLights =
//    [
//        new Light
//        {
//            Color = new Common.Color(1, 0.9f, 0.8f), // Warm light
//            Position = new Vector3(1000, 200, -300),
//            Intensity = 0.7f,
//        }
//    ],
//    AmbientLight = new Light
//    {
//        Color = new Common.Color(1, 1, 1),
//        Intensity = 0.2f
//    },
//    BackgroundColor = new Common.Color(0.1f, 0.1f, 0.2f),
//    RenderSettings = new RenderSettings(lineSkipStep: 1, maxRecursionDepth: 3),
//};

//scene.Camera = new Camera(
//    position: new Vector3(scene.ImageSize.X / 2, scene.ImageSize.Y / 2, -300),
//    lookAt: new Vector3(scene.ImageSize.X / 2, scene.ImageSize.Y / 2, 500),
//    up: new Vector3(0, -1, 0),
//    imageWidth: scene.ImageSize.X,
//    imageHeight: scene.ImageSize.Y,
//    fieldOfView: 60);

//scene.AddCube(new Vector3(1000, 300, 200), 100, new Vector3(0, 0, 0), new Material(Common.Color.White, 0f, 20));
//scene.AddCube(new Vector3(700, 300, 200), 100, new Vector3(0, 0, 0), new Material(Common.Color.Red, 0f, 20));

//var triangles = ObjImporter.LoadFromFile(@"C:\Users\David\Downloads\blender_exported\Untitled.obj");
//scene.AddTriangles(triangles);

Console.WriteLine(scene.PrintInfo());

var sw = Stopwatch.StartNew();
scene.Render();
sw.Stop();

Console.WriteLine($"Finished rendering in {sw.ElapsedMilliseconds} ms");

await scene.Bitmap.SaveAsPngAsync(filePath);

//Open the image with the default image viewer
Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

scene.Dispose();