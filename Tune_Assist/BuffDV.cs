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
