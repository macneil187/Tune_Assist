using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AutoTune
{
  public partial class AutoTune : Form
  {
    private enum AppStates { Idle, ParsingLog };

    public static AutoTune autotune;
    public List<int> MAFb1UserInput = new List<int>();
    public List<int> MAFb2UserInput = new List<int>();
    public List<int> AdjustMAFb1 = new List<int>();
    public List<int> AdjustMAFb2 = new List<int>();
    private BackgroundWorker worker;
    private BackgroundWorker mafWorker;
    private BackgroundWorker fuelCompWorker;
    private DataTable MAF1_DT = new DataTable();
    private DataTable MAF2_DT = new DataTable();
    private TextBox TextBox1 = new TextBox();
    private Parser parser = new Parser();
    //private Buffer.BuffDV_FuelComp DV_FC = new Buffer.BuffDV_FuelComp();
    private List<Tuple<double, int, int>> MafB1 = new List<Tuple<double, int, int>>();
    private List<Tuple<double, int, int>> MafB2 = new List<Tuple<double, int, int>>();
    private string fileName;
    private bool dualTB;
    private bool mafOption_CL = Properties.Settings.Default.MAF_CL;
    private bool mafOption_OL = Properties.Settings.Default.MAF_OL;
    private bool mafOption_IAT = Properties.Settings.Default.MAF_IAT;
    private bool mafOption_ACCEL = Properties.Settings.Default.MAF_ACCEL;
    
    public static List<string> mafHeaders = new List<string>
      { "Time", "A/F CORR-B1 (%)", "A/F CORR-B2 (%)",
      "ACCEL PED POS 1 (V-Accel)", "MAS A/F -B1 (V)",
      "MAS A/F -B2 (V)", "INTAKE AIR TMP", "TARGET AFR" };

    public AutoTune()
    {
      this.InitializeComponent();
      this.buildMAF_DT();
      this.SetAppState(AppStates.Idle, null);
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
        this.SetAppState(AppStates.ParsingLog, fi.Name);
        this.StatusBox.Text = "File Loaded: " + this.fileName;
        if (this.fileName.Contains(".csv"))
        {
          this.StatusBox.Text = this.fileName;
          try
          {
            if (Properties.Settings.Default.MAF_CL)
            {
              this.worker = new BackgroundWorker();
              this.worker.DoWork += this.loadworker_Start;
              this.worker.RunWorkerCompleted += this.loadworker_ParseLogCompleted;
              this.worker.WorkerReportsProgress = true;
              this.worker.WorkerSupportsCancellation = true;
              this.worker.ProgressChanged += this.loadworker_ProgressChanged;
              this.worker.RunWorkerAsync(openFileDialog.FileName);
            }
          }
          catch
          {
            this.SetAppState(AppStates.Idle, null);
            Console.WriteLine(" ERROR! ");
          }
          this.closeFileToolStripMenuItem.Enabled = true;
        }
      }
    }

    private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (this.buffDV1.Rows.Count < 50)
      {
        return;
      }

      if (MessageBox.Show("Are you sure you want to close this log?",
          "Close Log",
          MessageBoxButtons.YesNo) == DialogResult.No)
      {
        return;
      }
      this.buffDV1.DataSource = null;
      this.buffDV1.Refresh();
      this.buffDVmaf1.DataSource = null;
      this.buffDVmaf1.Refresh();
      this.buffDVmaf2.DataSource = null;
      this.buffDVmaf2.Refresh();
      this.tab2Loader(false);
      this.SetAppState(AppStates.Idle, null);
      this.closeFileToolStripMenuItem.Enabled = false;
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
          this.SetFileReadWidgetsVisible(false);
          this.StatusBox.Visible = true;
          this.ProgressBar.Visible = false;
          break;

        case AppStates.ParsingLog:
          this.SetFileReadWidgetsVisible(true);
          this.ProgressBar.Text = string.Format("Reading file: {0}", filename);
          this.StatusBox.Visible = false;
          break;
      }
    }

    void loadworker_Start(object sender, DoWorkEventArgs e)
    {
      Loader loader = new Loader();
      BackgroundWorker bw = sender as BackgroundWorker;
      string sFileToRead = (string)e.Argument;
      e.Result = loader.LoadLog(bw, sFileToRead);
      if (bw.CancellationPending)
      {
        e.Cancel = true;
      }
    }

    void loadworker_ParseLogCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      try
      {
        if (e.Error != null)
          MessageBox.Show(e.Error.Message, "Error During File Read");
        else if (e.Cancelled)
          this.StatusBox.Text = "** Cancelled **";
        else
        {
          this.buffDV1.DataSource = null;

          this.buffDV1.DataSource = (DataTable)e.Result;
          if (this.buffDV1.RowCount > 80)
          {
            this.buffDV1.Visible = true;
            this.buffDVmaf1.Visible = true;
            
            for (int c = 0; c < this.buffDV1.Columns.Count; ++c)
            {
              this.buffDV1.Columns[c].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            try
            {
              if (this.buffDV1.Columns.Contains("MAS A/F -B2 (V)"))
              {
                this.buffDVmaf2.Visible = true;
                this.dualTB = true;
                this.buffDVmaf2.Refresh();
              }
              this.scaleMAF();
            }
            catch
            { }
          }
          else
          {
            this.buffDV1.DataSource = null;
          }
        }
      }
      finally
      {
        this.ProgressBar.Value = 0;
        this.SetAppState(AppStates.Idle, null);
      }
    }

    void loadworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      this.ProgressBar.Value = e.ProgressPercentage;
    }

    void mafWorker_Start(object sender, DoWorkEventArgs e)
    {
      Parser maf_scaler = new Parser();
      BackgroundWorker bw = sender as BackgroundWorker;
      DataGridView tempgrid = (DataGridView)e.Argument;
      e.Result = maf_scaler.AdjustMAF(bw, tempgrid);
      if (bw.CancellationPending)
      {
        e.Cancel = true;
      }
    }

    void mafWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      this.ProgressBar.Value = e.ProgressPercentage;
    }

    void mafWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
    {
      try
      {
        if (e.Error != null)
          MessageBox.Show(e.Error.Message, "Error During Maf Adjustments");
        else if (e.Cancelled)
          this.StatusBox.Text = "** Canceled **";
        else
        {
          DataTable dt = new DataTable();
          dt = (DataTable)e.Result;
          if (dt != null)
          {
            for (int a = 0; a < 64; ++a)
            {
              if (!dt.Rows[a][1].Equals(100))
              {
                this.buffDVmaf1["Multiplier", a].Value = ((double)dt.Rows[a][1] / 100);
              }

              if (!dt.Rows[a][2].Equals(100))
              {
                this.buffDVmaf2["Multiplier", a].Value = ((double)dt.Rows[a][2] / 100);
              }

              this.buffDVmaf1["Hits", a].Value = (int)dt.Rows[a][3];
              this.buffDVmaf2["Hits", a].Value = (int)dt.Rows[a][4];
            }
            this.buffDVmaf1.Refresh();
            this.buffDVmaf2.Refresh();
            
          }
        }
      }
      finally
      {
        this.ProgressBar.Value = 0;
        this.SetAppState(AppStates.Idle, null);
        this.buildFC_DT();
      }
    }

    private void FuelCompWorker_Start(object sender, DoWorkEventArgs e)
    {
      Parser fuelComp = new Parser();
      BackgroundWorker bw = sender as BackgroundWorker;
      DataGridView tempgrid = (DataGridView)e.Argument;
      e.Result = fuelComp.AdjustFuelComp(bw, tempgrid);
      if (bw.CancellationPending)
      {
        e.Cancel = true;
      }
    }

    private void FuelCompWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      this.ProgressBar.Value = e.ProgressPercentage;
    }

    private void FuelCompWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
    {
      try
      {
        if (e.Error != null)
          MessageBox.Show(e.Error.Message, "Error During Fuel Comp Adjustments");
        else if (e.Cancelled)
          this.StatusBox.Text = "** Canceled **";
        else
        {
          if ((DataTable)e.Result != null)
          {
            this.DV_FuelComp.Columns.Clear();
            this.DV_FuelComp.DataSource = (DataTable)e.Result;
            this.DV_FuelComp.Refresh();

            for (int col = 0; col < this.DV_FuelComp.Columns.Count; ++col)
            {
              this.DV_FuelComp.Columns[col].Width = 47;
              this.DV_FuelComp.Columns[col].ReadOnly = true;
              this.DV_FuelComp.Columns[col].DefaultCellStyle.Format = "N2";
              this.DV_FuelComp.Columns[col].SortMode = DataGridViewColumnSortMode.NotSortable;
              this.DV_FuelComp.Columns[col].Resizable = System.Windows.Forms.DataGridViewTriState.False;
            }
          }
        }
      }
      finally
      {
        this.ProgressBar.Value = 0;
        this.SetAppState(AppStates.Idle, null);
      }
    }

    private void buildMAF_DT()
    {
      // MAF 1 DataTable
      if (this.MAF1_DT.Columns.Count >0 || this.MAF1_DT.Rows.Count >0)
      {
        this.MAF1_DT.Clear();
        this.MAF2_DT.Clear();
        this.MAF1_DT.Columns.Clear();
        this.MAF2_DT.Columns.Clear();
        this.MAF1_DT.Rows.Clear();
        this.MAF2_DT.Rows.Clear();
      }

      this.MAF1_DT.Columns.Add("Volts", typeof(double));
      this.MAF1_DT.Columns.Add("Values", typeof(int));
      this.MAF1_DT.Columns.Add("Adjustments", typeof(int));
      this.MAF1_DT.Columns.Add("Multiplier", typeof(double));
      this.MAF1_DT.Columns.Add("Hits", typeof(int));

      foreach (double d in this.parser.mafVolts)
      {
        this.MAF1_DT.Rows.Add(d);
      }

      this.buffDVmaf1.Rows.Clear();
      this.buffDVmaf1.Columns.Clear();
      this.buffDVmaf1.DataSource = null;
      this.buffDVmaf1.DataSource = this.MAF1_DT;
      this.buffDVmaf1.Columns["Volts"].Width = 40;
      this.buffDVmaf1.Columns["Volts"].ReadOnly = true;
      this.buffDVmaf1.Columns["Volts"].DefaultCellStyle.Format = "N2";
      this.buffDVmaf1.Columns["Volts"].SortMode = DataGridViewColumnSortMode.NotSortable;
      this.buffDVmaf1.Columns["Volts"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.buffDVmaf1.Columns["Values"].Width = 50;
      this.buffDVmaf1.Columns["Values"].ReadOnly = false;
      this.buffDVmaf1.Columns["Values"].DefaultCellStyle.Format = "d";
      this.buffDVmaf1.Columns["Values"].SortMode = DataGridViewColumnSortMode.NotSortable;
      this.buffDVmaf1.Columns["Adjustments"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.buffDVmaf1.Columns["Adjustments"].Width = 80;
      this.buffDVmaf1.Columns["Adjustments"].ReadOnly = true;
      this.buffDVmaf1.Columns["Adjustments"].DefaultCellStyle.Format = "d";
      this.buffDVmaf1.Columns["Adjustments"].SortMode = DataGridViewColumnSortMode.NotSortable;
      this.buffDVmaf1.Columns["Adjustments"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.buffDVmaf1.Columns["Multiplier"].Width = 60;
      this.buffDVmaf1.Columns["Multiplier"].ReadOnly = true;
      this.buffDVmaf1.Columns["Multiplier"].DefaultCellStyle.Format = "N4";
      this.buffDVmaf1.Columns["Multiplier"].SortMode = DataGridViewColumnSortMode.NotSortable;
      this.buffDVmaf1.Columns["Hits"].Visible = false;
      this.buffDVmaf1.Columns["Hits"].Width = 55;
      this.buffDVmaf1.Columns["Hits"].ReadOnly = false;
      this.buffDVmaf1.Columns["Hits"].DefaultCellStyle.Format = "d";
      this.buffDVmaf1.Columns["Hits"].SortMode = DataGridViewColumnSortMode.NotSortable;

      // MAF 2 DataTable
      this.MAF2_DT = this.MAF1_DT.Copy();
      this.buffDVmaf2.DataSource = this.MAF2_DT;
      this.buffDVmaf2.Columns["Volts"].Width = 40;
      this.buffDVmaf2.Columns["Volts"].ReadOnly = true;
      this.buffDVmaf2.Columns["Volts"].DefaultCellStyle.Format = "N2";
      this.buffDVmaf2.Columns["Volts"].SortMode = DataGridViewColumnSortMode.NotSortable;
      this.buffDVmaf2.Columns["Volts"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.buffDVmaf2.Columns["Values"].Width = 50;
      this.buffDVmaf2.Columns["Values"].ReadOnly = false;
      this.buffDVmaf2.Columns["Values"].DefaultCellStyle.Format = "d";
      this.buffDVmaf2.Columns["Values"].SortMode = DataGridViewColumnSortMode.NotSortable;
      this.buffDVmaf2.Columns["Adjustments"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.buffDVmaf2.Columns["Adjustments"].Width = 80;
      this.buffDVmaf2.Columns["Adjustments"].ReadOnly = true;
      this.buffDVmaf2.Columns["Adjustments"].DefaultCellStyle.Format = "d";
      this.buffDVmaf2.Columns["Adjustments"].SortMode = DataGridViewColumnSortMode.NotSortable;
      this.buffDVmaf2.Columns["Adjustments"].Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.buffDVmaf2.Columns["Multiplier"].Width = 55;
      this.buffDVmaf2.Columns["Multiplier"].ReadOnly = true;
      this.buffDVmaf2.Columns["Multiplier"].DefaultCellStyle.Format = "N4";
      this.buffDVmaf2.Columns["Multiplier"].SortMode = DataGridViewColumnSortMode.NotSortable;
      this.buffDVmaf2.Columns["Hits"].Visible = false;
      this.buffDVmaf2.Columns["Hits"].Width = 55;
      this.buffDVmaf2.Columns["Hits"].ReadOnly = false;
      this.buffDVmaf2.Columns["Hits"].DefaultCellStyle.Format = "d";
      this.buffDVmaf2.Columns["Hits"].SortMode = DataGridViewColumnSortMode.NotSortable;
    }

    private void buildFC_DT()
    {
      int index = 0;
      foreach (int i in BuffDV_FuelComp.FC_RPM)
      {
        this.DV_FuelComp_RPM.Rows.Add();
        this.DV_FuelComp_RPM.Rows[index].Height = 22;
        this.DV_FuelComp_RPM[0, index].Value = i;
        ++index;
      }

      index = 0;
      foreach (int i in BuffDV_FuelComp.FC_XdataByte)
      {
        this.DV_FuelComp_XdataByte.Columns.Add(Convert.ToString(i), Convert.ToString(i));
        this.DV_FuelComp_XdataByte.Columns[index].Width = 47;
        this.DV_FuelComp_XdataByte[Convert.ToString(i), 0].Value = i;
        ++index;
      }

      try
      {
        this.fuelCompWorker = new BackgroundWorker();
        this.fuelCompWorker.DoWork += this.FuelCompWorker_Start;
        this.fuelCompWorker.RunWorkerCompleted += this.FuelCompWorker_Completed;
        this.fuelCompWorker.WorkerReportsProgress = true;
        this.fuelCompWorker.WorkerSupportsCancellation = true;
        this.fuelCompWorker.ProgressChanged += this.FuelCompWorker_ProgressChanged;
        this.fuelCompWorker.RunWorkerAsync(this.buffDV1);
      }
      catch
      {
        this.SetAppState(AppStates.Idle, null);
        Console.WriteLine(" ERROR! ");
      }
    }

      private void scaleMAF()
    {
      //MAF1
      this.MAFb1UserInput.Clear();
      if (this.buffDVmaf1 != null)
      {
        for (int r = 0; r < this.buffDVmaf1.RowCount; ++r)
        {
          string teststr = Convert.ToString(this.buffDVmaf1["Values", r].Value);
          if (teststr != string.Empty)
            this.MAFb1UserInput.Add(Convert.ToInt32(teststr));
          else
          {
            this.MAFb1UserInput.Clear();
            break;
          }
        }
      }
      bool maf1Ready = this.MAFb1UserInput.Count == 64 ? true : false;
      //MAF2
      this.MAFb2UserInput.Clear();
      if (this.buffDVmaf2 != null)
      {
        for (int r = 0; r < this.buffDVmaf2.RowCount; ++r)
        {
          string teststr = Convert.ToString(this.buffDVmaf2["Values", r].Value);
          if (teststr != string.Empty)
          {
            this.MAFb2UserInput.Add(Convert.ToInt32(teststr));
          }
          else
          {
            this.MAFb2UserInput.Clear();
            break;
          }
        }
      }
      bool maf2Ready = this.MAFb1UserInput.Count == 64 ? true : false;
      //Send it!
      try
      {
        this.mafWorker = new BackgroundWorker();
        this.mafWorker.DoWork += this.mafWorker_Start;
        this.mafWorker.RunWorkerCompleted += this.mafWorker_Completed;
        this.mafWorker.WorkerReportsProgress = true;
        this.mafWorker.WorkerSupportsCancellation = true;
        this.mafWorker.ProgressChanged += this.mafWorker_ProgressChanged;
        this.mafWorker.RunWorkerAsync(this.buffDV1);
      }
      catch
      {
        this.SetAppState(AppStates.Idle, null);
        Console.WriteLine(" ERROR! ");
      }
    }
    
    private void SetFileReadWidgetsVisible(bool visible)
    {
      this.ProgressBar.Visible = visible;
      this.btnCancelParse.Visible = visible;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.worker.CancelAsync();
    }

    private void tabPage2_Enter(object sender, EventArgs e)
    {
      if (this.buffDV1.RowCount > 50)
      {
        this.tab2Loader(true);
      }
    }

    private void tabPage2_Leave(object sender, EventArgs e)
    {
      this.tab2Loader(false);
    }

    private void tab2Loader(bool status)
    {
      this.textBox_MAF1.Visible = status;
      this.buffDVmaf1.Visible = status;
      if (this.dualTB)
      {
        this.textBox_MAF2.Visible = status;
        this.buffDVmaf2.Visible = status;
      }
    }
    
    private void buffDVmaf_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      e.Control.KeyPress -= new KeyPressEventHandler(this.Column1_KeyPress);
      if (this.buffDVmaf1.CurrentCell.ColumnIndex == this.buffDVmaf1.Columns["Values"].Index
        || this.buffDVmaf2.CurrentCell.ColumnIndex == this.buffDVmaf2.Columns["Values"].Index)
      {
        if (e.Control is TextBox tb)
        {
          tb.KeyPress += new KeyPressEventHandler(this.Column1_KeyPress);
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
      if (this.buffDVmaf1.Focused && !this.buffDVmaf2.Focused)
      {
        int col = this.buffDVmaf1.CurrentCell.ColumnIndex;
        foreach (DataGridViewColumn c in this.buffDVmaf1.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        this.buffDVmaf1.ClearSelection();
        for (int r = 0; r < this.buffDVmaf1.RowCount; r++)
        {
          this.buffDVmaf1[col, r].Selected = true;
        }
      }
      else if (!this.buffDVmaf1.Focused && this.buffDVmaf2.Focused)
      {
        int col = this.buffDVmaf2.CurrentCell.ColumnIndex;
        foreach (DataGridViewColumn c in this.buffDVmaf2.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        this.buffDVmaf2.ClearSelection();
        for (int r = 0; r < this.buffDVmaf2.RowCount; r++)
        {
          this.buffDVmaf2[col, r].Selected = true;
        }
      }
      else if (this.buffDV1.Focused)
      {
        this.buffDV1.SelectAll();
      }
      else if (this.DV_Target.Focused)
      {
        this.DV_Target.SelectAll();
      }
    }

    private void copyValue()
    {
      if (this.buffDVmaf1.Focused && !this.buffDVmaf2.Focused)
      {
        if (this.buffDVmaf1.GetCellCount(DataGridViewElementStates.Selected) > 0)
        {
          try
          {
            string templine = this.buffDVmaf1.GetClipboardContent().GetText();
            string[] entries = templine.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            List<string> hexValues = new List<string>();
            foreach (var s in entries)
            {
              if (!String.IsNullOrEmpty(s))
              {
                int num = Convert.ToInt32(s);
                string hexstr = num.ToString("X2");
                hexValues.Add(hexstr);
              }
              else
              {
                hexValues.Add("0000");
              }
            }

            var newvalues = hexValues.Aggregate((a, b) => a + " \r\n" + b);
            System.Windows.Forms.Clipboard.SetText(newvalues);
          }
          catch (System.Runtime.InteropServices.ExternalException)
          {
            Console.WriteLine("The Clipboard could not be accessed. Please try again.");
          }
        }
      }
      else if (!this.buffDVmaf1.Focused && this.buffDVmaf2.Focused)
      {
        if (this.buffDVmaf2.GetCellCount(DataGridViewElementStates.Selected) > 0)
        {
          try
          {
            string templine = this.buffDVmaf2.GetClipboardContent().GetText();
            string[] entries = templine.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            List<string> hexValues = new List<string>();
            foreach (var s in entries)
            {
              if (!String.IsNullOrEmpty(s))
              {
                int num = Convert.ToInt32(s);
                string hexstr = num.ToString("X2");
                hexValues.Add(hexstr);
              }
              else
              {
                hexValues.Add("0000");
              }
            }
            var newvalues = hexValues.Aggregate((a, b) => a + " \r\n" + b);
            newvalues += " \r\n";
            System.Windows.Forms.Clipboard.SetText(newvalues);
          }
          catch (System.Runtime.InteropServices.ExternalException)
          {
            Console.WriteLine("The Clipboard could not be accessed. Please try again.");
          }
        }
      }
      else if(this.DV_Target.Focused)
      {
        if (this.DV_Target.GetCellCount(DataGridViewElementStates.Selected) > 0)
        {
          try
          {
            string templine = this.DV_Target.GetClipboardContent().GetText();
            string[] entries = templine.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            List<string> hexValues = new List<string>();
            foreach (var s in entries)
            {
              if (!String.IsNullOrEmpty(s))
              {
                int num = Convert.ToInt32(s);
                string hexstr = num.ToString("X2");
                hexValues.Add(hexstr);
              }
              else
              {
                hexValues.Add("0000");
              }
            }
            var newvalues = hexValues.Aggregate((a, b) => a + " \r\n" + b);
            newvalues += " \r\n";
            System.Windows.Forms.Clipboard.SetText(newvalues);
          }
          catch (System.Runtime.InteropServices.ExternalException)
          {
            Console.WriteLine("The Clipboard could not be accessed. Please try again.");
          }
        }
      }
    }

    private void pasteValue()
    {
      string s = Clipboard.GetText();
      if (s.EndsWith("\r\n"))
      {
        s = s.TrimEnd('\r', '\n');
      }

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
      if (this.buffDVmaf1.Focused && !this.buffDVmaf2.Focused)
      {
        row = this.buffDVmaf1.CurrentCell.RowIndex;
        for (int i = 0; i < clnLines.Count; i++)
        {
          if (row < maxRow)
          {
            this.buffDVmaf1[1, row].Value = clnLines[i];
            this.buffDVmaf1[2, row].Value = clnLines[i] * (double)this.buffDVmaf1[3, row].Value;  //Calculates the adjustment | user_value * multiplier
            ++row;
          }
          else
            break;
        }
      }
      else if (!this.buffDVmaf1.Focused && this.buffDVmaf2.Focused)
      {
        row = this.buffDVmaf2.CurrentCell.RowIndex;
        for (int i = 0; i < clnLines.Count; i++)
        {
          if (row < maxRow)
          {
            this.buffDVmaf2[1, row].Value = clnLines[i];
            this.buffDVmaf2[2, row].Value = clnLines[i] * (double)this.buffDVmaf2[3, row].Value;  //Calculates the adjustment | user_value * multiplier
            ++row;
          }
          else
            break;
        }
      }
    }

    private void showHits()
    {
      if (this.buffDVmaf1.Focused || this.buffDVmaf2.Focused)
      {
        if (!this.buffDVmaf1.Columns["Hits"].Visible)
        {
          this.buffDVmaf1.Columns["Hits"].Visible = true;
          this.buffDVmaf2.Columns["Hits"].Visible = true;
          this.buffDVmaf1.Columns["Multiplier"].Visible = false;
          this.buffDVmaf2.Columns["Multiplier"].Visible = false;
        }
        else
        {
          this.buffDVmaf1.Columns["Hits"].Visible = false;
          this.buffDVmaf2.Columns["Hits"].Visible = false;
          this.buffDVmaf1.Columns["Multiplier"].Visible = true;
          this.buffDVmaf2.Columns["Multiplier"].Visible = true;
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
            Console.WriteLine("'hex' in Parsing: " + hex);
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
          Console.WriteLine("The data you entered is in the wrong format for the cell");
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
          case Keys.H:
            this.showHits();
            e.Handled = true;
            break;
        }
      }
    }

    private void buffDV1_Leave(object sender, EventArgs e)
    {
      if (this.buffDVmaf2.RowCount > 0 && this.buffDVmaf2.ColumnCount > 0)
      {
        this.buffDVmaf2.CurrentCell = this.buffDVmaf2[0, 0];
        this.buffDVmaf2.CurrentCell.Selected = false;
      }

    }

    private void buffDV1_VisibleChanged(object sender, EventArgs e)
    {
      if (this.buffDV1.RowCount > 0 && this.buffDV1.ColumnCount > 0)
      {
        this.buffDV1.CurrentCell = this.buffDV1[0, 0];
        this.buffDV1.CurrentCell.Selected = false;
      }
    }

    private void buffDVmaf1_Leave(object sender, EventArgs e)
    {
      if (this.buffDVmaf1.RowCount > 0 && this.buffDVmaf1.ColumnCount > 0)
      {
        this.buffDVmaf1.CurrentCell = this.buffDVmaf1[0, 0];
        this.buffDVmaf1.CurrentCell.Selected = false;
      }
    }

    private void buffDVmaf1_VisibleChanged(object sender, EventArgs e)
    {
      if (this.buffDVmaf1.RowCount > 0 && this.buffDVmaf1.ColumnCount > 0)
      {
        this.buffDVmaf1.CurrentCell = this.buffDVmaf1[0, 0];
        this.buffDVmaf1.CurrentCell.Selected = false;
      }
    }

    private void buffDVmaf2_Leave(object sender, EventArgs e)
    {
      if (this.buffDVmaf2.RowCount > 0 && this.buffDVmaf2.ColumnCount > 0)
      {
        this.buffDVmaf2.CurrentCell = this.buffDVmaf2[0, 0];
        this.buffDVmaf2.CurrentCell.Selected = false;
      }
    }

    private void buffDVmaf2_VisibleChanged(object sender, EventArgs e)
    {
      if (this.buffDVmaf2.RowCount > 0 && this.buffDVmaf2.ColumnCount > 0)
      {
        this.buffDVmaf2.CurrentCell = this.buffDVmaf2[0, 0];
        this.buffDVmaf2.CurrentCell.Selected = false;
      }
    }

    private void buffDVmaf1_CellValidated(object sender, DataGridViewCellEventArgs e)
    {
       if (e.RowIndex > -1)
       {
         DataGridViewRow row = this.buffDVmaf1.Rows[e.RowIndex];
         string valueA = row.Cells["Values"].Value.ToString();
         string valueB = row.Cells["Multiplier"].Value.ToString();
         int value;
         double multi = 0;
         if (int.TryParse(valueA, out value)
             && double.TryParse(valueB, out multi))
         {
          double adjustmentValue = (double)value * multi;
          if (adjustmentValue > 65535)
          {
            adjustmentValue = 65535;
          }

          row.Cells["Adjustments"].Value = adjustmentValue;
         }
       }
    }

    private void buffDVmaf2_CellValidated(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex > -1)
      {
        DataGridViewRow row = this.buffDVmaf2.Rows[e.RowIndex];
        string valueA = row.Cells["Values"].Value.ToString();
        string valueB = row.Cells["Multiplier"].Value.ToString();
        int value;
        double multi = 0;
        if (int.TryParse(valueA, out value)
            && double.TryParse(valueB, out multi))
        {
          double adjustmentValue = (double)value * multi;
          if (adjustmentValue > 65535)
          {
            adjustmentValue = 65535;
          }

          row.Cells["Adjustments"].Value = adjustmentValue;
        }
      }
    }

    private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
    {
      HelpForm helpForm = new HelpForm();
      helpForm.Show();
    }

    private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      About about = new About();
      about.Show();
    }

    private void toolStripMenuItem1_Click(object sender, EventArgs e)
    {
      OptionForm optionsForm = new OptionForm();
      optionsForm.Show();
    }

    private void AutoTune_Load(object sender, EventArgs e)
    {
      this.comboBox_NAorFI.SelectedIndex = this.comboBox_NAorFI.Items.IndexOf("Naturally Aspirated");
      this.comboBox_Stage.SelectedIndex = this.comboBox_Stage.Items.IndexOf("Aggressive");
      this.DV_Target.DataSource = this.buffDT.DT_NAaggressive();
      for ( int i = 0; i < this.DV_Target.Columns.Count; ++i)
      {
        this.DV_Target.Columns[i].Width = 50;
      }
    }

    private void comboBox_NAorFI_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.switchTargetValue();
    }

    private void comboBox_Stage_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.switchTargetValue();
    }

    private void switchTargetValue()
    {
      if (this.comboBox_NAorFI.SelectedIndex == 0 && this.comboBox_Stage.SelectedIndex == 0)
      {
        this.DV_Target.DataSource = this.buffDT.DT_NAaggressive();
      }
      else if (this.comboBox_NAorFI.SelectedIndex == 0 && this.comboBox_Stage.SelectedIndex == 1)
      {
        this.DV_Target.DataSource = this.buffDT.DT_NAmild();
      }
      else if (this.comboBox_NAorFI.SelectedIndex == 1 && this.comboBox_Stage.SelectedIndex == 0)
      {
        this.DV_Target.DataSource = this.buffDT.DT_SCaggressive();
      }
      else if (this.comboBox_NAorFI.SelectedIndex == 1 && this.comboBox_Stage.SelectedIndex == 1)
      {
        this.DV_Target.DataSource = this.buffDT.DT_SCmild();
      }
      else if (this.comboBox_NAorFI.SelectedIndex == 2 && this.comboBox_Stage.SelectedIndex == 0)
      {
        this.DV_Target.DataSource = this.buffDT.DT_Taggressive();
      }
      else if (this.comboBox_NAorFI.SelectedIndex == 2 && this.comboBox_Stage.SelectedIndex == 1)
      {
        this.DV_Target.DataSource = this.buffDT.DT_Tmild();
      }
      else
      {
        this.DV_Target.DataSource = null;
      }
    }

    private void DV_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (this.buffDV1.Focused)
      {
        int col = e.ColumnIndex;
        foreach (DataGridViewColumn c in this.buffDV1.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        this.buffDV1.ClearSelection();
        for (int r = 0; r < this.buffDV1.RowCount; r++)
        {
          this.buffDV1[col, r].Selected = true;
        }
      }
      else if (this.buffDVmaf1.Focused && !this.buffDVmaf2.Focused)
      {
        int col = e.ColumnIndex;
        foreach (DataGridViewColumn c in this.buffDVmaf1.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        this.buffDVmaf1.ClearSelection();
        for (int r = 0; r < this.buffDVmaf1.RowCount; r++)
        {
          this.buffDVmaf1[col, r].Selected = true;
        }
      }
      else if (!this.buffDVmaf1.Focused && this.buffDVmaf2.Focused)
      {
        int col = e.ColumnIndex;
        foreach (DataGridViewColumn c in this.buffDVmaf2.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        this.buffDVmaf2.ClearSelection();
        for (int r = 0; r < this.buffDVmaf2.RowCount; r++)
        {
          this.buffDVmaf2[col, r].Selected = true;
        }
      }
    }

    private void DV_Leave(object sender, EventArgs e)
    {
      if (this.buffDV1.Focused)
      {
        this.buffDV1.ClearSelection();
      }
      else if (this.buffDVmaf1.Focused && !this.buffDVmaf2.Focused)
      {
        if (this.buffDVmaf1.RowCount > 0 && this.buffDVmaf1.ColumnCount > 0)
        {
          this.buffDVmaf1.CurrentCell = this.buffDVmaf1[0, 0];
          this.buffDVmaf1.CurrentCell.Selected = false;
        }
        this.buffDVmaf1.ClearSelection();
      }
      else if (!this.buffDVmaf1.Focused && this.buffDVmaf2.Focused)
      {
        if (this.buffDVmaf2.RowCount > 0 && this.buffDVmaf2.ColumnCount > 0)
        {
          this.buffDVmaf2.CurrentCell = this.buffDVmaf2[0, 0];
          this.buffDVmaf2.CurrentCell.Selected = false;
        }
        this.buffDVmaf2.ClearSelection();
      }
      else if (this.DV_Target.Focused)
      {
        this.DV_Target.ClearSelection();
      }
    }
    
    private void tabPage4_Click(object sender, EventArgs e)
    {
      this.DV_Target.CurrentCell = this.DV_Target[0, 0];
      this.DV_Target.CurrentCell.Selected = false;
      this.DV_Target.ClearSelection();
      this.DV_Target.Focus();
    }

    private void tabControl1_Click(object sender, EventArgs e)
    {
      this.buffDV1.ClearSelection();
      this.buffDVmaf1.ClearSelection();
      this.buffDVmaf2.ClearSelection();
      this.DV_Target.ClearSelection();
    }
  }
}