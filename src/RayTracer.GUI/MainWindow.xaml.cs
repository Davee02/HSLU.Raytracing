using Common;
using SixLabors.ImageSharp;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RayTracer.GUI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    const float movementStep = 50;
    const float rotationStep = MathF.PI / 10;

    private Vector3 cameraPosition = new(0, 0, -100);
    private Vector3 cameraRotation = new(0, 0, 0);
    private Scene scene = SceneCreator.CreateFinalScene();
    private float cameraFOVDegrees = 60.0f;

    // Flag to track if rendering is in progress
    private bool isRendering = false;

    public MainWindow()
    {
        InitializeComponent();

        scene.RenderSettings = new RenderSettings(lineSkipStep: 1, maxRecursionDepth: 2);

        RenderImage();
    }

    private async void RenderImage()
    {
        if (isRendering)
        {
            return;
        }

        try
        {
            // Set rendering flag and UI state
            isRendering = true;
            SetUIRenderingState(true);

            // Configure camera
            var defaultLookDirection = new Vector3(0, 0, 1);
            var defaultUpVector = new Vector3(0, -1, 0);
            var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(cameraRotation.Y, cameraRotation.X, cameraRotation.Z);
            var rotatedDirection = Vector3.Transform(defaultLookDirection, rotationMatrix);
            var rotatedUp = Vector3.Transform(defaultUpVector, rotationMatrix);
            scene.Camera = Camera.FromDirection(cameraPosition, rotatedDirection, rotatedUp, cameraFOVDegrees, scene.ImageSize.X, scene.ImageSize.Y);

            // Render on a background thread
            await Task.Run(() =>
            {
                scene.Render(progressCallback: UpdateRenderProgress);
            });

            // Display the rendered image
            var ms = new MemoryStream();
            scene.Bitmap.SaveAsBmp(ms);

            var img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = new MemoryStream(ms.ToArray());
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            img.Freeze();

            RenderedImage.Source = img;

            UpdateTextBoxes();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Rendering error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            // Reset UI state
            isRendering = false;
            SetUIRenderingState(false);
        }
    }

    // Updates the render progress display
    private void UpdateRenderProgress(float progress)
    {
        // Update UI on the UI thread
        Dispatcher.Invoke(() =>
        {
            // Convert to percentage
            int percentage = (int)(progress * 100);
            RenderStatusText.Text = $"Rendering: {percentage}% complete";

            // If we have percentage information, show a determinate progress bar
            if (progress > 0)
            {
                RenderProgress.IsIndeterminate = false;
                RenderProgress.Value = percentage;
            }
        });
    }

    // Sets UI to rendering or normal state
    private void SetUIRenderingState(bool isRendering)
    {
        // Update UI on UI thread
        Dispatcher.Invoke(() =>
        {
            // Show/hide rendering overlay
            RenderingOverlay.Visibility = isRendering ? Visibility.Visible : Visibility.Collapsed;

            // Reset progress if we're starting a new render
            if (isRendering)
            {
                RenderProgress.IsIndeterminate = true;
                RenderStatusText.Text = "Preparing to render...";
            }

            // Disable/enable control buttons during render
            var buttonsEnabled = !isRendering;

            // Movement buttons
            MoveLeft.IsEnabled = buttonsEnabled;
            MoveRight.IsEnabled = buttonsEnabled;
            MoveUp.IsEnabled = buttonsEnabled;
            MoveDown.IsEnabled = buttonsEnabled;
            MoveForth.IsEnabled = buttonsEnabled;
            MoveBack.IsEnabled = buttonsEnabled;

            // Rotation buttons
            RotateXPos.IsEnabled = buttonsEnabled;
            RotateXNeg.IsEnabled = buttonsEnabled;
            RotateYPos.IsEnabled = buttonsEnabled;
            RotateYNeg.IsEnabled = buttonsEnabled;
            RotateZPos.IsEnabled = buttonsEnabled;
            RotateZNeg.IsEnabled = buttonsEnabled;

            // FOV controls
            FOVAngle.IsEnabled = buttonsEnabled;
            FOVSlider.IsEnabled = buttonsEnabled;
            NarrowFOV.IsEnabled = buttonsEnabled;
            StandardFOV.IsEnabled = buttonsEnabled;
            WideFOV.IsEnabled = buttonsEnabled;

            // Other buttons
            Render.IsEnabled = buttonsEnabled;
            ResetCamera.IsEnabled = buttonsEnabled;
            ExportImage.IsEnabled = buttonsEnabled;

            // Update Render button text
            Render.Content = isRendering ? "RENDERING..." : "RENDER SCENE";
        });
    }

    private void UpdateTextBoxes()
    {
        XPosition.Text = cameraPosition.X.ToString();
        YPosition.Text = cameraPosition.Y.ToString();
        ZPosition.Text = cameraPosition.Z.ToString();

        XRotation.Text = cameraRotation.X.ToString();
        YRotation.Text = cameraRotation.Y.ToString();
        ZRotation.Text = cameraRotation.Z.ToString();

        FOVAngle.Text = cameraFOVDegrees.ToString();
        FOVSlider.Value = cameraFOVDegrees;
    }

    private void MoveDown_Click(object sender, RoutedEventArgs e)
    {
        cameraPosition.Y += movementStep;
        RenderImage();
    }

    private void MoveUp_Click(object sender, RoutedEventArgs e)
    {
        cameraPosition.Y -= movementStep;
        RenderImage();
    }

    private void MoveLeft_Click(object sender, RoutedEventArgs e)
    {
        cameraPosition.X -= movementStep;
        RenderImage();
    }

    private void MoveRight_Click(object sender, RoutedEventArgs e)
    {
        cameraPosition.X += movementStep;
        RenderImage();
    }

    private void MoveForth_Click(object sender, RoutedEventArgs e)
    {
        cameraPosition.Z += movementStep;
        RenderImage();
    }

    private void MoveBack_Click(object sender, RoutedEventArgs e)
    {
        cameraPosition.Z -= movementStep;
        RenderImage();
    }

    private void XPosition_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(XPosition.Text, out var x))
        {
            cameraPosition.X = x;
        }
    }

    private void YPosition_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(YPosition.Text, out var y))
        {
            cameraPosition.Y = y;
        }
    }

    private void ZPosition_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(ZPosition.Text, out var z))
        {
            cameraPosition.Z = z;
        }
    }

    private void Render_Click(object sender, RoutedEventArgs e)
    {
        RenderImage();
    }

    private void RotateXPos_Click(object sender, RoutedEventArgs e)
    {
        cameraRotation.X += rotationStep;
        RenderImage();
    }

    private void RotateXNeg_Click(object sender, RoutedEventArgs e)
    {
        cameraRotation.X -= rotationStep;
        RenderImage();
    }

    private void RotateYPos_Click(object sender, RoutedEventArgs e)
    {
        cameraRotation.Y += rotationStep;
        RenderImage();
    }

    private void RotateYNeg_Click(object sender, RoutedEventArgs e)
    {
        cameraRotation.Y -= rotationStep;
        RenderImage();
    }

    private void RotateZPos_Click(object sender, RoutedEventArgs e)
    {
        cameraRotation.Z += rotationStep;
        RenderImage();
    }

    private void RotateZNeg_Click(object sender, RoutedEventArgs e)
    {
        cameraRotation.Z -= rotationStep;
        RenderImage();
    }

    private void XRotation_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(XRotation.Text, out var x))
        {
            cameraRotation.X = x;
        }
    }

    private void YRotation_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(YRotation.Text, out var y))
        {
            cameraRotation.Y = y;
        }
    }

    private void ZRotation_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(ZRotation.Text, out var z))
        {
            cameraRotation.Z = z;
        }
    }

    private void ResetCamera_Click(object sender, RoutedEventArgs e)
    {
        cameraPosition = new Vector3(0, 0, -100);
        cameraRotation = new Vector3(0, 0, 0);
        cameraFOVDegrees = 60.0f;
        UpdateTextBoxes();
        RenderImage();
    }

    private void FOVSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        cameraFOVDegrees = (float)e.NewValue;
        UpdateTextBoxes();
    }

    private void NarrowFOV_Click(object sender, RoutedEventArgs e)
    {
        cameraFOVDegrees = 45.0f;
        RenderImage();
    }

    private void StandardFOV_Click(object sender, RoutedEventArgs e)
    {
        cameraFOVDegrees = 60.0f;
        RenderImage();

    }

    private void WideFOV_Click(object sender, RoutedEventArgs e)
    {
        cameraFOVDegrees = 90.0f;
        RenderImage();
    }

    private void FOVAngle_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(FOVAngle.Text, out var fov))
        {
            cameraFOVDegrees = fov;
            UpdateTextBoxes();
        }
    }

    private void ExportImage_Click(object sender, RoutedEventArgs e)
    {
        // Only allow export if there's an image to export
        if (RenderedImage.Source == null)
        {
            MessageBox.Show("No image available to export.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Create a save file dialog
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Title = "Save Ray-Traced Image",
            Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp",
            DefaultExt = ".png"
        };

        // Show the dialog and get result
        bool? result = saveFileDialog.ShowDialog();

        // Process save file dialog box result
        if (result == true)
        {
            try
            {
                // Get selected file path
                string filePath = saveFileDialog.FileName;

                // Get the file extension to determine format
                string extension = Path.GetExtension(filePath).ToLower();

                // Save in the appropriate format
                using var fileStream = new FileStream(filePath, FileMode.Create);

                // Get the image format based on extension
                switch (extension)
                {
                    case ".jpg":
                    case ".jpeg":
                        scene.Bitmap.SaveAsJpeg(fileStream);
                        break;
                    case ".png":
                        scene.Bitmap.SaveAsPng(fileStream);
                        break;
                    case ".bmp":
                        scene.Bitmap.SaveAsBmp(fileStream);
                        break;
                    default:
                        // Default to PNG if extension is unrecognized
                        scene.Bitmap.SaveAsPng(fileStream);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}