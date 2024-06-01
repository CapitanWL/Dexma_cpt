using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Dexma_cpt_ClientSide;

public partial class MessageBoxWindow : Window
{
    public MessageBoxWindow(string message)
    {
        InitializeComponent();
        this.Find<TextBlock>("MessageText").Text = message;
    }

    public MessageBoxWindow()
    {
        InitializeComponent();
    }

        private void CloseWindow(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

}