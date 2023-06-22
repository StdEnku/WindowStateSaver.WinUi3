namespace Sandbox;

using Microsoft.UI.Xaml;
using WindowStateSaver.WinUi3;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowStateSaver.RegisterAndLoad(this);
    }
}
