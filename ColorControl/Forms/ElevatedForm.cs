using ColorControl.Svc;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl.Forms;

public partial class ElevatedForm : Form
{
    private ColorControlBackgroundService _backgroundService;

    public ElevatedForm(ColorControlBackgroundService backgroundService)
    {
        InitializeComponent();

        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        Load += ElevatedForm_Load;
        Shown += ElevatedForm_Shown;

        Task.Run(CheckMutexAsync);

        _backgroundService = backgroundService;
        _backgroundService.PipeName = "elevatedpipe";

        Task.Run(async () => await _backgroundService.StartAsync(CancellationToken.None));
    }

    private void ElevatedForm_Shown(object sender, EventArgs e)
    {
        Hide();
    }

    protected void ElevatedForm_Load(object sender, EventArgs e)
    {
        Size = new Size(0, 0);
    }

    private void CheckMutexAsync()
    {
        var mutex = new Mutex(false, Shared.Common.GlobalContext.CurrentContext.MutexId, out var created);
        try
        {
            mutex.WaitOne();
        }
        catch (AbandonedMutexException)
        {
        }
        BeginInvoke(Application.Exit);
    }
}
