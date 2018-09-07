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
  }
}
