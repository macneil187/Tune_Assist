using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTune
{
  public partial class OptionForm : Form
  {
    public OptionForm()
    {
      this.InitializeComponent();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      Properties.Settings.Default.Reset();
      this.Dispose();
    }

    private void buttonSave_Click(object sender, EventArgs e)
    {
      Properties.Settings.Default.Save();
      this.Dispose();
    }

    private void checkBoxClosedLoop_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkBoxClosedLoop.Checked)
      {
        this.checkBoxAccelChange.Checked = true; // Properties.Settings.Default.MAF_IAT = true;
        this.checkBoxAirTemp.Checked = true; //Properties.Settings.Default.MAF_ACCEL = true;
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
