namespace AutoTune
{
  using System.Windows.Forms;

  public class IndexFinder
  {
    public int TimeDex { get; set; }

    public int StB1Dex { get; set; }

    public int StB2Dex { get; set; }

    public int AccelDex { get; set; }

    public int LtB1Dex { get; set; }

    public int LtB2Dex { get; set; }

    public int AfrB1Dex { get; set; }

    public int AfrB2Dex { get; set; }

    public int MafB1Dex { get; set; }

    public int MafB2Dex { get; set; }

    public int TargetDex { get; set; }

    public int IntakeAirTempDex { get; set; }

    public int CoolantTempDex { get; set; }

    public int FuelCompTraceDex { get; set; }

    public int RpmDex { get; set; }

    public bool dualTB { get; set; }


    public void FindHeader_Indexes(DataGridView tempgrid)
    {

      if (tempgrid.Columns.Contains("Time"))
      {
        this.TimeDex = tempgrid.Columns["Time"].Index;
      }
      else
      {
        this.TimeDex = -1;
      }

      if (tempgrid.Columns.Contains("A/F CORR-B1 (%)"))
      {
        this.StB1Dex = tempgrid.Columns["A/F CORR-B1 (%)"].Index;
      }
      else
      {
        this.StB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("A/F CORR-B2 (%)"))
      {
        this.StB2Dex = tempgrid.Columns["A/F CORR-B2 (%)"].Index;
      }
      else
      {
        this.StB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("THROTTLE SENSOR 1 - B1(V)"))
      {
        this.AccelDex = tempgrid.Columns["THROTTLE SENSOR 1 - B1(V)"].Index;
      }
      else if (tempgrid.Columns.Contains("ACCEL PED POS 1 (V-Accel)"))
      {
        this.AccelDex = tempgrid.Columns["ACCEL PED POS 1 (V-Accel)"].Index;
      }
      else
      {
        this.AccelDex = -1;
      }

      if (tempgrid.Columns.Contains("LT Fuel Trim B1 (%)"))
      {
        this.LtB1Dex = tempgrid.Columns["LT Fuel Trim B1 (%)"].Index;
      }
      else
      {
        this.LtB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("LT Fuel Trim B2 (%)"))
      {
        this.LtB2Dex = tempgrid.Columns["LT Fuel Trim B2 (%)"].Index;
      }
      else
      {
        this.LtB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("LC-1 (1) AFR"))
      {
        this.AfrB1Dex = tempgrid.Columns["LC-1 (1) AFR"].Index;
      }
      else if (tempgrid.Columns.Contains("AFR WB-B1"))
      {
        this.AfrB1Dex = tempgrid.Columns["AFR WB-B1"].Index;
      }
      else
      {
        this.AfrB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("LC-1 (2) AFR"))
      {
        tempgrid.Columns.Contains("LC-1 (2) AFR");
      }
      else if (tempgrid.Columns.Contains("AFR WB-B2"))
      {
        this.AfrB2Dex = tempgrid.Columns["AFR WB-B2"].Index;
      }
      else
      {
        this.AfrB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("MAS A/F -B1 (V)"))
      {
        this.MafB1Dex = tempgrid.Columns["MAS A/F -B1 (V)"].Index;
      }
      else
      {
        this.MafB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("MAS A/F -B2 (V)"))
      {
        this.MafB2Dex = tempgrid.Columns["MAS A/F -B2 (V)"].Index;
      }
      else
      {
        this.MafB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("TARGET AFR"))
      {
        this.TargetDex = tempgrid.Columns["TARGET AFR"].Index;
      }
      else
      {
        this.TargetDex = -1;
      }

      if (tempgrid.Columns.Contains("INTAKE AIR TMP"))
      {
        this.IntakeAirTempDex = tempgrid.Columns["INTAKE AIR TMP"].Index;
      }
      else
      {
        this.IntakeAirTempDex = -1;
      }

      if (tempgrid.Columns.Contains("COOLANT TEMP"))
      {
        this.CoolantTempDex = tempgrid.Columns["COOLANT TEMP"].Index;
      }
      else if (tempgrid.Columns.Contains("ENG OIL TEMP"))
      {
        this.CoolantTempDex = tempgrid.Columns["ENG OIL TEMP"].Index;
      }
      else
      {
        this.CoolantTempDex = -1;
      }

      if (tempgrid.Columns.Contains("Fuel Compensation X Trace"))
      {
        this.FuelCompTraceDex = tempgrid.Columns["Fuel Compensation X Trace"].Index;
      }
      else if (tempgrid.Columns.Contains("Fuel Compensation X Trace (%)"))
      {
        this.FuelCompTraceDex = tempgrid.Columns["Fuel Compensation X Trace (%)"].Index;
      }
      else
      {
        this.FuelCompTraceDex = -1;
      }

      if (tempgrid.Columns.Contains("ENGINE RPM (rpm)"))
      {
        this.RpmDex = tempgrid.Columns["ENGINE RPM (rpm)"].Index;
      }
      else
      {
        this.RpmDex = -1;
      }
    }
  }
}
