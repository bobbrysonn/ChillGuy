using ChillGuy.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
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
        private Size originalSize;

        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(200, 200, 1200, 800));
            ViewModel = new MainViewModel();
        }

        private void Button_Add(object sender, RoutedEventArgs e)
        {
            var chillGuyImage = new Image
            {
                Source = new BitmapImage(new("ms-appx:///Assets/ChillGuy.png")),
                Width = 400,
                Height = 400,
                Margin = new Thickness(90, 100, 0, 0)
            };
            chillGuyImage.PointerMoved += (s, e) =>
            {
                if (isDragging)
                {
                    chillGuyImage.Margin = new Thickness(
                        dragStartMargin.X + (e.GetCurrentPoint(ChillGuyCanvas).Position.X - dragStartPoint.X),
                        dragStartMargin.Y + (e.GetCurrentPoint(ChillGuyCanvas).Position.Y - dragStartPoint.Y), 0, 0);
                }
            };
            chillGuyImage.PointerPressed += (s, e) =>
            {
                isDragging = true;
                dragStartPoint = e.GetCurrentPoint(ChillGuyCanvas).Position;
                dragStartMargin = new Point(chillGuyImage.Margin.Left, chillGuyImage.Margin.Top);
                originalSize = new Size(chillGuyImage.Width, chillGuyImage.Height);
                chillGuyImage.CapturePointer(e.Pointer);
            };
            chillGuyImage.PointerReleased += (s, e) =>
            {
                isDragging = false;
                chillGuyImage.ReleasePointerCapture(e.Pointer);
            };

            ChillGuyCanvas.Children.Add(chillGuyImage);
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            ChillGuyCanvas.Children.Clear();
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            var parent = thumb.Parent as FrameworkElement;

            if (thumb != null && parent != null)
            {
                // Resize logic
                double newWidth = parent.Width + e.HorizontalChange;
                double newHeight = parent.Height + e.VerticalChange;

                // Prevent resizing to negative dimensions
                if (newWidth > 50) // Minimum Width
                    parent.Width = newWidth;

                if (newHeight > 50) // Minimum Height
                    parent.Height = newHeight;
            }
        }

        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;

            if (thumb != null)
            {
                // Center of the image (relative to the parent Grid)
                double centerX = Wireframe.Width / 2;
                double centerY = Wireframe.Height / 2;

                // Get the position of the Thumb relative to the parent
                var position = thumb.TransformToVisual(Wireframe).TransformPoint(new Windows.Foundation.Point(e.HorizontalChange, e.VerticalChange));

                // Calculate the angle between the center and the Thumb's position
                double angle = Math.Atan2(position.Y - centerY, position.X - centerX) * 180 / Math.PI;

                // Apply the rotation
                ImageRotateTransform.Angle = angle;
            }
        }
    }
}
