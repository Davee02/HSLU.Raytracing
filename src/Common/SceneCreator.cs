using Common.Objects;
using System.Numerics;

namespace Common;
public static class SceneCreator
{
    public static Scene CreateChurchScene()
    {
        var outputDimensions = new Vector2(1280, 720);
        var loc = new Vector3(0, 0, -100);

        var scene = new Scene(outputDimensions)
        {
            DiffusedLights =
            [
                new Light
                {
                    Color = new Common.Color(1 ,1 ,1),
                    Position = loc,
                    //AttenuationA = 1e-6f,
                    AttenuationC = 1f,
                },
                new Light
                {
                    Color = new Common.Color(1 ,1 ,1),
                    Position = new Vector3(0, 120, -100),
                    //AttenuationA = 1e-6f,
                    AttenuationC = 1f,
                },
            ],
            AmbientLight = new Light
            {
                Color = new Common.Color(1, 1, 1),
                AttenuationC = 0.2f
            },
            BackgroundColor = new Common.Color(0.1f, 0.1f, 0.2f),
            RenderSettings = new RenderSettings(lineSkipStep: 1, maxRecursionDepth: 5),
        };

        scene.Camera = Camera.FromLookAt(
            position: loc,
            lookAt: new Vector3(0, 0, 1),
            up: new Vector3(0, -1, 0),
            imageWidth: scene.ImageSize.X,
            imageHeight: scene.ImageSize.Y,
            fieldOfView: 60,
            sampleCount: 1);

        var triangles = ObjImporter.LoadFromFile(@"C:\Users\David\Downloads\church\church.obj");
        scene.AddTriangles(triangles);

        return scene;
    }

    public static Scene CreateContestScene()
    {
        var outputDimensions = new Vector2(1280, 720);

        var scene = new Scene(outputDimensions)
        {
            DiffusedLights =
            [
                new Light
                {
                    Color = new Common.Color(1 ,1 ,1),
                    Position = new Vector3(outputDimensions.X / 2 +200, outputDimensions.Y / 2 + 450, -100),
                    //AttenuationA = 1e-6f,
                    AttenuationC = 1f,
                },
            ],
            AmbientLight = new Light
            {
                Color = new Common.Color(1, 1, 1),
                AttenuationC = 0.2f
            },
            BackgroundColor = new Common.Color(0.1f, 0.1f, 0.2f),
            RenderSettings = new RenderSettings(lineSkipStep: 1, maxRecursionDepth: 5),
        };

        scene.Camera = Camera.FromLookAt(
            position: new Vector3(outputDimensions.X / 2, 600, -10),
            lookAt: new Vector3(outputDimensions.X / 2, 600, 0),
            up: new Vector3(0, -1, 0),
            imageWidth: scene.ImageSize.X,
            imageHeight: scene.ImageSize.Y,
            fieldOfView: 60,
            sampleCount: 1);

        var triangles = ObjImporter.LoadFromFile(@"C:\Users\David\Downloads\suzanne\Untitled.obj");
        scene.AddTriangles(triangles);
      

        return scene;
    }

    public static Scene CreateFinalScene()
    {
        var outputDimensions = new Vector2(1920, 1080);

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
                    AttenuationA = 2e-6f,
                    AttenuationC = 0.8f,
                },
                new Light
                {
                    Color = Common.Color.White,
                    Position = new Vector3(roomCenter.X, roomCenter.Y - 200, 0),
                    AttenuationA = 2e-6f,
                    AttenuationC = 0.6f,
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

        scene.Camera = Camera.FromLookAt(
            position: new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z - (2 * halfSize)),
            lookAt: roomCenter,
            up: new Vector3(0, -1, 0),
            imageWidth: scene.ImageSize.X,
            imageHeight: scene.ImageSize.Y,
            fieldOfView: 60,
            sampleCount: 5);

        var triangles = ObjImporter.LoadFromFile(@"C:\Users\David\Downloads\final_scene_exported\Untitled.obj");
        scene.AddTriangles(triangles);

        var greenMaterial = new Material(Common.Color.Green, 0.2f, 20, 0f);
        var whiteMaterial = new Material(Common.Color.White, 0f, 20, 0f);
        var cyanMaterial = new Material(Common.Color.Cyan, 0.2f, 20, 0f);
        var yellowMaterial = new Material(Common.Color.Yellow, 0.2f, 20, 0f);

        scene.AddSphere(new Vector3(550, 270, 300), 150, new Material(new Common.Color(0f, 0.1f, 0f), 0.3f, 20, transparency: 0.95f, refractionIndex: 6f));
        scene.AddSphere(new Vector3(300, 370, 250), 50, cyanMaterial);

        // Right wall (yellow)
        scene.AddRectangle(Objects.Rectangle.CreateWall(
            new Vector3(roomCenter.X + halfSize, roomCenter.Y, roomCenter.Z),
            new Vector3(-1, 0, 0),
            roomSize,
            5,
            yellowMaterial));

        // Floor (white)
        scene.AddRectangle(Objects.Rectangle.CreateWall(
            new Vector3(roomCenter.X, roomCenter.Y + halfSize, roomCenter.Z),
            new Vector3(0, -1, 0),
            roomSize,
            5,
            whiteMaterial));

        // Back wall (white)
        scene.AddRectangle(Objects.Rectangle.CreateWall(
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

        scene.Camera = Camera.FromLookAt(
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

    public static Scene CreateCrystalShowcaseScene()
    {
        var outputDimensions = new Vector2(1920, 1080);

        var scene = new Scene(outputDimensions)
        {
            RenderSettings = new RenderSettings(lineSkipStep: 1, maxRecursionDepth: 5), // Higher recursion for crystal effects
            BackgroundColor = new Color(0.02f, 0.03f, 0.08f), // Very dark blue-black
        };

        // Set up camera
        var cameraPosition = new Vector3(500, 150, -650);
        var lookAtPoint = new Vector3(500, 200, 400);

        scene.Camera = Camera.FromLookAt(
            position: cameraPosition,
            lookAt: lookAtPoint,
            up: new Vector3(0, -1, 0),
            fieldOfView: 40, // Narrower for more detail
            imageWidth: scene.ImageSize.X,
            imageHeight: scene.ImageSize.Y,
            sampleCount: 1);

        // Lighting
        scene.AmbientLight = new Light
        {
            Color = new Color(0.1f, 0.1f, 0.15f), // Subtle blue ambient
            AttenuationC = 0.15f
        };

        // Strategic colored lighting for dramatic effects
        scene.DiffusedLights = [
            // Main spotlight from above-right (warm)
            new Light
        {
            Position = new Vector3(700, -200, -300),
            Color = new Color(1.0f, 0.85f, 0.7f), // Warm light
            //AttenuationA = 0.00008f,
            //AttenuationB = 0.00001f,
            AttenuationC = 1.2f
        },
        // Blue backlight
        new Light
        {
            Position = new Vector3(500, 100, 800),
            Color = new Color(0.2f, 0.3f, 1.0f), // Strong blue
            //AttenuationA = 0.0001f,
            //AttenuationB = 0.00001f,
            AttenuationC = 0.8f
        },
        // Purple side light
        new Light
        {
            Position = new Vector3(-200, 100, 300),
            Color = new Color(0.8f, 0.2f, 1.0f), // Purple
            //AttenuationA = 0.0001f,
            //AttenuationB = 0.0001f,
            AttenuationC = 0.7f
        },
        // Green accent light
        new Light
        {
            Position = new Vector3(1200, 50, 300),
            Color = new Color(0.2f, 1.0f, 0.3f), // Green
            //AttenuationA = 0.0002f,
            //AttenuationB = 0.0001f,
            AttenuationC = 0.6f
        }
        ];

        // Materials
        // Highly reflective black surface for the base
        var displaySurfaceMaterial = new Material(
            new Color(0.05f, 0.05f, 0.05f), // Almost black
            reflectivity: 0.9f, // Very reflective
            shininess: 150,
            transparency: 0.0f);

        // Crystal materials with different properties
        // Clear crystal with high refraction
        var clearCrystalMaterial = new Material(
            new Color(0.95f, 0.95f, 0.98f),
            reflectivity: 0.15f,
            shininess: 120,
            transparency: 0.95f,
            refractionIndex: 1.8f);

        // Blue-tinted crystal
        var blueCrystalMaterial = new Material(
            new Color(0.7f, 0.8f, 1.0f),
            reflectivity: 0.1f,
            shininess: 100,
            transparency: 0.9f,
            refractionIndex: 1.65f);

        // Pink-tinted crystal
        var pinkCrystalMaterial = new Material(
            new Color(1.0f, 0.8f, 0.9f),
            reflectivity: 0.1f,
            shininess: 100,
            transparency: 0.92f,
            refractionIndex: 1.7f);

        // Yellow crystal with different refraction
        var yellowCrystalMaterial = new Material(
            new Color(1.0f, 0.95f, 0.6f),
            reflectivity: 0.12f,
            shininess: 110,
            transparency: 0.88f,
            refractionIndex: 1.5f);

        // Green crystal
        var greenCrystalMaterial = new Material(
            new Color(0.6f, 1.0f, 0.7f),
            reflectivity: 0.08f,
            shininess: 90,
            transparency: 0.9f,
            refractionIndex: 1.55f);

        // Water-like material for "liquid" effect
        var waterMaterial = new Material(
            new Color(0.7f, 0.8f, 1.0f),
            reflectivity: 0.1f,
            shininess: 80,
            transparency: 0.98f,
            refractionIndex: 1.33f); // Water refraction

        // Create display surface (reflective base)
        scene.AddRectangle(
            position: new Vector3(500, 350, 400),
            normal: new Vector3(0, -1, 0),
            up: new Vector3(0, 0, 1),
            width: 1000,
            height: 800,
            thickness: 10,
            material: displaySurfaceMaterial);

        // Central crystal formation (cluster of prisms)
        // Main crystal prism
        scene.AddCube(
            new Vector3(500, 220, 400),
            140, // Large central crystal
            new Vector3(30, 15, 45), // Rotated for interesting angles
            clearCrystalMaterial);

        // Smaller crystals around the central one
        scene.AddCube(
            new Vector3(420, 270, 350),
            80,
            new Vector3(10, 40, 25),
            blueCrystalMaterial);

        scene.AddCube(
            new Vector3(580, 290, 350),
            70,
            new Vector3(15, 30, 5),
            pinkCrystalMaterial);

        scene.AddCube(
            new Vector3(500, 260, 500),
            90,
            new Vector3(45, 10, 30),
            yellowCrystalMaterial);

        // Add some small scattered crystals
        scene.AddCube(
            new Vector3(300, 310, 250),
            50,
            new Vector3(5, 10, 15),
            greenCrystalMaterial);

        scene.AddCube(
            new Vector3(700, 320, 250),
            45,
            new Vector3(35, 20, 5),
            blueCrystalMaterial);

        scene.AddCube(
            new Vector3(600, 315, 600),
            55,
            new Vector3(25, 35, 45),
            pinkCrystalMaterial);

        scene.AddCube(
            new Vector3(400, 305, 600),
            60,
            new Vector3(20, 15, 30),
            yellowCrystalMaterial);

        // Spherical crystals for variety
        scene.AddSphere(
            new Vector3(300, 300, 500),
            70,
            clearCrystalMaterial);

        scene.AddSphere(
            new Vector3(700, 300, 500),
            65,
            waterMaterial);

        // Add small crystal spheres for accent
        for (int i = 0; i < 12; i++)
        {
            float angle = i * MathF.PI / 6;
            float radius = 320;
            float x = 500 + radius * MathF.Cos(angle);
            float z = 400 + radius * MathF.Sin(angle);

            // Alternate materials
            Material material = i % 4 == 0 ? clearCrystalMaterial :
                               i % 4 == 1 ? blueCrystalMaterial :
                               i % 4 == 2 ? pinkCrystalMaterial :
                                          yellowCrystalMaterial;

            scene.AddSphere(
                new Vector3(x, 325, z),
                25,
                material);
        }

        // Add a "water pool" effect in front
        scene.AddSphere(
            new Vector3(500, 340, 200), // Positioned partially below the surface
            120,
            waterMaterial);

        return scene;
    }
}
