using ChillGuy.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
using System.Diagnostics;
using System.Numerics;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ChillGuy
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private bool isDragging;
        private Point dragStartPoint;
        private Point dragStartMargin;

        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(200, 200, 1200, 800));
            ViewModel = new MainViewModel();
        }

        /// <summary>
        /// Add a ChillGuy image to the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Add(object sender, RoutedEventArgs e)
        {
            var chillGuyImage = new Image
            {
                Source = new BitmapImage(new("ms-appx:///Assets/ChillGuy.png")),
                Width = 200,
                Height = 200,
                Margin = new Thickness(90, 100, 0, 0),
            };
            // Create resize handles
            var topLeftHandle = CreateResizeHandle(chillGuyImage, -8, -8, "TopLeft");
            var topRightHandle = CreateResizeHandle(chillGuyImage, chillGuyImage.Width - 8, -8, "TopRight");
            var bottomLeftHandle = CreateResizeHandle(chillGuyImage, -8, chillGuyImage.Height - 8, "BottomLeft");
            var bottomRightHandle = CreateResizeHandle(chillGuyImage, chillGuyImage.Width - 8, chillGuyImage.Height - 8, "BottomRight");

            // Add resize handles to the canvas
            ChillGuyCanvas.Children.Add(topLeftHandle);
            ChillGuyCanvas.Children.Add(topRightHandle);
            ChillGuyCanvas.Children.Add(bottomLeftHandle);
            ChillGuyCanvas.Children.Add(bottomRightHandle);

            chillGuyImage.PointerMoved += (s, e) =>
            {
                if (isDragging)
                {
                    chillGuyImage.Margin = new Thickness(
                        dragStartMargin.X + (e.GetCurrentPoint(ChillGuyCanvas).Position.X - dragStartPoint.X),
                        dragStartMargin.Y + (e.GetCurrentPoint(ChillGuyCanvas).Position.Y - dragStartPoint.Y), 0, 0);

                    // Move the handles as well
                    topLeftHandle.Margin = new Thickness(
                        (dragStartMargin.X - 8) + (e.GetCurrentPoint(ChillGuyCanvas).Position.X - dragStartPoint.X),
                        (dragStartMargin.Y - 8) + (e.GetCurrentPoint(ChillGuyCanvas).Position.Y - dragStartPoint.Y), 0, 0);
                    topRightHandle.Margin = new Thickness(
                        (dragStartMargin.X + chillGuyImage.Width - 8) + (e.GetCurrentPoint(ChillGuyCanvas).Position.X - dragStartPoint.X),
                        (dragStartMargin.Y - 8) + (e.GetCurrentPoint(ChillGuyCanvas).Position.Y - dragStartPoint.Y), 0, 0);
                    bottomLeftHandle.Margin = new Thickness(
                        (dragStartMargin.X - 8) + (e.GetCurrentPoint(ChillGuyCanvas).Position.X - dragStartPoint.X),
                        (dragStartMargin.Y + chillGuyImage.Height - 8) + (e.GetCurrentPoint(ChillGuyCanvas).Position.Y - dragStartPoint.Y), 0, 0);
                    bottomRightHandle.Margin = new Thickness(
                        (dragStartMargin.X + chillGuyImage.Width - 8) + (e.GetCurrentPoint(ChillGuyCanvas).Position.X - dragStartPoint.X),
                        (dragStartMargin.Y + chillGuyImage.Height - 8) + (e.GetCurrentPoint(ChillGuyCanvas).Position.Y - dragStartPoint.Y), 0, 0);
                }
            };
            chillGuyImage.PointerPressed += (s, e) =>
            {
                isDragging = true;
                dragStartPoint = e.GetCurrentPoint(ChillGuyCanvas).Position;
                dragStartMargin = new Point(chillGuyImage.Margin.Left, chillGuyImage.Margin.Top);
                chillGuyImage.CapturePointer(e.Pointer);

                // Reset the handles positions
                topLeftHandle.Margin = new Thickness(chillGuyImage.Margin.Left - 8, chillGuyImage.Margin.Top - 8, 0, 0);
                topRightHandle.Margin = new Thickness(chillGuyImage.Margin.Left + chillGuyImage.Width - 8, chillGuyImage.Margin.Top - 8, 0, 0);
                bottomLeftHandle.Margin = new Thickness(chillGuyImage.Margin.Left - 8, chillGuyImage.Margin.Top + chillGuyImage.Height - 8, 0, 0);
                bottomRightHandle.Margin = new Thickness(chillGuyImage.Margin.Left + chillGuyImage.Width - 8, chillGuyImage.Margin.Top + chillGuyImage.Height - 8, 0, 0);
            };
            chillGuyImage.PointerReleased += (s, e) =>
            {
                isDragging = false;
                chillGuyImage.ReleasePointerCapture(e.Pointer);
            };

            ChillGuyCanvas.Children.Add(chillGuyImage);
        }

        /// <summary>
        /// Delete all ChillGuy images from the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            ChillGuyCanvas.Children.Clear();
        }

        private Rectangle CreateResizeHandle(Image image, double left, double top, string name)
        {
            var handle = new Rectangle
            {
                Name = $"{name}ResizeHandle",
                Width = 16,
                Height = 15,
                Fill = new SolidColorBrush(Colors.White),
                Margin = new Thickness(image.Margin.Left + left, image.Margin.Top + top, 0, 0),
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 1
            };

            handle.PointerPressed += (s, e) =>
            {
                Debug.WriteLine("Pressed");
                handle.CapturePointer(e.Pointer);
                isDragging = true;
                dragStartPoint = e.GetCurrentPoint(ChillGuyCanvas).Position;
                dragStartMargin = new Point(image.Margin.Left, image.Margin.Top);
            };

            handle.PointerMoved += (s, e) =>

            {
                if (!isDragging)
                    return;

                var currentPoint = e.GetCurrentPoint(ChillGuyCanvas).Position;
                double deltaX = currentPoint.X - dragStartPoint.X;
                double deltaY = currentPoint.Y - dragStartPoint.Y;

                Vector3 vector3 = new((float)(image.Scale.X + deltaX/image.Width), (float)(image.Scale.Y + deltaY/image.Height), 1f);
                image.Scale = vector3;

                Debug.WriteLine($"Image width: {image.Width}, Image height: {image.Height}");

                // Move the handle as well
                handle.Margin = new Thickness(handle.Margin.Left + deltaX, handle.Margin.Top + deltaY, 0, 0);

                dragStartPoint = currentPoint;
            };

            handle.PointerReleased += (s, e) =>
            {
                isDragging = false;
                handle.ReleasePointerCapture(e.Pointer);
            };

            return handle;
        }
    }
}
