namespace AutoTune
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Windows.Forms;

  public class BuffDV_FuelComp : DataGridView
  {
    public static List<int> fcRPM = new List<int>
    {
      800, 1200, 1600, 2000, 2400, 2800, 3200, 3600, 4000, 4400, 4800, 5200, 5600, 6000, 6400, 6800
    };

    public static List<double> fcThrottlePercent = new List<double>
    {
      6.27, 12.55, 18.82, 25.10, 31.37, 37.65, 43.92, 50.20, 56.27, 62.02, 69.02, 75.29, 81.57, 87.84, 94.12, 100.00
    };

    public List<int> FcRPM
    {
      get
      {
        return fcRPM;
      }

      set
      {
        fcRPM = value;
      }
    }

    public List<double> FcThrottlePercent
    {
      get
      {
        return fcThrottlePercent;
      }

      set
      {
        fcThrottlePercent = value;
      }
    }

    public DataTable DV_FCrpm()
    {
      DataTable rpm = new DataTable();
      rpm.Columns.Add("rpm" , typeof(int));
      int rowindex = 0;
      foreach (int i in fcRPM)
      {
        rpm.Rows.Add(Convert.ToInt32(i));
      }

      return rpm;
    }
  }
}
