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
      this.ClosedLoop = this.checkBoxClosedLoop.Checked;
      this.OpenLoop = this.checkBoxOpenLoop.Checked;
      this.FilterAirTemp = this.checkBoxAirTemp.Checked;
      this.FilterAccel = this.checkBoxAccelChange.Checked;
      this.MinimalChanges = this.Minimal_MAF_checkbox.Checked;
    }

    private void checkBoxClosedLoop_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkBoxClosedLoop.Checked)
      {
        this.checkBoxAccelChange.Checked = true;
        this.checkBoxAirTemp.Checked = true;
        this.checkBoxClosedLoop.Checked = true;
      }
      else
      {
        this.checkBoxAccelChange.Checked = false;
        this.checkBoxAirTemp.Checked = false;
        this.checkBoxClosedLoop.Checked = false;
      }
    }
  }
}
