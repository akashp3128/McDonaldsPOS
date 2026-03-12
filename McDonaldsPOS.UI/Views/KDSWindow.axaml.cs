using Avalonia.Controls;
using McDonaldsPOS.UI.ViewModels;

namespace McDonaldsPOS.UI.Views;

public partial class KDSWindow : Window
{
    public KDSWindow()
    {
        InitializeComponent();

        Opened += Window_Opened;
        Closing += Window_Closing;
    }

    private void Window_Opened(object? sender, System.EventArgs e)
    {
        if (DataContext is KDSViewModel vm)
        {
            vm.StartRefresh();
        }
    }

    private void Window_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is KDSViewModel vm)
        {
            vm.StopRefresh();
        }
    }
}
