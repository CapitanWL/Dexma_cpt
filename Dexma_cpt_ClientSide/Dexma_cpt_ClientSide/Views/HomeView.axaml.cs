using Avalonia.Controls;
using Avalonia.Input;
using Dexma_cpt_CommonModels;

namespace Dexma_cpt_ClientSide.Views;

public partial class HomeView : UserControl
{

    public HomeView()
    {
        InitializeComponent();

    }

    private void StackPanel_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            if (sender is StackPanel stackPanel && stackPanel.DataContext is DecryptMessageModel selectedMessage)
            {
                if (selectedMessage.MessageFrom == App.User.Username)
                {

                    var contextMenu = stackPanel.ContextMenu as ContextMenu;
                    contextMenu.DataContext = this.DataContext;
                    contextMenu.IsEnabled = true;
                }
                else
                {
                    var contextMenu = stackPanel.ContextMenu as ContextMenu;
                    contextMenu.IsEnabled = false;
                }
            }
        }
    }
    /*
    private void TabControl_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        var tabControl = (TabControl)sender;
        var selectedTabItem = tabControl.SelectedItem as TabItem;

        if (selectedTabItem != null)
        {
            var scrollViewer = selectedTabItem.FindLogicalAncestorOfType<ScrollViewer>();

            if (scrollViewer != null)
            {
                
                scrollViewer.ScrollToEnd();
            }
        }
    }

    */



}