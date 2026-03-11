using System.ComponentModel;
using System.Windows;
using McDonaldsPOS.UI.ViewModels;

namespace McDonaldsPOS.UI.Views;

/// <summary>
/// Kitchen Display System window
/// </summary>
public partial class KDSWindow : Window
{
    public KDSWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is KDSViewModel vm)
        {
            vm.StartRefresh();
        }
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (DataContext is KDSViewModel vm)
        {
            vm.StopRefresh();
        }
    }
}
