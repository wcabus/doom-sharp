using System.ComponentModel;
using System.Runtime.CompilerServices;
using DoomSharp.Core;

namespace DoomSharp.Maui.ViewModels;

public class ConsoleViewModel : IConsole, INotifyPropertyChanged
{
    public static readonly ConsoleViewModel Instance = new();

    private ConsoleViewModel() {}

    private string _consoleOutput = "";
    private string _title = "DooM# - Console output";

    public string ConsoleOutput
    {
        get => _consoleOutput;
        set
        {
            _consoleOutput = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public void Write(string message)
    {
        ConsoleOutput += message;
    }

    public void SetTitle(string title)
    {
        Title = $"{title} - Console output";
        MainViewModel.Instance.Title = title;
    }

    public void Shutdown()
    {
        //TODO
        //Application.Current.Dispatcher.Invoke(() =>
        //{
        //    Application.Current.Shutdown();
        //});
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}