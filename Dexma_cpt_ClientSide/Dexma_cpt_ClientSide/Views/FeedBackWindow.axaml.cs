using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Dexma_cpt_ClientSide;

public partial class FeedBackWindow : Window
{
    public FeedBackWindow(string message)
    {
        InitializeComponent();
        this.Find<TextBlock>("MessageText").Text = message;
    }

    public FeedBackWindow()
    {
        InitializeComponent();
    }

    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}