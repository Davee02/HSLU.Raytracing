using System.Numerics;

namespace Common;
public static class SceneCreator
{
    public static Scene CreateFinalScene()
    {
        var outputDimensions = new Vector2(1280, 720);

        float roomSize = 800f;
        float halfSize = roomSize / 2f;
        var roomCenter = new Vector3(500, 200, 500);

        var scene = new Scene(outputDimensions)
        {
            DiffusedLights =
            [
                new Light
        {
            Color = new Common.Color(1, 0.9f, 0.8f),
            Position = new Vector3(outputDimensions.X / 2, 500, -250),
            AttenuationA = 1e-6f,
            AttenuationC = 1f,
        },
        new Light
        {
            Color = Common.Color.White,
            Position = new Vector3(roomCenter.X, roomCenter.Y - 200, 0),
            AttenuationA = 1e-6f,
            AttenuationC = 0.7f,
        }
            ],
            AmbientLight = new Light
            {
                Color = new Common.Color(1, 1, 1),
                AttenuationC = 0.2f
            },
            BackgroundColor = new Common.Color(0.1f, 0.1f, 0.2f),
            RenderSettings = new RenderSettings(lineSkipStep: 1, maxRecursionDepth: 5),
        };

        scene.Camera = new Camera(
            position: new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z - (2 * halfSize)),
            lookAt: roomCenter,
            up: new Vector3(0, -1, 0),
            imageWidth: scene.ImageSize.X,
            imageHeight: scene.ImageSize.Y,
            fieldOfView: 60,
            sampleCount: 5);

        var triangles = ObjImporter.LoadFromFile(@"C:\Users\David\Downloads\final_scene_exported\Untitled.obj");
        scene.AddTriangles(triangles);

        var redMaterial = new Material(Common.Color.Red, 0.2f, 20, 0f);
        var greenMaterial = new Material(Common.Color.Green, 0.2f, 20, 0f);
        var whiteMaterial = new Material(Common.Color.White, 0f, 20, 0f);
        var glassMaterial = new Material(new Common.Color(0.8f, 0.8f, 1.0f), 0.1f, 50, 0.8f, 10.5f);
        var cyanMaterial = new Material(Common.Color.Cyan, 0.2f, 20, 0f);
        var yellowMaterial = new Material(Common.Color.Yellow, 0.2f, 20, 0f);

        scene.AddSphere(new Vector3(550, 250, 300), 150, new Material(new Common.Color(0f, 0.1f, 0f), 0.3f, 20, transparency: 0.95f, refractionIndex: 6f));
        scene.AddSphere(new Vector3(300, 370, 250), 50, cyanMaterial);

        // Right wall (yellow)
        scene.AddRectangle(Common.Rectangle.CreateWall(
            new Vector3(roomCenter.X + halfSize, roomCenter.Y, roomCenter.Z),
            new Vector3(-1, 0, 0),
            roomSize,
            5,
            yellowMaterial));

        // Floor (white)
        scene.AddRectangle(Common.Rectangle.CreateWall(
            new Vector3(roomCenter.X, roomCenter.Y + halfSize, roomCenter.Z),
            new Vector3(0, -1, 0),
            roomSize,
            5,
            whiteMaterial));

        // Back wall (white)
        scene.AddRectangle(Common.Rectangle.CreateWall(
            new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z + halfSize),
            new Vector3(0, 0, -1),
            roomSize,
            50,
            greenMaterial));

        return scene;
    }

    public static Scene CreateCornellBoxScene()
    {
        var scene = new Scene(new Vector2(1280f / 2, 720f / 2))
        {
            AmbientLight = new Light
            {
                Color = new Color(1, 1, 1),
                AttenuationC = 0.2f
            },
            BackgroundColor = Color.Black
        };

        // Room dimensions
        float roomSize = 800f;
        float halfSize = roomSize / 2f;
        var roomCenter = new Vector3(1000, 500, 500);

        scene.Camera = new Camera(
            position: new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z - (2 * halfSize)),
            lookAt: new Vector3(1000, 500, 500),
            up: new Vector3(0, -1, 0),
            imageWidth: scene.ImageSize.X,
            imageHeight: scene.ImageSize.Y,
            fieldOfView: 60,
            sampleCount: 1);

        // Add diffuse lights
        scene.DiffusedLights.Add(new Light
        {
            Position = new Vector3(roomCenter.X, roomCenter.Y - 200, 0),
            Color = Color.White,
            AttenuationA = 1e-6f,
            AttenuationC = 0.7f,
        });

        // Materials for walls
        var redMaterial = new Material(Color.Red, 0.2f, 20, 0f);
        var greenMaterial = new Material(Color.Green, 0.2f, 20, 0f);
        var whiteMaterial = new Material(Color.White, 0f, 20, 0f);
        var glassMaterial = new Material(new Color(0.8f, 0.8f, 1.0f), 0.1f, 50, 0.8f, 10.5f);
        var cyanMaterial = new Material(Color.Cyan, 0.2f, 20, 0f);
        var yellowMaterial = new Material(Color.Yellow, 0.2f, 20, 0f);

        // Create the walls of the Cornell box

        // Back wall (transparent white)
        scene.AddRectangle(Rectangle.CreateWall(
            new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z + halfSize),
            new Vector3(0, 0, -1),
            roomSize,
            50,
            glassMaterial));

        // Left wall (purple)
        scene.AddRectangle(Rectangle.CreateWall(
            new Vector3(roomCenter.X - halfSize, roomCenter.Y, roomCenter.Z),
            new Vector3(1, 0, 0),
            roomSize,
            5,
            redMaterial));

        // Right wall (yellow)
        scene.AddRectangle(Rectangle.CreateWall(
            new Vector3(roomCenter.X + halfSize, roomCenter.Y, roomCenter.Z),
            new Vector3(-1, 0, 0),
            roomSize,
            5,
            yellowMaterial));

        // Floor (white)
        scene.AddRectangle(Rectangle.CreateWall(
            new Vector3(roomCenter.X, roomCenter.Y + halfSize, roomCenter.Z),
            new Vector3(0, -1, 0),
            roomSize,
            5,
            whiteMaterial));

        // Ceiling (cyan)
        scene.AddRectangle(Rectangle.CreateWall(
            new Vector3(roomCenter.X, roomCenter.Y - halfSize, roomCenter.Z),
            new Vector3(0, 1, 0),
            roomSize,
            5,
            cyanMaterial));

        // Add spheres

        // White reflective sphere
        scene.AddSphere(
            new Vector3(roomCenter.X - (halfSize * 0.3f), roomCenter.Y + (halfSize * 0.5f), roomCenter.Z),
            100,
            new Material(Color.White, 0.7f, 50, 0f));

        // Transmissive sphere
        scene.AddSphere(
            new Vector3(roomCenter.X + (halfSize * 0.3f), roomCenter.Y + (halfSize * 0.5f), 600),
            100,
            new Material(Color.Red, 0f, 100, 0.9f, 1.2f));

        // Sphere behind transmissive sphere
        scene.AddSphere(
            new Vector3(roomCenter.X + (halfSize * 0.3f), roomCenter.Y + (halfSize * 0.5f), 800),
            50,
            new Material(Color.Green, 0f, 20, 0f));

        // Small cyan sphere
        scene.AddSphere(
            new Vector3(roomCenter.X, roomCenter.Y + (halfSize * 0.7f), roomCenter.Z - (halfSize * 0.3f)),
            50,
            new Material(Color.Cyan, 0f, 20, 0f));

        // Blue sphere in the back
        scene.AddSphere(
            new Vector3(roomCenter.X, 500, 1000),
            100,
            new Material(Color.Blue, 0f, 20, 0f));

        return scene;
    }
}
