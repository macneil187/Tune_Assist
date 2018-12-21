namespace AutoTune
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using System.Windows.Forms;

  public class IndexFinder
  {
    private static int timeDex;
    private static int stB1Dex;
    private static int stB2Dex;
    private static int accelDex;
    private static int ltB1Dex;
    private static int ltB2Dex;
    private static int afrB1Dex;
    private static int afrB2Dex;
    private static int mafB1Dex;
    private static int mafB2Dex;
    private static int targetDex;
    private static int intakeAirTempDex;
    private static int coolantTempDex;
    private static int fuelCompTraceDex;
    private static int rpmDex;
    private static bool dualTB;

    public int TimeDex
    {
      get
      {
        return timeDex;
      }
    }

    public int StB1Dex
    {
      get
      {
        return stB1Dex;
      }
    }

    public int StB2Dex
    {
      get
      {
        return stB2Dex;
      }
    }

    public int AccelDex
    {
      get
      {
        return accelDex;
      }
    }

    public int LtB1Dex
    {
      get
      {
        return ltB1Dex;
      }
    }

    public int LtB2Dex
    {
      get
      {
        return ltB2Dex;
      }
    }

    public int AfrB1Dex
    {
      get
      {
        return afrB1Dex;
      }
    }

    public int AfrB2Dex
    {
      get
      {
        return afrB2Dex;
      }
    }

    public int MafB1Dex
    {
      get
      {
        return mafB1Dex;
      }
    }

    public int MafB2Dex
    {
      get
      {
        return mafB2Dex;
      }
    }

    public int TargetDex
    {
      get
      {
        return targetDex;
      }
    }

    public int IntakeAirTempDex
    {
      get
      {
        return intakeAirTempDex;
      }
    }

    public int CoolantTempDex
    {
      get
      {
        return coolantTempDex;
      }
    }

    public int FuelCompTraceDex
    {
      get
      {
        return fuelCompTraceDex;
      }
    }

    public int RpmDex
    {
      get
      {
        return rpmDex;
      }
    }

    public void FindHeader_Indexes(DataGridView tempgrid)
    {

      if (tempgrid.Columns.Contains("Time"))
      {
        timeDex = tempgrid.Columns["Time"].Index;
      }
      else
      {
        timeDex = -1;
      }

      if (tempgrid.Columns.Contains("A/F CORR-B1 (%)"))
      {
        stB1Dex = tempgrid.Columns["A/F CORR-B1 (%)"].Index;
      }
      else
      {
        stB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("A/F CORR-B2 (%)"))
      {
        stB2Dex = tempgrid.Columns["A/F CORR-B2 (%)"].Index;
      }
      else
      {
        stB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("THROTTLE SENSOR 1 - B1(V)"))
      {
        accelDex = tempgrid.Columns["THROTTLE SENSOR 1 - B1(V)"].Index;
      }
      else if (tempgrid.Columns.Contains("ACCEL PED POS 1 (V-Accel)"))
      {
        accelDex = tempgrid.Columns["ACCEL PED POS 1 (V-Accel)"].Index;
      }
      else
      {
        accelDex = -1;
      }

      if (tempgrid.Columns.Contains("LT Fuel Trim B1 (%)"))
      {
        ltB1Dex = tempgrid.Columns["LT Fuel Trim B1 (%)"].Index;
      }
      else
      {
        ltB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("LT Fuel Trim B2 (%)"))
      {
        ltB2Dex = tempgrid.Columns["LT Fuel Trim B2 (%)"].Index;
      }
      else
      {
        ltB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("LC-1 (1) AFR"))
      {
        afrB1Dex = tempgrid.Columns["LC-1 (1) AFR"].Index;
      }
      else if (tempgrid.Columns.Contains("AFR WB-B1"))
      {
        afrB1Dex = tempgrid.Columns["AFR WB-B1"].Index;
      }
      else
      {
        afrB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("LC-1 (2) AFR"))
      {
        tempgrid.Columns.Contains("LC-1 (2) AFR");
      }
      else if (tempgrid.Columns.Contains("AFR WB-B2"))
      {
        afrB2Dex = tempgrid.Columns["AFR WB-B2"].Index;
      }
      else
      {
        afrB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("MAS A/F -B1 (V)"))
      {
        mafB1Dex = tempgrid.Columns["MAS A/F -B1 (V)"].Index;
      }
      else
      {
        mafB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("MAS A/F -B2 (V)"))
      {
        mafB2Dex = tempgrid.Columns["MAS A/F -B2 (V)"].Index;
      }
      else
      {
        mafB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("TARGET AFR"))
      {
        targetDex = tempgrid.Columns["TARGET AFR"].Index;
      }
      else
      {
        targetDex = -1;
      }

      if (tempgrid.Columns.Contains("INTAKE AIR TMP"))
      {
        intakeAirTempDex = tempgrid.Columns["INTAKE AIR TMP"].Index;
      }
      else
      {
        intakeAirTempDex = -1;
      }

      if (tempgrid.Columns.Contains("COOLANT TEMP"))
      {
        coolantTempDex = tempgrid.Columns["COOLANT TEMP"].Index;
      }
      else
      {
        coolantTempDex = -1;
      }

      if (tempgrid.Columns.Contains("Fuel Compensation X Trace"))
      {
        fuelCompTraceDex = tempgrid.Columns["Fuel Compensation X Trace"].Index;
      }
      else if (tempgrid.Columns.Contains("Fuel Compensation X Trace (%)"))
      {
        fuelCompTraceDex = tempgrid.Columns["Fuel Compensation X Trace (%)"].Index;
      }
      else
      {
        fuelCompTraceDex = -1;
      }

      if (tempgrid.Columns.Contains("ENGINE RPM (rpm)"))
      {
        rpmDex = tempgrid.Columns["ENGINE RPM (rpm)"].Index;
      }
      else
      {
        rpmDex = -1;
      }
    }
  }
}
