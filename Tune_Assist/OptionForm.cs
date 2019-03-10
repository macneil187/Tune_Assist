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

    private void Minimal_MAF_checkbox_CheckedChanged(object sender, EventArgs e)
    {
      Properties.Settings.Default.Maf_MINIMAL = this.Minimal_MAF_checkbox.Checked;
      Properties.Settings.Default.Save();
    }

    private void checkBoxAirTemp_CheckedChanged(object sender, EventArgs e)
    {
      Properties.Settings.Default.MAF_IAT = this.checkBoxAirTemp.Checked;
      Properties.Settings.Default.Save();
    }

    private void checkBoxAccelChange_CheckedChanged(object sender, EventArgs e)
    {
      Properties.Settings.Default.MAF_ACCEL = this.checkBoxAccelChange.Checked;
      Properties.Settings.Default.Save();
    }

    private void checkBoxClosedLoop_CheckedChanged(object sender, EventArgs e)
    {
      Properties.Settings.Default.MAF_CL = this.checkBoxClosedLoop.Checked;
      Properties.Settings.Default.Save();
    }

    private void checkBoxOpenLoop_CheckedChanged(object sender, EventArgs e)
    {
      Properties.Settings.Default.MAF_OL = this.checkBoxOpenLoop.Checked;
      Properties.Settings.Default.Save();
    }
  }
}
