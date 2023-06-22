# WindowStateSaver.WinUi3

![](./images/icon.png)

This library provides a function that allows WinUi3 to write information about the position, size, and maximized state of the window when the application is exited in a Json file, and restore the previous state when the application is launched next time.

![Nuget](https://img.shields.io/nuget/dt/WindowStateSaver.WinUi3?label=Nuget&logo=Nuget&style=social) : [NuGet Gallery | WindowStateSaver.WinUi3](https://www.nuget.org/packages/EnkuToolkit.Wpf](https://www.nuget.org/packages/WindowStateSaver.WinUi3)

## How to Use.

What the user of the library must do is very simple. Simply call the RegisterAndLoad method in the constructor of MainWindow as shown below to save and restore the state.

MainWinodw.cs

```c#
namespace MyApp;

using Microsoft.UI.Xaml;
using WindowStateSaver.WinUi3;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowStateSaver.RegisterAndLoad(this); // <- Add
    }
}
```

