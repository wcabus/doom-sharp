using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DoomSharp.Core;
using DoomSharp.Windows.Annotations;

namespace DoomSharp.Windows.ViewModels;

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
#if DEBUG
        if (Debugger.IsAttached)
        {
            Debug.WriteLine(message.TrimEnd('\r', '\n'));
        }
#endif
        ConsoleOutput += message;
    }

    public void SetTitle(string title)
    {
        Title = $"{title} - Console output";
        MainViewModel.Instance.Title = title;
    }

    public void Shutdown()
    {
        try
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                Application.Current?.Shutdown();
            });
        }
        catch (TaskCanceledException) { }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}