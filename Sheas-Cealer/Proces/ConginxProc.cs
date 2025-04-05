using Sheas_Cealer.Consts;
using Sheas_Cealer.Utils;
using Sheas_Core;
using System;

namespace Sheas_Cealer.Proces;

internal class ConginxProc : Proc, IDisposable
{
    internal ConginxProc() : base(MainConst.ConginxPath) { }

    protected override void OnExited(EventArgs e)
    {
        base.OnExited(e);
        _ = NginxCleaner.Clean();
    }
}
