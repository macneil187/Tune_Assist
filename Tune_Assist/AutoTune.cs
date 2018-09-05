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
    private BackgroundWorker mafWorker;
    private string fileName;
    private bool dualTB;
    private DataTable MAF1_DT = new DataTable();
    private DataTable MAF2_DT = new DataTable();
    private TextBox TextBox1 = new TextBox();

    public List<double> maf_volts = new List<double>
    {
      0.08, 0.16, 0.23, 0.31, 0.39, 0.47, 0.55, 0.63, 0.70, 0.78, 0.86, 0.94, 1.02, 1.09, 1.17, 1.25, 1.33,
      1.41, 1.48, 1.56, 1.64, 1.72, 1.80, 1.88, 1.95, 2.03, 2.11, 2.19, 2.27, 2.34, 2.42, 2.50, 2.58, 2.66,
      2.73, 2.81, 2.89, 2.97, 3.05, 3.13, 3.20, 3.28, 3.36, 3.44, 3.52, 3.59, 3.67, 3.75, 3.83, 3.91, 3.98,
      4.06, 4.14, 4.22, 4.30, 4.38, 4.45, 4.53, 4.61, 4.69, 4.77, 4.84, 4.92, 5.00
    };

    private List<Tuple<double, int, int>> MafB1 = new List<Tuple<double, int, int>>();
    private List<Tuple<double, int, int>> MafB2 = new List<Tuple<double, int, int>>();
    public List<Int32> MAFb1UserInput = new List<Int32>(); //64
    public List<Int32> MAFb2UserInput = new List<Int32>();
    public List<Int32> AdjustMAFb1 = new List<Int32>();
    public List<Int32> AdjustMAFb2 = new List<Int32>();


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
          closeFileToolStripMenuItem.Enabled = true;
        }
      }
    }

    private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (buffDV1.Rows.Count < 50)
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
      closeFileToolStripMenuItem.Enabled = false;
    }

    private void fileToolStripMenuItem_Exit_Click(object sender, EventArgs e)
    {
      Application.ExitThread();
      Application.Exit();
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
          MessageBox.Show(e.Error.Message, "Error During File Read");
        else if (e.Cancelled)
          StatusBox.Text = "** Cancelled **";
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
              scaleMAF();
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

    void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      ProgressBar.Value = e.ProgressPercentage;
    }

    private void buildMAF_DT()
    {
      // MAF 1 DataTable
      MAF1_DT.Columns.Add("Volts", typeof(double));
      MAF1_DT.Columns.Add("Values", typeof(int));
      MAF1_DT.Columns.Add("Adjustments", typeof(int));
      MAF1_DT.Columns.Add("Multiplier", typeof(double));

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
      buffDVmaf1.Columns["Multiplier"].Width = 60;
      buffDVmaf1.Columns["Multiplier"].ReadOnly = true;
      buffDVmaf1.Columns["Multiplier"].DefaultCellStyle.Format = "N4";
      buffDVmaf1.Columns["Multiplier"].SortMode = DataGridViewColumnSortMode.NotSortable;

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
      buffDVmaf2.Columns["Multiplier"].Width = 55;
      buffDVmaf2.Columns["Multiplier"].ReadOnly = true;
      buffDVmaf2.Columns["Multiplier"].DefaultCellStyle.Format = "N4";
      buffDVmaf2.Columns["Multiplier"].SortMode = DataGridViewColumnSortMode.NotSortable;
    }

    private void scaleMAF()
    {
      //MAF1
      MAFb1UserInput.Clear();
      if (buffDVmaf1 != null)
      {
        for (int r = 0; r < buffDVmaf1.RowCount; ++r)
        {
          string teststr = Convert.ToString(buffDVmaf1["Values", r].Value);
          if (teststr != "")
            MAFb1UserInput.Add(Convert.ToInt32(teststr));
          else
          {
            MAFb1UserInput.Clear();
            break;
          }
        }
      }
      bool maf1Ready = this.MAFb1UserInput.Count == 64 ? true : false;
      //MAF2
      MAFb2UserInput.Clear();
      if (buffDVmaf2 != null)
      {
        for (int r = 0; r < buffDVmaf2.RowCount; ++r)
        {
          string teststr = Convert.ToString(buffDVmaf2["Values", r].Value);
          if (teststr != "")
          {
            MAFb2UserInput.Add(Convert.ToInt32(teststr));
          }
          else
          {
            MAFb2UserInput.Clear();
            break;
          }
        }
      }
      bool maf2Ready = this.MAFb1UserInput.Count == 64 ? true : false;
      //Send it!
      try
      {
        mafWorker = new BackgroundWorker();
        mafWorker.DoWork += mafWorker_FindAdjust;
        mafWorker.RunWorkerCompleted += mafWorker_Completed;
        mafWorker.WorkerReportsProgress = true;
        mafWorker.WorkerSupportsCancellation = true;
        mafWorker.ProgressChanged += mafWorker_ProgressChanged;
        mafWorker.RunWorkerAsync(buffDV1);
      }
      catch
      {
        SetAppState(AppStates.Idle, null);
        Console.WriteLine(" ERROR! ");
      }
    }

    void mafWorker_FindAdjust(object sender, DoWorkEventArgs e)
    {
      MAF_Scaling maf_scaler = new MAF_Scaling();
      BackgroundWorker bw = sender as BackgroundWorker;
      DataGridView tempgrid = (DataGridView)e.Argument;
      e.Result = maf_scaler.AdjustMAF_CL(bw, tempgrid);
      if (bw.CancellationPending)
        e.Cancel = true;
    }

    void mafWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      ProgressBar.Value = e.ProgressPercentage;
    }

    void mafWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
    {
      try
      {
        if (e.Error != null)
          MessageBox.Show(e.Error.Message, "Error During Maf Adjustments");
        else if (e.Cancelled)
          StatusBox.Text = "** Cancelled **";
        else
        {
          DataTable dt = new DataTable();
          dt = (DataTable)e.Result;
          if (dt != null)
          {
            for (int a = 0; a < 64; ++a)
            {
              if (!dt.Rows[a][1].Equals(100))
                buffDVmaf1["Multiplier", a].Value = ((double)dt.Rows[a][1] / 100);
              if (!dt.Rows[a][2].Equals(100))
                buffDVmaf2["Multiplier", a].Value = ((double)dt.Rows[a][2] / 100);
            }
            buffDVmaf1.Refresh();
            buffDVmaf2.Refresh();
          }
        }
      }
      finally
      {
        ProgressBar.Value = 0;
        SetAppState(AppStates.Idle, null);
      }
    }

    private void SetFileReadWidgetsVisible(bool visible)
    {
      ProgressBar.Visible = visible;
      btnCancelParse.Visible = visible;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      worker.CancelAsync();
    }

    private void tabPage2_Enter(object sender, EventArgs e)
    {
      if (buffDV1.RowCount > 50)
        tab2Loader(true);
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
    
    private void buffDVmaf_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
      if (buffDVmaf1.CurrentCell.ColumnIndex == buffDVmaf1.Columns["Values"].Index
        || buffDVmaf2.CurrentCell.ColumnIndex == buffDVmaf2.Columns["Values"].Index)
      {
        if (e.Control is TextBox tb)
        {
          tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
        }
      }
    }
    
    private void Column1_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
        e.Handled = true;
    }

    private void selectAllValues()
    {
      if (buffDVmaf1.Focused && !buffDVmaf2.Focused)
      {
        int col = buffDVmaf1.CurrentCell.ColumnIndex;
        foreach (DataGridViewColumn c in buffDVmaf1.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        buffDVmaf1.ClearSelection();
        for (int r = 0; r < buffDVmaf1.RowCount; r++)
          buffDVmaf1[col, r].Selected = true;
      }
      if (!buffDVmaf1.Focused && buffDVmaf2.Focused)
      {
        int col = buffDVmaf2.CurrentCell.ColumnIndex;
        foreach (DataGridViewColumn c in buffDVmaf2.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        buffDVmaf2.ClearSelection();
        for (int r = 0; r < buffDVmaf2.RowCount; r++)
          buffDVmaf2[col, r].Selected = true;
      }

      if (buffDV1.Focused)
      {
        int col = buffDV1.CurrentCell.ColumnIndex;
        foreach (DataGridViewColumn c in buffDV1.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        buffDV1.ClearSelection();
        for (int r = 0; r < buffDV1.RowCount; r++)
          buffDV1[col, r].Selected = true;
      }

    }

    private void copyValue()
    {
      if (buffDVmaf1.Focused && !buffDVmaf2.Focused)
      {
        try
        {
          //May need to copy values as hex to be inputted into uprev graph
          Clipboard.SetDataObject(
              this.buffDVmaf1.GetClipboardContent());
        }
        catch (System.Runtime.InteropServices.ExternalException)
        {
         Console.WriteLine("The Clipboard could not be accessed. Please try again.");
        }
      }
      else if (!buffDVmaf1.Focused && buffDVmaf2.Focused)
      {
        try
        {
          Clipboard.SetDataObject(
              this.buffDVmaf2.GetClipboardContent());
        }
        catch (System.Runtime.InteropServices.ExternalException)
        {
          Console.WriteLine("The Clipboard could not be accessed. Please try again.");
        }
      }
    }

    private void pasteValue()
    {
      string s = Clipboard.GetText();
      if (s.EndsWith("\r\n"))
        s = s.TrimEnd('\r', '\n');
      string[] lines = s.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
      List<int> clnLines = new List<int>();
      const int maxRow = 64;
      int row;
      foreach (string tmpstr in lines)
      {
        int hex;
        string str = tmpstr;
        if (str == "0"
          || str == "00"
          || str == "00 ")
        {
          clnLines.Add(0);
        }
        else if (str.EndsWith(" ")
                && str.Length >= 3
                && str.Length <= 5)
              {
                str = str.TrimEnd(' ');
                hex = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                clnLines.Add(hex);
              }
        else if (str.Length < 3 || str.Length > 5)
        {
          hex = 0000;
          clnLines.Add(hex);
        }
        else
          clnLines.Add(Convert.ToInt32(str));
      }
      if (buffDVmaf1.Focused && !buffDVmaf2.Focused)
      {
        row = buffDVmaf1.CurrentCell.RowIndex;
        for (int i = 0; i < clnLines.Count; i++)
        {
          if (row < maxRow)
          {
            buffDVmaf1[1, row].Value = clnLines[i];
            buffDVmaf1[2, row].Value = clnLines[i] * (double)buffDVmaf1[3, row].Value;  //Calculates the adjustment | user_value * multiplier
            ++row;
          }
          else
            break;
        }
      }
      else if (!buffDVmaf1.Focused && buffDVmaf2.Focused)
      {
        row = buffDVmaf2.CurrentCell.RowIndex;
        for (int i = 0; i < clnLines.Count; i++)
        {
          if (row < maxRow)
          {
            buffDVmaf2[1, row].Value = clnLines[i];
            buffDVmaf2[2, row].Value = clnLines[i] * (double)buffDVmaf2[3, row].Value;  //Calculates the adjustment | user_value * multiplier
            ++row;
          }
          else
            break;
        }
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

    private void buffDVmaf_KeyDown(object sender, KeyEventArgs e)
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

    private void buffDV1_Leave(object sender, EventArgs e)
    {
      if (buffDVmaf2.RowCount > 0 && buffDVmaf2.ColumnCount > 0)
      {
        buffDVmaf2.CurrentCell = this.buffDVmaf2[0, 0];
        this.buffDVmaf2.CurrentCell.Selected = false;
      }
    }

    private void buffDV1_VisibleChanged(object sender, EventArgs e)
    {
      if (buffDV1.RowCount > 0 && buffDV1.ColumnCount > 0)
      {
        buffDV1.CurrentCell = this.buffDV1[0, 0];
        this.buffDV1.CurrentCell.Selected = false;
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

    private void buffDVmaf1_VisibleChanged(object sender, EventArgs e)
    {
      if (buffDVmaf1.RowCount > 0 && buffDVmaf1.ColumnCount > 0)
      {
        buffDVmaf1.CurrentCell = this.buffDVmaf1[0, 0];
        this.buffDVmaf1.CurrentCell.Selected = false;
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

    private void buffDVmaf2_VisibleChanged(object sender, EventArgs e)
    {
      if (buffDVmaf2.RowCount > 0 && buffDVmaf2.ColumnCount > 0)
      {
        buffDVmaf2.CurrentCell = this.buffDVmaf2[0, 0];
        this.buffDVmaf2.CurrentCell.Selected = false;
      }
    }

    private void buffDVmaf1_CellValidated(object sender, DataGridViewCellEventArgs e)
    {
       if (e.RowIndex > -1)
       {
         DataGridViewRow row = buffDVmaf1.Rows[e.RowIndex];
         string valueA = row.Cells["Values"].Value.ToString();
         string valueB = row.Cells["Multiplier"].Value.ToString();
         int value;
         double multi = 0;
         if (Int32.TryParse(valueA, out value)
             && Double.TryParse(valueB, out multi))
         {
          row.Cells["Adjustments"].Value = (double)value * multi;
         }
       }
    }

    private void buffDVmaf2_CellValidated(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex > -1)
      {
        DataGridViewRow row = buffDVmaf2.Rows[e.RowIndex];
        string valueA = row.Cells["Values"].Value.ToString();
        string valueB = row.Cells["Multiplier"].Value.ToString();
        int value;
        double multi = 0;
        if (Int32.TryParse(valueA, out value)
            && Double.TryParse(valueB, out multi))
        {
          row.Cells["Adjustments"].Value = (double)value * multi;
        }
      }
    }
  }
}