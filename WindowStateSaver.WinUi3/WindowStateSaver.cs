/*
* MIT License
* 
* Copyright (c) 2023 StdEnku
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/
namespace WindowStateSaver.WinUi3;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Storage;
using WinRT.Interop;
using System.IO;
using System.Text.Json;
using Windows.Graphics;

/// <summary>
/// Class for recording the position and size of the Window and whether it is maximized when exiting, and automatically loading it the next time it is launched.
/// </summary>
public class WindowStateSaver
{
    private WeakReference<Window> _windowWeakReference;
    private WindowSaveData _saveData;
    private string _saveFileName;
    private const string DefaultSaveFileName = "WindowStateSaveData.json";

    /// <summary>
    /// Methods to register the target Window and load its previous state
    /// </summary>
    /// <remarks>
    /// It is assumed to be called in the constructor of the target Window class.
    /// </remarks>
    /// <param name="window">Object of the target Window class</param>
    /// <param name="saveFileName">Name of the json file to be used as the save file</param>
    /// <exception cref="ArgumentException">Thrown if the filename contains invalid characters</exception>
    public static void RegisterAndLoad(Window window, string saveFileName = DefaultSaveFileName)
    {
        var chars = Path.GetInvalidFileNameChars();

        if (saveFileName.IndexOfAny(chars) >= 0)
            throw new ArgumentException("The saveFileName argument of the WindowStateSaver.RegisterAndLoad method contains a path or invalid characters.", nameof(saveFileName));

        new WindowStateSaver(window, saveFileName);
    }

    private WindowStateSaver(Window window, string saveFileName)
    {
        _saveFileName = saveFileName;
        _windowWeakReference = new(window);
        _appWindow.Changed += OnAppWindowChanged;
        window.Closed += OnWindowClosed;
        _saveData = new();

        var jsonString = ReadSaveFile(saveFileName);
        if (jsonString is null) return;

        WindowSaveData? windowSaveData;
        try
        {
            windowSaveData = JsonSerializer.Deserialize<WindowSaveData>(jsonString);
        }
        catch
        {
            return;
        }

        if (windowSaveData is null) return;

        var size = new SizeInt32() { Height = windowSaveData.Height, Width = windowSaveData.Width };
        var pos = new PointInt32() { X = windowSaveData.X, Y = windowSaveData.Y };
        _appWindow.Resize(size);
        _appWindow.Move(pos);

        if (windowSaveData.IsMaximized) _overlappedPresenter.Maximize();

        _saveData = windowSaveData;
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        var jsonString = JsonSerializer.Serialize(_saveData);
        WriteSaveFile(_saveFileName, jsonString);
    }

    private void OnAppWindowChanged(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowChangedEventArgs args)
    {
        if (_overlappedPresenter.State == OverlappedPresenterState.Maximized)
        {
            _saveData.IsMaximized = true;
        }
        else
        {
            var size = _appWindow.Size;
            var pos = _appWindow.Position;

            _saveData.Height = size.Height;
            _saveData.Width = size.Width;
            _saveData.X = pos.X;
            _saveData.Y = pos.Y;
            _saveData.IsMaximized = false;
        }
    }

    private static string? ReadSaveFile(string saveFileName)
    {
        string? result;
        try
        {
            var localStorageFolder = ApplicationData.Current.LocalFolder;
            var getFileAsyncOperation = localStorageFolder.GetFileAsync(saveFileName);
            var getFileTask = getFileAsyncOperation.AsTask();
            getFileTask.Wait();
            var storageFile = getFileTask.Result;

            var readTextAsyncOperation = FileIO.ReadTextAsync(storageFile);
            var readTextAsyncTask = readTextAsyncOperation.AsTask();
            readTextAsyncTask.Wait();
            result = readTextAsyncTask.Result;
        }
        catch (InvalidOperationException)
        {
            try
            {
                result = File.ReadAllText(saveFileName);
            }
            catch (Exception)
            {
                return null;
            }
        }
        catch (Exception)
        {
            return null;
        }
        return result;
    }

    private static void WriteSaveFile(string saveFileName, string text)
    {
        try
        {
            var localStorageFolder = ApplicationData.Current.LocalFolder;
            var createFileAsyncOperation = localStorageFolder.CreateFileAsync(saveFileName, CreationCollisionOption.ReplaceExisting);
            var createFileTask = createFileAsyncOperation.AsTask();
            createFileTask.Wait();
            var storageFile = createFileTask.Result;

            var writeTextAsyncOperation = FileIO.WriteTextAsync(storageFile, text);
            var writeTextTask = writeTextAsyncOperation.AsTask();
            writeTextTask.Wait();
        }
        catch (InvalidOperationException)
        {
            File.WriteAllText(saveFileName, text);
        }
    }

    private class WindowSaveData
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public bool IsMaximized { get; set; }
    }

    private Window _window
    {
        get
        {
            if (_windowWeakReference.TryGetTarget(out var window)) return window;
            else throw new NullReferenceException("There is no reference to the Window object that is the target of WindowStateSaver.");
        }
    }

    private Microsoft.UI.Windowing.AppWindow _appWindow
    {
        get
        {
            var hWnd = WindowNative.GetWindowHandle(_window);
            var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return Microsoft.UI.Windowing.AppWindow.GetFromWindowId(wndId);
        }
    }

    private OverlappedPresenter _overlappedPresenter => (OverlappedPresenter)_appWindow.Presenter;
}