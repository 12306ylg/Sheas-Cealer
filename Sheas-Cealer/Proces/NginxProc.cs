using Sheas_Cealer.Consts;
using Sheas_Cealer.Utils;
using Sheas_Core;
using System;

namespace Sheas_Cealer.Proces;

internal class NginxProc : Proc, IDisposable
{
    internal NginxProc() : base(MainConst.NginxPath) { }

    protected override void OnExited(EventArgs e)
    {
        base.OnExited(e);
        NginxCleaner.Clean().Wait();
    }
}
