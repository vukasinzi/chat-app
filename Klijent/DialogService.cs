using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

namespace Klijent;

public static class DialogService
{
    public static Task ShowMessageAsync(string message, string title = "Chat")
    {
        if (Dispatcher.UIThread.CheckAccess())
            return ShowMessageCoreAsync(message, title);

        var tcs = new TaskCompletionSource<object?>();
        Dispatcher.UIThread.Post(async () =>
        {
            try
            {
                await ShowMessageCoreAsync(message, title);
                tcs.TrySetResult(null);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });

        return tcs.Task;
    }

    private static async Task ShowMessageCoreAsync(string message, string title)
    {
        var dialog = new MessageDialog(title, message);
        var owner = GetOwner();

        if (owner is not null)
        {
            await dialog.ShowDialog(owner);
            return;
        }

        var tcs = new TaskCompletionSource<object?>();
        dialog.Closed += (_, _) => tcs.TrySetResult(null);
        dialog.Show();
        await tcs.Task;
    }

    private static Window? GetOwner()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return null;

        foreach (var window in desktop.Windows)
        {
            if (window.IsActive)
                return window;
        }

        return desktop.MainWindow;
    }
}
