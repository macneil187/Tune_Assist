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
      InitializeComponent();
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
      if (checkBoxClosedLoop.Checked)
      {
        checkBoxAccelChange.Checked = true; // Properties.Settings.Default.MAF_IAT = true;
        checkBoxAirTemp.Checked = true; //Properties.Settings.Default.MAF_ACCEL = true;
        checkBoxClosedLoop.Checked = true;
      }
      else
      {
        checkBoxAccelChange.Checked = false;
        checkBoxAirTemp.Checked = false;
        checkBoxClosedLoop.Checked = false;
      }
    }
  }
}
