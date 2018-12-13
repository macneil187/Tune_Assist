namespace AutoTune
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Windows.Forms;

  public class BuffDV : DataGridView
  {
    public BuffDV()
    {
      this.DoubleBuffered = true;
    }
  }

  public class BuffDV_FuelComp : DataGridView
  {
    public static List<int> FC_RPM = new List<int>
    {
      800, 1000, 1200, 1600, 2000, 2400, 2800, 3200, 3600, 4000, 4400, 4800, 5200, 5600, 6000, 6400
    };

    public static List<double> FC_XdataByte = new List<double>
    {
      16.00, 32.00, 48, 64, 80, 96, 112, 128, 144, 160, 176, 192, 208, 224, 240, 255
    };

    public BuffDV_FuelComp()
    {
      this.DoubleBuffered = true;
    }

    public DataTable DV_FCrpm()
    {
      DataTable rpm = new DataTable();
      rpm.Columns.Add("rpm" , typeof(int));
      int rowindex = 0;
      foreach (int i in FC_RPM)
      {
        rpm.Rows.Add(Convert.ToInt32(i));
      }

      return rpm;
    }
  }

  public class BuffDT : DataTable
  {
    public DataTable DT_NAmild()
    {
      DataTable dt = new DataTable("Naturally Aspirated Mild");
      for (int i = 0; i < 8; ++i)
      {

        dt.Columns.Add(string.Empty, typeof(double));
        dt.Columns[i].ColumnName = "x" + i;
        dt.Columns[i].ReadOnly = true;
      }

      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 12.8);
      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 12.6);
      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 12.6, 12.6);
      dt.Rows.Add(14.7, 14.7, 14.7, 12.6, 12.6, 12.6, 12.6, 12.6);
      dt.Rows.Add(14.7, 14.7, 13.3, 12.6, 12.6, 12.6, 12.6, 12.6);
      dt.Rows.Add(14.7, 13.6, 13.2, 12.6, 12.6, 12.6, 12.6, 12.6);
      dt.Rows.Add(14.7, 13.2, 12.6, 12.6, 12.6, 12.6, 12.4, 12.4);
      dt.Rows.Add(14.7, 13.2, 12.6, 12.6, 12.6, 12.6, 12.4, 12.4);

      return dt;
    }

    public DataTable DT_NAaggressive()
    {
      DataTable dt = new DataTable("Naturally Aspirated Aggressive");
      for (int i = 0; i < 8; ++i)
      {

        dt.Columns.Add(string.Empty, typeof(double));
        dt.Columns[i].ColumnName = "x" + i;
        dt.Columns[i].ReadOnly = true;
      }

      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 14.7);
      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 12.9);
      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 12.8);
      dt.Rows.Add(14.7, 14.7, 14.7, 12.8, 12.8, 12.8, 12.8, 12.8);
      dt.Rows.Add(14.7, 14.7, 13.3, 12.8, 12.8, 12.8, 12.8, 12.8);
      dt.Rows.Add(14.7, 13.6, 13.2, 12.8, 12.8, 12.8, 12.8, 12.8);
      dt.Rows.Add(14.7, 13.2, 12.8, 12.8, 12.8, 12.8, 12.8, 12.8);
      dt.Rows.Add(14.7, 13.2, 12.8, 12.8, 12.8, 12.8, 12.8, 12.8);

      return dt;
    }

    public DataTable DT_SCmild()
    {
      DataTable dt = new DataTable("SuperCharger Mild");
      for (int i = 0; i < 8; ++i)
      {

        dt.Columns.Add(string.Empty, typeof(double));
        dt.Columns[i].ColumnName = "x" + i;
        dt.Columns[i].ReadOnly = true;
      }

      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 11.8);
      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 13.2, 11.8);
      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 13.2, 12.2, 11.8);
      dt.Rows.Add(14.7, 14.7, 11.4, 11.4, 11.4, 11.4, 11.4, 11.4);
      dt.Rows.Add(14.7, 14.7, 11.4, 11.4, 11.4, 11.4, 11.4, 11.4);
      dt.Rows.Add(14.7, 14.7, 11.4, 11.4, 11.4, 11.4, 11.4, 11.4);
      dt.Rows.Add(14.7, 11.2, 11.2, 11.4, 11.4, 11.4, 11.4, 11.4);
      dt.Rows.Add(14.7, 11.2, 11.2, 11.4, 11.4, 11.4, 11.4, 11.4);

      return dt;
    }

    public DataTable DT_SCaggressive()
    {
      DataTable dt = new DataTable("SuperCharger Aggressive");
      for (int i = 0; i < 8; ++i)
      {

        dt.Columns.Add(string.Empty, typeof(double));
        dt.Columns[i].ColumnName = "x" + i;
        dt.Columns[i].ReadOnly = true;
      }

      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 12.8, 12.2);
      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 12.8, 12.2);
      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 13.2, 12.2, 11.8);
      dt.Rows.Add(14.7, 14.7, 12.2, 11.8, 11.8, 11.8, 11.8, 11.8);
      dt.Rows.Add(14.7, 14.7, 11.8, 11.8, 11.8, 11.8, 11.8, 11.8);
      dt.Rows.Add(14.7, 14.7, 11.8, 11.8, 11.8, 11.8, 11.8, 11.8);
      dt.Rows.Add(14.7, 11.2, 11.8, 11.8, 11.8, 11.8, 11.8, 11.8);
      dt.Rows.Add(14.7, 11.2, 11.8, 11.8, 11.8, 11.8, 11.8, 11.8);

      return dt;
    }

    public DataTable DT_Tmild()
    {
      DataTable dt = new DataTable("Turbo Mild");
      for (int i = 0; i < 8; ++i)
      {

        dt.Columns.Add(string.Empty, typeof(double));
        dt.Columns[i].ColumnName = "x" + i;
        dt.Columns[i].ReadOnly = true;
      }

      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 11.8, 11.5, 11.5);
      dt.Rows.Add(14.7, 14.7, 13.4, 12.8, 12.4, 11.8, 11.5, 11.5);
      dt.Rows.Add(14.7, 14.2, 12.8, 12.2, 11.8, 11.6, 11.5, 11.5);
      dt.Rows.Add(14.7, 13.1, 12.6, 11.9, 11.8, 11.5, 11.5, 11.5);
      dt.Rows.Add(14.7, 13.1, 12.4, 11.7, 11.8, 11.5, 11.5, 11.5);
      dt.Rows.Add(14.7, 12.4, 11.7, 11.7, 11.5, 11.5, 11.5, 11.5);
      dt.Rows.Add(14.7, 11.2, 11.5, 11.5, 11.5, 11.5, 11.5, 11.5);
      dt.Rows.Add(14.7, 11.2, 11.5, 11.5, 11.5, 11.5, 11.5, 11.5);

      return dt;
    }

    public DataTable DT_Taggressive()
    {
      DataTable dt = new DataTable("Turbo Aggressive");
      for (int i = 0; i < 8; ++i)
      {

        dt.Columns.Add(string.Empty, typeof(double));
        dt.Columns[i].ColumnName = "x" + i;
        dt.Columns[i].ReadOnly = true;
      }

      dt.Rows.Add(14.7, 14.7, 14.7, 14.7, 14.7, 12.1, 11.8, 11.8);
      dt.Rows.Add(14.7, 14.7, 13.4, 12.8, 12.4, 12.0, 11.8, 11.8);
      dt.Rows.Add(14.7, 14.2, 13.0, 12.2, 12.0, 11.9, 11.8, 11.8);
      dt.Rows.Add(14.7, 13.4, 12.8, 11.9, 12.0, 11.8, 11.8, 11.8);
      dt.Rows.Add(14.7, 13.4, 12.6, 11.7, 11.8, 11.8, 11.8, 11.8);
      dt.Rows.Add(14.7, 12.4, 12.2, 11.7, 11.8, 11.8, 11.8, 11.8);
      dt.Rows.Add(14.7, 11.2, 11.8, 11.8, 11.8, 11.8, 11.8, 11.8);
      dt.Rows.Add(14.7, 11.2, 11.8, 11.8, 11.8, 11.8, 11.8, 11.8);

      return dt;
    }
  }
}
