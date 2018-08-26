using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buffer
{
  public class BuffDV : DataGridView
  {
    public BuffDV()
    {
      //InitializeComponent();
      DoubleBuffered = true;
    }
  }
}
