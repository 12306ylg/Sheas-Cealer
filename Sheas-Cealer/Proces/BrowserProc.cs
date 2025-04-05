using Sheas_Core;
using System;
using System.Windows;

namespace Sheas_Cealer.Proces;

internal class BrowserProc : Proc
{
    private readonly bool ShutDownAppOnProcessExit;

    internal BrowserProc(string browserPath, bool shutDownAppOnProcessExit) : base(browserPath)
    {
        ShutDownAppOnProcessExit = shutDownAppOnProcessExit;

        OnExited(EventArgs.Empty);
    }

    protected override void OnExited(EventArgs e)
    {
        base.OnExited(e);
        if (ShutDownAppOnProcessExit)
            Application.Current.Dispatcher.InvokeShutdown();
    }
}
