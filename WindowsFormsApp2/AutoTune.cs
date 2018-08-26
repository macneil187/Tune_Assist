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
    private BackgroundWorker worker;
    private string fileName;
    private bool dualTB;

    public readonly List<double> maf_volts = new List<double>
    { .08, .16, .23, .31, .39, .47, .55, .63, .70, .78, .86, .94, 1.02, 1.09, 1.17, 1.25, 1.33,
      1.41, 1.48, 1.56, 1.64, 1.72, 1.80, 1.88, 1.95, 2.03, 2.11, 2.19, 2.27, 2.34, 2.42, 2.50,
      2.66, 2.73, 2.81, 2.89, 2.97, 3.05, 3.13, 3.20, 3.28, 3.36, 3.44, 3.52, 3.59 };

    public static List<string> mafHeaders = new List<string>
      { "Time", "A/F CORR-B1 (%)", "A/F CORR-B2 (%)",
      "ACCEL PED POS 1 (V-Accel)", "MAS A/F -B1 (V)",
      "MAS A/F -B2 (V)", "INTAKE AIR TMP", "TARGET AFR" };

    public AutoTune()
    {
      InitializeComponent();
      SetAppState(AppStates.Idle, null);
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
          if (buffDV1.RowCount>50)
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
            {}
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
      button1.Visible = visible;
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
      if(buffDV1.RowCount > 50)
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
      if (dualTB)
      {
        textBox_MAF2.Visible = status;
        buffDVmaf2.Visible = status;
      }
    }

    private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
      buffDV1.Dispose();
      buffDVmaf1.Dispose();
      buffDVmaf2.Dispose();
    }
  }
}
