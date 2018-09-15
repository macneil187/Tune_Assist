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
    private enum AppStates { Idle, ParsingLog };
    public static AutoTune autotune;
    public List<int> MAFb1UserInput = new List<int>();
    public List<int> MAFb2UserInput = new List<int>();
    public List<int> AdjustMAFb1 = new List<int>();
    public List<int> AdjustMAFb2 = new List<int>();
    private BackgroundWorker worker;
    private BackgroundWorker mafWorker;
    private DataTable MAF1_DT = new DataTable();
    private DataTable MAF2_DT = new DataTable();
    private TextBox TextBox1 = new TextBox();
    private Parser parser = new Parser();
    private Buffer.BuffDV_FuelComp DV_FC = new Buffer.BuffDV_FuelComp();
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
      InitializeComponent();
      buildMAF_DT();
      buildFC_DT();
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
            if (Properties.Settings.Default.MAF_CL)
            {
              worker = new BackgroundWorker();
              worker.DoWork += worker_ParseLog;
              worker.RunWorkerCompleted += worker_ParseLogCompleted;
              worker.WorkerReportsProgress = true;
              worker.WorkerSupportsCancellation = true;
              worker.ProgressChanged += worker_ProgressChanged;
              worker.RunWorkerAsync(openFileDialog.FileName);

              
            }
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
      SetAppState(AppStates.Idle, null);
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
      Loader loader = new Loader();
      BackgroundWorker bw = sender as BackgroundWorker;
      string sFileToRead = (string)e.Argument;
      e.Result = loader.LoadLog(bw, sFileToRead);
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
          if (buffDV1.RowCount > 80)
          {
            buffDV1.Visible = true;
            buffDVmaf1.Visible = true;
            
            for (int c = 0; c < buffDV1.Columns.Count; ++c)
            {
              buffDV1.Columns[c].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            try
            {
              if (buffDV1.Columns.Contains("MAS A/F -B2 (V)"))
              {
                buffDVmaf2.Visible = true;
                this.dualTB = true;
                buffDVmaf2.Refresh();
              }
              scaleMAF();
              FillFC_DT();
            }
            catch
            { }
          }
          else
          {
            buffDV1.DataSource = null;
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
      if (MAF1_DT.Columns.Count >0 || MAF1_DT.Rows.Count >0)
      {
        MAF1_DT.Clear();
        MAF2_DT.Clear();
        MAF1_DT.Columns.Clear();
        MAF2_DT.Columns.Clear();
        MAF1_DT.Rows.Clear();
        MAF2_DT.Rows.Clear();
      }

      MAF1_DT.Columns.Add("Volts", typeof(double));
      MAF1_DT.Columns.Add("Values", typeof(int));
      MAF1_DT.Columns.Add("Adjustments", typeof(int));
      MAF1_DT.Columns.Add("Multiplier", typeof(double));
      MAF1_DT.Columns.Add("Hits", typeof(int));

      foreach (double d in parser.maf_volts)
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
      buffDVmaf1.Columns["Hits"].Visible = false;
      buffDVmaf1.Columns["Hits"].Width = 55;
      buffDVmaf1.Columns["Hits"].ReadOnly = false;
      buffDVmaf1.Columns["Hits"].DefaultCellStyle.Format = "d";
      buffDVmaf1.Columns["Hits"].SortMode = DataGridViewColumnSortMode.NotSortable;

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
      buffDVmaf2.Columns["Hits"].Visible = false;
      buffDVmaf2.Columns["Hits"].Width = 55;
      buffDVmaf2.Columns["Hits"].ReadOnly = false;
      buffDVmaf2.Columns["Hits"].DefaultCellStyle.Format = "d";
      buffDVmaf2.Columns["Hits"].SortMode = DataGridViewColumnSortMode.NotSortable;
    }

    private void buildFC_DT()
    {
      int Index = 0;
      foreach (int i in Buffer.BuffDV_FuelComp.FC_RPM)
      {
        DV_FuelComp_RPM.Rows.Add();
        DV_FuelComp_RPM.Rows[Index].Height = 22;
        DV_FuelComp_RPM[0, Index].Value = i;
        ++Index;
      }
      Index = 0;
      foreach (int i in Buffer.BuffDV_FuelComp.FC_XdataByte)
      {
        DV_FuelComp_XdataByte.Columns.Add(Convert.ToString(i), Convert.ToString(i));
        DV_FuelComp_XdataByte.Columns[Index].Width = 47;
        DV_FuelComp_XdataByte[Convert.ToString(i), 0].Value = i;
        DV_FuelComp.Rows.Add();
        ++Index;
      }
    }

    private void FillFC_DT()
    {
      // indexFinder1 = maf_volts.BinarySearch(maf1v);
      // if (indexFinder1 < 0)
      //   indexFinder1 = ~indexFinder1;
      
      List<int> tmpRPMlist = Buffer.BuffDV_FuelComp.FC_RPM;
      List<int> tmpXlist = Buffer.BuffDV_FuelComp.FC_XdataByte;

      if (parser.targetDex != -1 && parser.FuelCompTraceDex != -1)
      {
        double actualAFR1;
       double actualAFR2;
        double afr1;
        int afr_b1Index = parser.afrB1Dex;
        double afr2;
        int afr_b2Index = parser.afrB2Dex;
        int finaltrim1;
        int finaltrim2;
        int fuelXtrace;
        int TraceDEX = parser.FuelCompTraceDex;
        double longtrim1;
        int ltB1 = parser.LTb1Dex;
        double longtrim2;
        int ltB2 = parser.LTb2Dex;
        int RPM;
        int rpmIndex = parser.rpmDex;
        int shorttrim1;
        int stB1 = parser.STb1Dex;
        int shorttrim2 = 100;
        int stB2 = parser.STb2Dex;
        double target;
        int targetIndex = parser.targetDex;
        bool dualTB = parser.dualTB;
        int trim;
        double tmpAdjustment1;
        double tmpAdjustment2;

        //int traceValue = buffDV1.Columns["Fuel Compensation X Trace"].Value
        //tempgrid.Rows[r].Cells[mafB1Dex].Value
        //Convert.ToDouble(OL_DT1.Rows[line][c]);

        for (int row = 0; row < buffDV1.Rows.Count; ++row)
        {
          if (targetIndex == -1)
            break;
          target = Convert.ToDouble(buffDV1.Rows[row].Cells[targetIndex].Value);
          if (target == 14.7)
          {
            afr1 = Convert.ToDouble(buffDV1.Rows[row].Cells[afr_b1Index].Value);
            afr2 = Convert.ToDouble(buffDV1.Rows[row].Cells[afr_b2Index].Value);
            fuelXtrace = Convert.ToInt32(buffDV1.Rows[row].Cells[TraceDEX].Value);
            shorttrim1 = Convert.ToInt32(buffDV1.Rows[row].Cells[stB1].Value);
            shorttrim2 = Convert.ToInt32(buffDV1.Rows[row].Cells[stB2].Value);
            RPM = Convert.ToInt32(buffDV1.Rows[row].Cells[rpmIndex].Value);

            if (ltB1 != -1 && dualTB) // Dual throttle bodies and have logged long term trim
            {
              longtrim1 = Convert.ToDouble(buffDV1.Rows[row].Cells[ltB1].Value);
              longtrim2 = Convert.ToDouble(buffDV1.Rows[row].Cells[ltB2].Value);
              finaltrim1 = (shorttrim1 + Convert.ToInt32(longtrim1)) / 2;
              finaltrim2 = (shorttrim2 + Convert.ToInt32(longtrim2)) / 2;
            }
            else if (ltB1 == -1 && dualTB) // Dual throttle bodies and have NOT logged long term trim
            {
              finaltrim1 = shorttrim1;
              finaltrim2 = shorttrim2;
            }
            else if (ltB1 != -1 && !dualTB) // Single throttle body and have logged long term trim
            {
              longtrim1 = Convert.ToDouble(buffDV1.Rows[row].Cells[ltB1].Value);
              longtrim2 = Convert.ToDouble(buffDV1.Rows[row].Cells[ltB2].Value);
              finaltrim1 = (shorttrim1 + Convert.ToInt32(longtrim1)) / 2;
              finaltrim2 = (shorttrim2 + Convert.ToInt32(longtrim2)) / 2;
              finaltrim1 = (finaltrim1 + finaltrim2) / 2;
            }
            else
            {
              finaltrim1 = (shorttrim1 + shorttrim2) / 2;
              finaltrim2 = 100;
            }

            trim = (finaltrim1 + finaltrim2) / 2;
            Console.WriteLine(trim.ToString());


          }
          else if (target < 14.7)
          {
            afr1 = Convert.ToDouble(buffDV1.Rows[row].Cells[afr_b1Index].Value);
            afr2 = Convert.ToDouble(buffDV1.Rows[row].Cells[afr_b2Index].Value);
            fuelXtrace = Convert.ToInt32(buffDV1.Rows[row].Cells[TraceDEX].Value);
            RPM = Convert.ToInt32(buffDV1.Rows[row].Cells[rpmIndex].Value);

            actualAFR1 = Convert.ToDouble(buffDV1.Rows[row + 2].Cells[afr_b1Index].Value);
            if (dualTB && afr2 < 14.7)
              actualAFR2 = Convert.ToDouble(buffDV1.Rows[row + 2].Cells[afr_b2Index].Value);
            else
              actualAFR2 = 0;

            if (actualAFR1 != 0 && actualAFR2 != 0)
            {
              tmpAdjustment1 = ((actualAFR1 + actualAFR2) / target) * 100;
            }
            else
            {
              tmpAdjustment1 = (actualAFR1 / target) * 100;
            }
            Console.WriteLine(tmpAdjustment1.ToString());


          }
          
          





        }
      }
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
      Parser maf_scaler = new Parser();
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
              buffDVmaf1["Hits", a].Value = (int)dt.Rows[a][3];
              buffDVmaf2["Hits", a].Value = (int)dt.Rows[a][4];
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
      else if (!buffDVmaf1.Focused && buffDVmaf2.Focused)
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
      else if (buffDV1.Focused)
      {
        buffDV1.SelectAll();
      }
      else if (DV_Target.Focused)
      {
        DV_Target.SelectAll();
      }
    }

    private void copyValue()
    {
      if (buffDVmaf1.Focused && !buffDVmaf2.Focused)
      {
        if (this.buffDVmaf1.GetCellCount(DataGridViewElementStates.Selected) > 0)
        {
          try
          {
            string templine = this.buffDVmaf1.GetClipboardContent().GetText();
            string[] entries  = templine.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
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
      else if (!buffDVmaf1.Focused && buffDVmaf2.Focused)
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
      else if(DV_Target.Focused)
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

    private void showHits()
    {
      if (buffDVmaf1.Focused || buffDVmaf2.Focused)
      {
        if (!buffDVmaf1.Columns["Hits"].Visible)
        {
          buffDVmaf1.Columns["Hits"].Visible = true;
          buffDVmaf2.Columns["Hits"].Visible = true;
          buffDVmaf1.Columns["Multiplier"].Visible = false;
          buffDVmaf2.Columns["Multiplier"].Visible = false;
        }
        else
        {
          buffDVmaf1.Columns["Hits"].Visible = false;
          buffDVmaf2.Columns["Hits"].Visible = false;
          buffDVmaf1.Columns["Multiplier"].Visible = true;
          buffDVmaf2.Columns["Multiplier"].Visible = true;
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
            showHits();
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
        this.buffDV1.CurrentCell = this.buffDV1[0, 0];
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
         if (int.TryParse(valueA, out value)
             && double.TryParse(valueB, out multi))
         {
          double adjustmentValue = (double)value * multi;
          if (adjustmentValue > 65535)
            adjustmentValue = 65535;
          row.Cells["Adjustments"].Value = adjustmentValue;
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
        if (int.TryParse(valueA, out value)
            && double.TryParse(valueB, out multi))
        {
          double adjustmentValue = (double)value * multi;
          if (adjustmentValue > 65535)
            adjustmentValue = 65535;
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
      comboBox_NAorFI.SelectedIndex = comboBox_NAorFI.Items.IndexOf("Naturally Aspirated");
      comboBox_Stage.SelectedIndex = comboBox_Stage.Items.IndexOf("Aggressive");
      DV_Target.DataSource = buffDT.DT_NAaggressive();
      for ( int i = 0; i < DV_Target.Columns.Count; ++i)
      {
        DV_Target.Columns[i].Width = 50;
      }
    }

    private void comboBox_NAorFI_SelectedIndexChanged(object sender, EventArgs e)
    {
      switchTargetValue();
    }

    private void comboBox_Stage_SelectedIndexChanged(object sender, EventArgs e)
    {
      switchTargetValue();
    }

    private void switchTargetValue()
    {
      if (comboBox_NAorFI.SelectedIndex == 0 && comboBox_Stage.SelectedIndex == 0)
        DV_Target.DataSource = buffDT.DT_NAaggressive();
      else if (comboBox_NAorFI.SelectedIndex == 0 && comboBox_Stage.SelectedIndex == 1)
        DV_Target.DataSource = buffDT.DT_NAmild();
      else if (comboBox_NAorFI.SelectedIndex == 1 && comboBox_Stage.SelectedIndex == 0)
        DV_Target.DataSource = buffDT.DT_SCaggressive();
      else if (comboBox_NAorFI.SelectedIndex == 1 && comboBox_Stage.SelectedIndex == 1)
        DV_Target.DataSource = buffDT.DT_SCmild();
      else if (comboBox_NAorFI.SelectedIndex == 2 && comboBox_Stage.SelectedIndex == 0)
        DV_Target.DataSource = buffDT.DT_Taggressive();
      else if (comboBox_NAorFI.SelectedIndex == 2 && comboBox_Stage.SelectedIndex == 1)
        DV_Target.DataSource = buffDT.DT_Tmild();
      else
        DV_Target.DataSource = null;
    }

    private void DV_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (buffDV1.Focused)
      {
        int col = e.ColumnIndex;
        foreach (DataGridViewColumn c in buffDV1.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        buffDV1.ClearSelection();
        for (int r = 0; r < buffDV1.RowCount; r++)
          buffDV1[col, r].Selected = true;
      }
      else if (buffDVmaf1.Focused && !buffDVmaf2.Focused)
      {
        int col = e.ColumnIndex;
        foreach (DataGridViewColumn c in buffDVmaf1.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        buffDVmaf1.ClearSelection();
        for (int r = 0; r < buffDVmaf1.RowCount; r++)
          buffDVmaf1[col, r].Selected = true;
      }
      else if (!buffDVmaf1.Focused && buffDVmaf2.Focused)
      {
        int col = e.ColumnIndex;
        foreach (DataGridViewColumn c in buffDVmaf2.Columns)
        {
          c.SortMode = DataGridViewColumnSortMode.NotSortable;
          c.Selected = false;
        }
        buffDVmaf2.ClearSelection();
        for (int r = 0; r < buffDVmaf2.RowCount; r++)
          buffDVmaf2[col, r].Selected = true;
      }
    }

    private void DV_Leave(object sender, EventArgs e)
    {
      if (buffDV1.Focused)
      {
        buffDV1.ClearSelection();
      }
      else if (buffDVmaf1.Focused && !buffDVmaf2.Focused)
      {
        if (buffDVmaf1.RowCount > 0 && buffDVmaf1.ColumnCount > 0)
        {
          buffDVmaf1.CurrentCell = this.buffDVmaf1[0, 0];
          this.buffDVmaf1.CurrentCell.Selected = false;
        }
        buffDVmaf1.ClearSelection();
      }
      else if (!buffDVmaf1.Focused && buffDVmaf2.Focused)
      {
        if (buffDVmaf2.RowCount > 0 && buffDVmaf2.ColumnCount > 0)
        {
          buffDVmaf2.CurrentCell = this.buffDVmaf2[0, 0];
          this.buffDVmaf2.CurrentCell.Selected = false;
        }
        buffDVmaf2.ClearSelection();
      }
      else if (DV_Target.Focused)
      {
        this.DV_Target.ClearSelection();
      }
    }
    
    private void tabPage4_Click(object sender, EventArgs e)
    {
      DV_Target.CurrentCell = this.DV_Target[0, 0];
      DV_Target.CurrentCell.Selected = false;
      DV_Target.ClearSelection();
      DV_Target.Focus();
    }

    private void tabControl1_Click(object sender, EventArgs e)
    {
      buffDV1.ClearSelection();
      buffDVmaf1.ClearSelection();
      buffDVmaf2.ClearSelection();
      DV_Target.ClearSelection();
    }
  }
}