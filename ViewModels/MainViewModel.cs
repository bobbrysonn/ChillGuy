using Windows.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;

namespace ChillGuy.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private Brush _canvasBackground;

        [ObservableProperty]
        private Color _currentColor;

        public MainViewModel()
        {
            CurrentColor = Color.FromArgb(255, 141, 146, 123);
            CanvasBackground = new SolidColorBrush(CurrentColor);
        }

        partial void OnCurrentColorChanged(Color value)
        {
            CanvasBackground = new SolidColorBrush(value);
        }
    }
}
