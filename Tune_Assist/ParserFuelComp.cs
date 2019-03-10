namespace AutoTune
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Linq;
  using System.Text;
  using System.Windows.Forms;

  public class ParserFuelComp
  {
    private BuffDV_FuelComp buffFC = new BuffDV_FuelComp();
    //private List<int> tmpRPMlist = BuffDV_FuelComp.FcRPM;
    //private List<double> tmpXlist = BuffDV_FuelComp.fcThrottlePercent;

    private IndexFinder indexer = new IndexFinder();
    private DataTable DT_FC= new DataTable();
    private double accel;
    private double accelChange;
    private bool accelAfterDecel;
    private double actualAFR1;
    private double actualAFR2;
    private double afr1;
    private double afr2;
    private double coolantTemp;
    private double finaltrim1;
    private double finaltrim2;
    private double fuelXtrace;
    private int indexFinderDB;
    private int indexFinderRPM;
    private double intakeAirTemp;
    private double intakeAirTempAVG;
    private double longtrim1;
    private double longtrim2;
    private double nextaccel;
    private int nexttime;
    private int shorttrim1;
    private int shorttrim2 = 100;
    private int rpm;
    private double target;
    private int time;
    private bool dualTB;

    private void FindIAT_average(DataGridView tempgrid)
    {
      List<double> iatFull = new List<double>();

      for (int r = 0; r < tempgrid.Rows.Count - 1; ++r)
      {
        double intakeAirTemp = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.IntakeAirTempDex].Value);
        iatFull.Add(intakeAirTemp);
      }

      this.intakeAirTempAVG = (double)iatFull.Average();
    }

    // *********** Fuel Comp Adjustments below.
    public DataTable AdjustFuelComp(BackgroundWorker bw, DataGridView tempgrid)
    {
      using (DataTable dt_FC_hits = new DataTable())
      using (DataTable dt_FC_totals = new DataTable())
      {
        this.indexer.FindHeader_Indexes(tempgrid);

        if (this.indexer.TimeDex == -1
          || this.indexer.StB1Dex == -1
          || this.indexer.StB2Dex == -1
          || this.indexer.AccelDex == -1
          || this.indexer.AfrB1Dex == -1
          || this.indexer.AfrB2Dex == -1
          || this.indexer.TargetDex == -1
          || this.indexer.FuelCompTraceDex == -1
          || this.indexer.RpmDex == -1
          || this.indexer.IntakeAirTempDex == -1
          || tempgrid.Rows.Count < 50)
        {
          StringBuilder sb = new StringBuilder();
          sb.Append("Could not find the following headers: \n");
          if (this.indexer.TimeDex == -1) { sb.Append("Time\n"); }
          if (this.indexer.StB1Dex == -1) { sb.Append("A/F CORR-B1 (%)\n"); }
          if (this.indexer.StB2Dex == -1) { sb.Append("A/F CORR-B2 (%)\n"); }
          if (this.indexer.AccelDex == -1) { sb.Append("ACCEL PED POS 1\n"); }
          if (this.indexer.AfrB1Dex == -1) { sb.Append("AFR WB-B1\n"); }
          if (this.indexer.AfrB2Dex == -1) { sb.Append("AFR WB-B2\n"); }
          if (this.indexer.TargetDex == -1) { sb.Append("TARGET AFR\n"); }
          if (this.indexer.IntakeAirTempDex == -1) { sb.Append("INTAKE AIR TMP\n"); }
          if (this.indexer.FuelCompTraceDex == -1) { sb.Append("Fuel Compensation X Trace\n"); }
          if (this.indexer.RpmDex == -1) { sb.Append("ENGINE RPM (rpm)\n"); }

          Console.WriteLine(sb.ToString());
          return this.DT_FC;
        }

        if (Properties.Settings.Default.MAF_IAT && this.indexer.IntakeAirTempDex != -1)
        {
          this.FindIAT_average(tempgrid);
        }

        //foreach (int i in this.tmpXlist)
        foreach (int i in this.buffFC.FcThrottlePercent)
        {
          dt_FC_hits.Columns.Add(Convert.ToString(i), typeof(int));
          dt_FC_totals.Columns.Add(Convert.ToString(i), typeof(double));
          this.DT_FC.Columns.Add(Convert.ToString(i), typeof(double));
        }

        //foreach (int i in this.tmpRPMlist)
        foreach (int i in this.buffFC.FcRPM)
        {
          dt_FC_hits.Rows.Add();
          dt_FC_totals.Rows.Add();
          this.DT_FC.Rows.Add();
        }

        for (int row = 0; row < dt_FC_totals.Rows.Count; ++row)
        {
          for (int col = 0; col < dt_FC_totals.Columns.Count; ++col)
          {
            dt_FC_totals.Rows[row][col] = "100";
            dt_FC_hits.Rows[row][col] = "1";
            this.DT_FC.Rows[row][col] = "100";
          }
        }

        for (int row = 1; row < tempgrid.Rows.Count - 10; ++row)
        {
          this.accel = Convert.ToDouble(tempgrid.Rows[row].Cells[this.indexer.AccelDex].Value);
          this.nextaccel = Convert.ToDouble(tempgrid.Rows[row + 1].Cells[this.indexer.AccelDex].Value);
          this.afr1 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.indexer.AfrB1Dex].Value);
          this.afr2 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.indexer.AfrB2Dex].Value);
          this.coolantTemp = Convert.ToDouble(tempgrid.Rows[row + 1].Cells[this.indexer.CoolantTempDex].Value);
          this.intakeAirTemp = Convert.ToInt32(tempgrid.Rows[row].Cells[this.indexer.IntakeAirTempDex].Value);
          this.time = Convert.ToInt32(tempgrid.Rows[row].Cells[this.indexer.TimeDex].Value);
          this.nexttime = Convert.ToInt32(tempgrid.Rows[row + 1].Cells[this.indexer.TimeDex].Value);
          this.fuelXtrace = Convert.ToDouble(tempgrid.Rows[row].Cells[this.indexer.FuelCompTraceDex].Value);
          this.shorttrim1 = Convert.ToInt32(tempgrid.Rows[row].Cells[this.indexer.StB1Dex].Value);
          this.shorttrim2 = Convert.ToInt32(tempgrid.Rows[row].Cells[this.indexer.StB1Dex].Value);
          this.rpm = Convert.ToInt32(tempgrid.Rows[row].Cells[this.indexer.RpmDex].Value);
          this.target = Convert.ToDouble(tempgrid.Rows[row].Cells[this.indexer.TargetDex].Value);

          this.accelChange = Convert.ToDouble(((this.nextaccel - this.accel) / (this.nexttime - this.time)) * 1000);

          // Makes sure the given RPM value lands in the last index.
          int lastRPMx = this.buffFC.FcRPM.Count - 1;
          if (this.rpm > this.buffFC.FcRPM[lastRPMx])
          {
            this.rpm = this.buffFC.FcRPM[lastRPMx];
          }

          if (this.target > 15 || this.rpm < 600)
          {
            continue;
          }

          // Back on accel after decel  ** This will skip down rows to avoid skewing values
          if (this.afr1 == 60 || this.target == 30)
          {
            this.accelAfterDecel = true;
            continue;
          }
          else if (this.accelAfterDecel)
          {
            row += 10;
            this.accelAfterDecel = false;
            continue;
          }

          // Only allows rows where intake air temp are close to the average.
          if (this.intakeAirTemp <= this.intakeAirTempAVG - 8 && this.intakeAirTemp >= this.intakeAirTempAVG + 8
            /*&& Properties.Settings.Default.MAF_ACCEL*/ && this.accelChange > -0.1 && this.accelChange < 0.1)
          {
            continue;
          }

          this.indexFinderDB = this.buffFC.FcThrottlePercent.BinarySearch(this.fuelXtrace);
          if (this.indexFinderDB < 0)
          {
            this.indexFinderDB = ~this.indexFinderDB;
          }

          this.indexFinderRPM = this.buffFC.FcRPM.BinarySearch(this.rpm);
          if (this.indexFinderRPM < 0)
          {
            this.indexFinderRPM = ~this.indexFinderRPM;
          }

          if (this.indexer.StB1Dex != -1 && this.indexer.StB2Dex != -1 && this.afr1 < 25 && this.afr2 < 25 && this.target == 14.7)
          {
            this.shorttrim1 = Convert.ToInt32(tempgrid.Rows[row].Cells[this.indexer.StB1Dex].Value);
            this.shorttrim2 = Convert.ToInt32(tempgrid.Rows[row].Cells[this.indexer.StB2Dex].Value);

            // if long term trimlogged
            if (this.indexer.LtB1Dex != -1 && this.indexer.LtB2Dex != -1)
            {
              this.longtrim1 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.indexer.LtB1Dex].Value);
              this.longtrim2 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.indexer.LtB2Dex].Value);
              this.finaltrim1 = (this.shorttrim1 + Convert.ToInt32(this.longtrim1)) - 100;
              this.finaltrim2 = (this.shorttrim2 + Convert.ToInt32(this.longtrim2)) - 100;
              this.finaltrim1 = (this.finaltrim1 + this.finaltrim2) / 2;
            }
            else
            {
              this.finaltrim1 = (this.shorttrim1 + this.shorttrim2) / 2;
            }
          }
          else if (this.indexer.StB1Dex != -1 && this.indexer.StB2Dex != -1 && this.afr1 < 25 && this.afr2 < 25 && this.target < 14.7)
          {
            this.actualAFR1 = Convert.ToDouble(tempgrid.Rows[row + 2].Cells[this.indexer.AfrB1Dex].Value);

            if (this.dualTB)
            {
              this.actualAFR2 = Convert.ToDouble(tempgrid.Rows[row + 2].Cells[this.indexer.AfrB2Dex].Value);
            }
            else
            {
              this.actualAFR2 = 0;
            }

            if (this.actualAFR1 == 0 || this.actualAFR1 > 18 || this.actualAFR2 > 18)
            {
              this.finaltrim1 = 0;
              continue;
            }
            else if (this.actualAFR1 != 0 && this.actualAFR2 != 0)
            {
              this.finaltrim1 = (((this.actualAFR1 + this.actualAFR2) / 2) / this.target) * 100;
            }
            else
            {
              this.finaltrim1 = (this.actualAFR1 / this.target) * 100;
            }
          }

          int hitCount = Convert.ToInt32(dt_FC_hits.Rows[this.indexFinderRPM][this.indexFinderDB]);
          double value = Convert.ToDouble(dt_FC_totals.Rows[this.indexFinderRPM][this.indexFinderDB]);

          if (value == 100
            && hitCount == 1)
          {
            dt_FC_totals.Rows[this.indexFinderRPM][this.indexFinderDB] = this.finaltrim1 + 100;
            dt_FC_hits.Rows[this.indexFinderRPM][this.indexFinderDB] = hitCount + 1;
          }
          else
          {
            dt_FC_totals.Rows[this.indexFinderRPM][this.indexFinderDB] = value + this.finaltrim1;
            dt_FC_hits.Rows[this.indexFinderRPM][this.indexFinderDB] = Convert.ToString(hitCount + 1);
          }
        }

        for (int row = 0; row < dt_FC_totals.Rows.Count; ++row)
        {
          for (int col = 0; col < dt_FC_totals.Columns.Count; ++col)
          {
            double total = 100;
            int hits = 1;
            if (dt_FC_totals.Rows[row][col].ToString() == "0" || dt_FC_totals.Rows[row][col].ToString() == "100" || string.IsNullOrEmpty(dt_FC_totals.Rows[row][col].ToString()))
            {
              total = 100;
            }
            else if (dt_FC_totals.Rows[row][col] != null || !string.IsNullOrEmpty(dt_FC_totals.Rows[row][col].ToString()))
            {
              string totalvalue = Convert.ToString(dt_FC_totals.Rows[row][col]);
              total = Convert.ToDouble(totalvalue);
            }

            if (dt_FC_hits.Rows[row][col].ToString() == "0" || dt_FC_hits.Rows[row][col].ToString() == "1" || string.IsNullOrEmpty(dt_FC_hits.Rows[row][col].ToString()))
            {
              hits = 1;
            }
            else if (dt_FC_hits.Rows[row][col] != null || !string.IsNullOrEmpty(dt_FC_hits.Rows[row][col].ToString()))
            {
              hits = Convert.ToInt32(dt_FC_hits.Rows[row][col]);
            }

            if (total == 0 || hits == 0)
            {
              this.DT_FC.Rows[row][col] = 100;
            }
            else
            {
              this.DT_FC.Rows[row][col] = Convert.ToDouble(Convert.ToDouble(dt_FC_totals.Rows[row][col]) / Convert.ToInt32(dt_FC_hits.Rows[row][col]));
            }
          }
        }

        return this.DT_FC;
      }
    }
  }
}
