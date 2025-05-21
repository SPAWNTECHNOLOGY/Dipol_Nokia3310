using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DipolNokia3310.ViewModels;

namespace DipolNokia3310.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}