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
    private DataTable MAF1_DT = new DataTable();
    private DataTable MAF2_DT = new DataTable();

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
      buildMAF_DT();
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
            worker.DoWork += worker_ParseLog;
            worker.RunWorkerCompleted += worker_ParseLogCompleted;
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

    void worker_ParseLog(object sender, DoWorkEventArgs e)
    {
      Parser parser = new Parser();
      BackgroundWorker bw = sender as BackgroundWorker;
      string sFileToRead = (string)e.Argument;
      e.Result = parser.ParseLog(bw, sFileToRead);
      if (bw.CancellationPending)
        e.Cancel = true;
    }

    void worker_ParseLogCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                buffDVmaf2.Visible = true;
                this.dualTB = true;
                buffDVmaf2.Refresh();
              }
            }
            catch
            { }
          }
        }
      }
      finally
      {
        ProgressBar.Value = 0;
        SetAppState(AppStates.Idle, null);
      }
    }

    private void buildMAF_DT()
    {
      // MAF 1 DataTable
      MAF1_DT.Columns.Add("Volts", typeof(double));
      MAF1_DT.Columns.Add("Values", typeof(int));
      MAF1_DT.Columns.Add("Adjustments", typeof(int));

      foreach (double d in maf_volts)
        MAF1_DT.Rows.Add(d);
      
      buffDVmaf1.Rows.Clear();
      buffDVmaf1.Columns.Clear();
      buffDVmaf1.DataSource = null;
      buffDVmaf1.DataSource = MAF1_DT;
      buffDVmaf1.Columns["Volts"].Width = 40;
      buffDVmaf1.Columns["Volts"].ReadOnly = true;
      buffDVmaf1.Columns["Volts"].DefaultCellStyle.Format = "N2";
      buffDVmaf1.Columns["Volts"].SortMode = DataGridViewColumnSortMode.NotSortable;
      buffDVmaf1.Columns["Volts"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      buffDVmaf1.Columns["Values"].Width = 50;
      buffDVmaf1.Columns["Values"].ReadOnly = false;
      buffDVmaf1.Columns["Values"].DefaultCellStyle.Format = "d";
      buffDVmaf1.Columns["Values"].SortMode = DataGridViewColumnSortMode.NotSortable;
      buffDVmaf1.Columns["Adjustments"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      buffDVmaf1.Columns["Adjustments"].Width = 80;
      buffDVmaf1.Columns["Adjustments"].ReadOnly = true;
      buffDVmaf1.Columns["Adjustments"].DefaultCellStyle.Format = "d";
      buffDVmaf1.Columns["Adjustments"].SortMode = DataGridViewColumnSortMode.NotSortable;
      buffDVmaf1.Columns["Adjustments"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      
      // MAF 2 DataTable
      this.MAF2_DT = MAF1_DT.Copy();
      buffDVmaf2.DataSource = MAF2_DT;
      buffDVmaf2.Columns["Volts"].Width = 40;
      buffDVmaf2.Columns["Volts"].ReadOnly = true;
      buffDVmaf2.Columns["Volts"].DefaultCellStyle.Format = "N2";
      buffDVmaf2.Columns["Volts"].SortMode = DataGridViewColumnSortMode.NotSortable;
      buffDVmaf2.Columns["Volts"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      buffDVmaf2.Columns["Values"].Width = 50;
      buffDVmaf2.Columns["Values"].ReadOnly = false;
      buffDVmaf2.Columns["Values"].DefaultCellStyle.Format = "d";
      buffDVmaf2.Columns["Values"].SortMode = DataGridViewColumnSortMode.NotSortable;
      buffDVmaf2.Columns["Adjustments"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      buffDVmaf2.Columns["Adjustments"].Width = 80;
      buffDVmaf2.Columns["Adjustments"].ReadOnly = true;
      buffDVmaf2.Columns["Adjustments"].DefaultCellStyle.Format = "d";
      buffDVmaf2.Columns["Adjustments"].SortMode = DataGridViewColumnSortMode.NotSortable;
      buffDVmaf2.Columns["Adjustments"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
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
      Console.WriteLine("break"); //temp break
      //parser.PopulateMAFgrid();
    }

    private void buffDVmaf1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
      if (buffDVmaf1.CurrentCell.ColumnIndex ==
          buffDVmaf1.Columns["Values"].Index)
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
      if (buffDVmaf1.CurrentCell.ColumnIndex ==
          buffDVmaf2.Columns["Values"].Index)
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
      if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
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
    }

    private void pasteValue()
    {
      try
      {
        string tmpstr = ((string)Clipboard.GetDataObject().GetData(DataFormats.StringFormat)).Trim().Split(' ')[0].Trim();
        if (tmpstr.Length != 4)
          return;
        int rowIndex = buffDVmaf1.CurrentCellAddress.Y;
        Console.WriteLine(" rowindex is : " + rowIndex);
        Console.WriteLine(" tmpstr is : " + tmpstr);
        string[] lines = tmpstr.Split('\n');
        int iFail = 0, iRow = buffDVmaf1.CurrentCell.RowIndex;
        int iCol = buffDVmaf1.CurrentCell.ColumnIndex;
        DataGridViewCell oCell;
        foreach (string line in lines)
        {
          if (iRow < buffDVmaf1.RowCount && line.Length > 0)
          {
            string[] sCells = line.Split('\t');
            for (int i = 0; i < sCells.GetLength(0); ++i)
            {
              if (iCol + i < this.buffDVmaf1.ColumnCount)
              {
                oCell = buffDVmaf1[iCol + i, iRow];
                if (!oCell.ReadOnly)
                {
                  if (oCell.Value.ToString() != sCells[i])
                  {
                    oCell.Value = Convert.ChangeType(sCells[i],
                                          oCell.ValueType);
                    oCell.Style.BackColor = Color.Tomato;
                  }
                  else
                    iFail++;
                }
              }
              else
              { break; }
            }
            iRow++;
          }
          else
          { break; }
          if (iFail > 0)
            MessageBox.Show(string.Format("{0} updates failed due" +
                            " to read only column setting", iFail));
        }
      }
      catch (FormatException)
      {
        MessageBox.Show("The data you pasted is in the wrong format for the cell");
        return;
      }
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
    
    private void buffDVmaf_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
    {
      string tmpstr = Convert.ToString(e.Value);
      if (e != null && e.Value != null && e.DesiredType.Equals(typeof(int)))
      {
        try
        {
          int hex;
          if (tmpstr.EndsWith(" ") && tmpstr.Length ==5)
          {
            tmpstr = tmpstr.TrimEnd(' ');
            hex = int.Parse(tmpstr, System.Globalization.NumberStyles.HexNumber);
            e.Value = hex;
            e.ParsingApplied = true;
          }
          else if (tmpstr.Length < 3 || tmpstr.Length > 5)
          {
            e.Value = 0000;
            e.ParsingApplied = true;
          }
          else
          {
            e.Value = tmpstr;
            e.ParsingApplied = true;
          }
        }
        catch
        {
          Console.WriteLine("The data you pasted is in the wrong format for the cell");
        }
      }
      else
      {
        e.ParsingApplied = true;
      }
    }
  }
}