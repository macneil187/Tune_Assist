namespace AutoTune
{
  using System;
  using System.Threading;
  using System.Windows.Forms;

  public partial class OptionForm : Form
  {
    private bool ClosedLoop;
    private bool OpenLoop;
    private bool FilterAirTemp;
    private bool FilterAccel;
    private bool MinimalChanges;

    public OptionForm()
    {
      this.InitializeComponent();
    }
  }
}
