using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTune
{
  public partial class AutoTune : Form
  {
    public static AutoTune autotune;
    private enum AppStates { Idle, ParsingLog };
    private BackgroundWorker worker;
    private string fileName;
    private bool dualTB;

    public List<double> maf_volts = new List<double>
    {
      0.08, 0.16, 0.23, 0.31, 0.39, 0.47, 0.55, 0.63, 0.70, 0.78, 0.86, 0.94, 1.02, 1.09, 1.17, 1.25, 1.33,
      1.41, 1.48, 1.56, 1.64, 1.72, 1.80, 1.88, 1.95, 2.03, 2.11, 2.19, 2.27, 2.34, 2.42, 2.50, 2.66, 2.73,
      2.81, 2.89, 2.97, 3.05, 3.13, 3.20, 3.28, 3.36, 3.44, 3.52, 3.59, 3.67, 3.75, 3.83, 3.91, 3.98, 4.06,
      4.14, 4.22, 4.30, 4.38, 4.45, 4.53, 4.61, 4.69, 4.77, 4.84, 4.92, 5.00
    };

    private List<Tuple<double, int, int>> MafB1 = new List<Tuple<double, int, int>>();
    private List<Tuple<double, int, int>> MafB2 = new List<Tuple<double, int, int>>();
    private List<Int32> MAFb1UserInput = new List<Int32>(63);
    private List<Int32> MAFb2UserInput = new List<Int32>(63);
    private List<Int32> AdjustMAFb1 = new List<Int32>(63);
    private List<Int32> AdjustMAFb2 = new List<Int32>(63);


    public static List<string> mafHeaders = new List<string>
      { "Time", "A/F CORR-B1 (%)", "A/F CORR-B2 (%)",
      "ACCEL PED POS 1 (V-Accel)", "MAS A/F -B1 (V)",
      "MAS A/F -B2 (V)", "INTAKE AIR TMP", "TARGET AFR" };

    public AutoTune()
    {
      InitializeComponent();
      SetAppState(AppStates.Idle, null);
      autotune = this;
    }

    private void openFileToolStripMenuItem_Open_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      openFileDialog.Filter = "csv Files (*.csv)|*.csv|All Files (*.*)|*.*";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK)
      {
        this.fileName = openFileDialog.FileName;
        FileInfo fi = new FileInfo(openFileDialog.FileName);
        SetAppState(AppStates.ParsingLog, fi.Name);
        StatusBox.Text = "File Loaded: " + fileName;
        if (fileName.Contains(".csv"))
        {
          StatusBox.Text = this.fileName;
          try
          {
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync(openFileDialog.FileName);
          }
          catch
          {
            SetAppState(AppStates.Idle, null);
            Console.WriteLine(" ERROR! ");
          }
        }
      }
    }

    private void SetAppState(AppStates newState, string filename)
    {
      switch (newState)
      {
        case AppStates.Idle:
          SetFileReadWidgetsVisible(false);
          StatusBox.Visible = true;
          ProgressBar.Visible = false;
          break;

        case AppStates.ParsingLog:
          SetFileReadWidgetsVisible(true);
          ProgressBar.Text = string.Format("Reading file: {0}", filename);
          StatusBox.Visible = false;
          break;
      }
    }

    void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
      Parser parser = new Parser();
      BackgroundWorker bw = sender as BackgroundWorker;
      string sFileToRead = (string)e.Argument;
      e.Result = parser.ParseLog(bw, sFileToRead);
      if (bw.CancellationPending)
        e.Cancel = true;
    }

    void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      try
      {
        if (e.Error != null)
        {
          MessageBox.Show(e.Error.Message, "Error During File Read");
        }
        else if (e.Cancelled)
        {
          StatusBox.Text = "** Cancelled **";
        }
        else
        {
          buffDV1.DataSource = null;
          buffDV1.DataSource = (DataTable)e.Result;
          if (buffDV1.RowCount > 50)
          {
            buffDV1.Visible = true;
            buffDVmaf1.Visible = true;
            try
            {
              if (buffDV1.Columns.Contains("MAS A/F -B2 (V)"))
              {
                tabPage2.Visible = true;
                Console.WriteLine("User log is dual TB.");
                dualTB = true;
              }
            }
            catch
            { }
            setupMAFgrids();
          }
        }
      }
      finally
      {
        ProgressBar.Value = 0;
        SetAppState(AppStates.Idle, null);
      }
    }

    private void setupMAFgrids()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Volts", typeof(double));
      dt.Columns.Add("Values", typeof(int));
      dt.Columns.Add("Adjustments", typeof(int));

      foreach (double d in maf_volts)
        dt.Rows.Add(d);

      buffDVmaf1.Rows.Clear();
      buffDVmaf1.Columns.Clear();
      buffDVmaf1.DataSource = null;
      buffDVmaf1.DataSource = dt;
      buffDVmaf1.Columns[0].Width = 40;
      buffDVmaf1.Columns[0].ReadOnly = true;
      buffDVmaf1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
      buffDVmaf1.Columns[0].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      buffDVmaf1.Columns[1].Width = 50;
      buffDVmaf1.Columns[1].ReadOnly = false;
      buffDVmaf1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
      buffDVmaf1.Columns[2].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      buffDVmaf1.Columns[2].Width = 80;
      buffDVmaf1.Columns[2].ReadOnly = true;
      buffDVmaf1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
      buffDVmaf1.Columns[2].Resizable = System.Windows.Forms.DataGridViewTriState.False;

      if (dualTB)
      {
        DataTable dt2 = new DataTable();
        dt2.Columns.Add("Volts", typeof(double));
        dt2.Columns.Add("Values", typeof(int));
        dt2.Columns.Add("Adjustments", typeof(int));

        foreach (double d in maf_volts)
          dt2.Rows.Add(d);

        buffDVmaf2.Rows.Clear();
        buffDVmaf2.Columns.Clear();
        buffDVmaf2.DataSource = null;
        buffDVmaf2.DataSource = dt2;
        buffDVmaf2.Columns[0].Width = 40;
        buffDVmaf2.Columns[0].ReadOnly = true;
        buffDVmaf2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        buffDVmaf2.Columns[0].Resizable = System.Windows.Forms.DataGridViewTriState.False;
        buffDVmaf2.Columns[1].Width = 50;
        buffDVmaf2.Columns[1].ReadOnly = false;
        buffDVmaf2.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        buffDVmaf2.Columns[1].Resizable = System.Windows.Forms.DataGridViewTriState.False;
        buffDVmaf2.Columns[2].Width = 80;
        buffDVmaf2.Columns[2].ReadOnly = true;
        buffDVmaf2.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        buffDVmaf2.Columns[2].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      }
    }


    private void SetFileReadWidgetsVisible(bool visible)
    {
      ProgressBar.Visible = visible;
      btnCancelParse.Visible = visible;
    }

    void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      ProgressBar.Value = e.ProgressPercentage;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      worker.CancelAsync();
    }

    private void tabPage2_Enter(object sender, EventArgs e)
    {
      if (buffDV1.RowCount > 50)
      {
        tab2Loader(true);
      }
    }

    private void tabPage2_Leave(object sender, EventArgs e)
    {
      tab2Loader(false);
    }

    private void tab2Loader(bool status)
    {
      textBox_MAF1.Visible = status;
      buffDVmaf1.Visible = status;
      if (dualTB)
      {
        textBox_MAF2.Visible = status;
        buffDVmaf2.Visible = status;
      }
    }

    private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (buffDV1.Columns.Count < 50)
        return;
      if (MessageBox.Show("Are you sure you want to close this log?",
          "Close Log",
          MessageBoxButtons.YesNo) == DialogResult.No)
      {
        return;
      }

      buffDV1.DataSource = null;
      buffDV1.Refresh();

      buffDVmaf1.DataSource = null;
      buffDVmaf1.Refresh();

      buffDVmaf2.DataSource = null;
      buffDVmaf2.Refresh();

      tab2Loader(false);
    }

    private void fileToolStripMenuItem_Exit_Click(object sender, EventArgs e)
    {
      Application.ExitThread();
      Application.Exit();
    }

    private void btn_ScaleMAF_Click(object sender, EventArgs e)
    {
      //parser.PopulateMAFgrid();
    }

    private void buffDVmaf1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
      if (buffDVmaf1.CurrentCell.ColumnIndex == 1)
      {
        TextBox tb = e.Control as TextBox;
        if (tb != null)
        {
          tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
        }
      }
    }

    private void buffDVmaf2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
      if (buffDVmaf2.CurrentCell.ColumnIndex == 1)
      {
        TextBox tb = e.Control as TextBox;
        if (tb != null)
        {
          tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
        }
      }
    }

    private void Column1_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!char.IsControl(e.KeyChar) 
        && !char.IsDigit(e.KeyChar)
        && e.KeyChar != '.')
      {
        e.Handled = true;
      }
    }

    private void selectAllValues()
    {

    }

    private void copyValue()
    {
      if (buffDVmaf1.SelectedRows.Count == 1)
      {
        
      }
      else
      {
        
      }
      //Clipboard.SetDataObject((object)this .  CellHex(0U, 0U), true);
    }

    private void pasteValue()
    {
      string tmpstr = ((string)Clipboard.GetDataObject().GetData(DataFormats.StringFormat)).Trim().Split(' ')[0].Trim();
      if (tmpstr.Length != 4 )
        return;
      //after checking values == 4, take hex value and convert to int. 
    }

    private void buffDVmaf_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Control)
      {
        switch (e.KeyCode)
        {
          case Keys.A:
            this.selectAllValues();
            e.Handled = true;
            break;
          case Keys.C:
            this.copyValue();
            e.Handled = true;
            break;
          case Keys.V:
            this.pasteValue();
            e.Handled = true;
            break;
        }
      }
    }

    private void buffDVmaf2_Leave(object sender, EventArgs e)
    {
      if (buffDVmaf2.RowCount > 0 && buffDVmaf2.ColumnCount > 0)
      {
        buffDVmaf2.CurrentCell = this.buffDVmaf2[0, 0];
        this.buffDVmaf2.CurrentCell.Selected = false;
      }
    }

    private void buffDVmaf1_Leave(object sender, EventArgs e)
    {
      if (buffDVmaf1.RowCount > 0 && buffDVmaf1.ColumnCount > 0)
      {
        buffDVmaf1.CurrentCell = this.buffDVmaf1[0, 0];
        this.buffDVmaf1.CurrentCell.Selected = false;
      }
    }
  }
}
