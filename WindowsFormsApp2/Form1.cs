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

namespace WindowsFormsApp2
{
  public partial class Form1 : Form
  {
    private DataGridView DV = new DataGridView();
    List<string> headers = new List<string> { };
    List<string> matchedHeaders = new List<string> { };
    List<string> cleanedHeaders = new List<string> { };
    List<int> skipValues = new List<int> { };
    private string fileName;

    public readonly List<double> maf_volts = new List<double>
    { .08, .16, .23, .31, .39, .47, .55, .63, .70, .78, .86, .94, 1.02, 1.09, 1.17, 1.25, 1.33,
      1.41, 1.48, 1.56, 1.64, 1.72, 1.80, 1.88, 1.95, 2.03, 2.11, 2.19, 2.27, 2.34, 2.42, 2.50,
      2.66, 2.73, 2.81, 2.89, 2.97, 3.05, 3.13, 3.20, 3.28, 3.36, 3.44, 3.52, 3.59 };

    public static List<string> mafHeaders = new List<string>
      { "Time",
      "A/F CORR-B1 (%)",
      "A/F CORR-B2 (%)",
      "ACCEL PED POS 1 (V-Accel)",
      "MAS A/F -B1 (V)",
      "MAS A/F -B2 (V)",
      "INTAKE AIR TMP",
      "TARGET AFR" };

    public Form1()
    {
      InitializeComponent();
    }

    private void findHeaders()
    {
      using (StreamReader SR = new StreamReader(fileName))
      {
        headers.Clear();
        matchedHeaders.Clear();
        skipValues.Clear();
        headers.AddRange(SR.ReadLine().Split(','));
        int headerindex = 0;
        foreach (var label in headers)
        {
          if (!matchedHeaders.Contains(label))
          {
            matchedHeaders.Add(label);
            headerindex++;
          }
          else
          {
            skipValues.Add(headerindex);
          }
        }
        Console.WriteLine("findHeaders() has found : \n\t\t" + 
          headers.Count + " headers. \n\t\t" +
          skipValues.Count + " were duplicates. ");
      }
    }

    private void CleanHeaders()
    {
      if (matchedHeaders == null)
        return;
      for (int a = 0; a< matchedHeaders.Count; ++a)
      {
        switch(matchedHeaders[a])
        {
          case "Time":
            cleanedHeaders.Add("Time");
            break;
          case "A/F CORR-B1 (%)":
            cleanedHeaders.Add("AFR_Cor_B1");
            break;
          case "A/F CORR-B2 (%)":
            cleanedHeaders.Add("AFR_Cor_B2");
            break;
          case "ACCEL PED POS 1 (V-Accel)":
            cleanedHeaders.Add("Accel_Pos");
            break;
          case "MAS A/F -B1 (V)":
            cleanedHeaders.Add("AFR_B1");
            break;
          case "MAS A/F -B2 (V)":
            cleanedHeaders.Add("AFR_B2");
            break;
          case "INTAKE AIR TMP":
            cleanedHeaders.Add("Intake_Temp");
            break;
          case "TARGET AFR":
            cleanedHeaders.Add("Target_AFR");
            break;
        }
      }
    }

      private void Setup_Log_DV()
    {
      if (cleanedHeaders.Count < 5 || cleanedHeaders == null)
        return;
      this.Controls.Add(DV);
      //DV.ColumnCount = 5;

      DV.ColumnCount = cleanedHeaders.Count;
      DV.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
      DV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
      DV.ColumnHeadersDefaultCellStyle.Font = new Font(DV.Font, FontStyle.Bold);
      DV.Name = "DV";
      DV.Location = new Point(8, 8);
      DV.Size = new Size(500, 250);
      DV.AutoSizeRowsMode =
          DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
      DV.ColumnHeadersBorderStyle =
          DataGridViewHeaderBorderStyle.Single;
      DV.CellBorderStyle = DataGridViewCellBorderStyle.Single;
      DV.GridColor = Color.Black;
      DV.RowHeadersVisible = false;

      for (int h = 0; h < cleanedHeaders.Count; ++h)
      {
        DV.Columns[h].Name = cleanedHeaders[h];
        DV.Columns[h].ReadOnly = true;
        DV.Columns[h].Frozen = true;
      }
      //DV.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
      DV.MultiSelect = false;
      DV.Dock = DockStyle.Fill;
      //DV.CellFormatting += new DataGridViewCellFormattingEventHandler(DV_CellFormatting);
      DV.Visible = false;
    }

    private void Setup_MAF_DV()
    {
      this.Controls.Add(DV);
      DV.ColumnCount = 5;
      DV.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
      DV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
      DV.ColumnHeadersDefaultCellStyle.Font = new Font(DV.Font, FontStyle.Bold);
      DV.Name = "DV";
      DV.Location = new Point(8, 8);
      DV.Size = new Size(500, 250);
      DV.AutoSizeRowsMode =
          DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
      DV.ColumnHeadersBorderStyle =
          DataGridViewHeaderBorderStyle.Single;
      DV.CellBorderStyle = DataGridViewCellBorderStyle.Single;
      DV.GridColor = Color.Black;
      DV.RowHeadersVisible = false;
      DV.Columns[0].Name = "MAF V";
      DV.Columns[0].ReadOnly = true;
      DV.Columns[1].Name = "Bank 1";
      DV.Columns[1].DefaultCellStyle.Font =
          new Font(DV.DefaultCellStyle.Font, FontStyle.Italic);
      DV.Columns[2].Name = "B1 Adjust";
      DV.Columns[2].ReadOnly = true;
      DV.Columns[3].Name = "Bank 2";
      DV.Columns[3].DefaultCellStyle.Font =
          new Font(DV.DefaultCellStyle.Font, FontStyle.Italic);
      DV.Columns[4].Name = "B2 Adjust";
      DV.Columns[4].ReadOnly = true;
      //DV.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
      DV.MultiSelect = false;
      DV.Dock = DockStyle.Fill;
      //DV.CellFormatting += new DataGridViewCellFormattingEventHandler(DV_CellFormatting);
    }

    private void DV_CellFormatting(object sender,
        System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
    {
      if (e != null && (this.DV.Columns[e.ColumnIndex].Name == "B1 Adjust" || this.DV.Columns[e.ColumnIndex].Name == "B2 Adjust") && e.Value != null)
      {
        try
        {
          e.Value = Double.Parse(e.Value.ToString());
          e.FormattingApplied = true;
        }
        catch (FormatException)
        {
          Console.WriteLine("{0} is not a valid input.", e.Value.ToString());
        }
      }
    }

    private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
    {

    }

    private void Form1_Load(object sender, EventArgs e)
    {
      //Setup_Log_DV();
    }

    private void Populate_Log_DV()
    {
      using (StreamReader SR = new StreamReader(fileName))
      {
        string[] lines = SR.ReadToEnd().Split('\n');
        try
        {
          for (int x = 1; x < lines.Length-1; ++x)
          {
            string[] line = lines[x].Split(',');
            List<string> cell = new List<string> { };
            for (int i = 0; i < headers.Count; ++i)
            {
              if (!skipValues.Contains(i))
                cell.Add(line[i]);
            }

            DV.Rows.Add(cell.ToArray());
            DV.Columns[0].DisplayIndex = 0;
            DV.Columns[1].DisplayIndex = 1;
            DV.Columns[2].DisplayIndex = 2;
            DV.Columns[3].DisplayIndex = 3;
            DV.Columns[4].DisplayIndex = 4;
          }
          DV.ColumnHeadersVisible = true;
          DV.Visible = true;
        }
        catch
        {
          Console.WriteLine("Selected file is empty.");
        }
      }
    }

    private void openFileToolStripMenuItem_Open_Click(object sender, EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      openFileDialog.Filter = "csv Files (*.csv)|*.csv|All Files (*.*)|*.*";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK)
      {
        this.fileName = openFileDialog.FileName;
        StatusBox.Text = "File Loaded: " + fileName;
        if (fileName.Contains(".csv"))
        {
          findHeaders();
          CleanHeaders();
          Setup_Log_DV();
          Populate_Log_DV();
        }
      }
    }
  }
}
