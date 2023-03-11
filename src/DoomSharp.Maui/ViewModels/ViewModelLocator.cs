namespace DoomSharp.Maui.ViewModels;

public class ViewModelLocator
{
    public ConsoleViewModel ConsoleViewModel => ConsoleViewModel.Instance;
    public MainViewModel MainViewModel => MainViewModel.Instance;
}