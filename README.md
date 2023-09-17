# WindowStateSaver.WinUi3

![](./images/icon.png)

This library provides a function that allows WinUi3 to write information about the position, size, and maximized state of the window when the application is exited in a Json file, and restore the previous state when the application is launched next time.

![Nuget](https://img.shields.io/nuget/dt/WindowStateSaver.WinUi3?label=Nuget&logo=Nuget&style=social) : [NuGet Gallery | WindowStateSaver.WinUi3](https://www.nuget.org/packages/WindowStateSaver.WinUi3)

## How to Use.

What the user of the library must do is very simple. Simply call the RegisterAndLoad method in the constructor of MainWindow as shown below to save and restore the state.

MainWinodw.xaml.cs

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
The save data file is a WindowStateSaveData.json file created directly under the local folder for Packaged, and a WindowStateSaveData.json file created in the same folder as the exe file for UnPackaged. file created in the same folder as the exe file in the case of UnPackaged.

## Third-Party-Notices
### WindowsAppSdk

[repository](https://github.com/microsoft/WindowsAppSDK/tree/main)

```text
MIT License
Copyright (c) Microsoft Corporation.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE
```
