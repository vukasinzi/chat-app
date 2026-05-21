using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Klijent;

public partial class MessageDialog : Window
{
    public MessageDialog()
    {
        InitializeComponent();
    }

    public MessageDialog(string title, string message)
        : this()
    {
        Title = title;
        DialogTitle.Text = title;
        DialogMessage.Text = message;
    }

    private void Ok_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
