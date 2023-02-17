using System;
using System.Windows;

using Scover.Dialogs;

namespace ExampleWpf;

public sealed partial class MainWindow : Window, IDisposable
{
    private readonly Page _page = new() { MainInstruction = "Blah blah blah" };

    public MainWindow()
    {
        InitializeComponent();
        //_ = new Dialog(_page).Show(/*new WindowInteropHelper(this).Handle*/);
    }

    public void Dispose() => _page.Dispose();

    private void Button_Click(object sender, RoutedEventArgs e) => _ = new Dialog(_page).Show(/*new WindowInteropHelper(this).Handle*/);

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        //_ = new Dialog(_page).Show(/*new WindowInteropHelper(this).Handle*/);
    }
}