namespace AutoTune
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Linq;
  using System.Text;
  using System.Windows.Forms;

  public class Parser
  {
    public readonly List<double> mafVolts = new List<double>
    {
      0.00, 0.08, 0.16, 0.24, 0.32, 0.40, 0.48, 0.56, 0.64, 0.72, 0.80, 0.88, 0.96, 1.04, 1.12, 1.20, 1.28,
      1.36, 1.44, 1.52, 1.60, 1.68, 1.76, 1.84, 1.92, 2.00, 2.08, 2.16, 2.24, 2.32, 2.40, 2.48, 2.56, 2.64,
      2.72, 2.80, 2.88, 2.96, 3.04, 3.12, 3.20, 3.28, 3.36, 3.44, 3.52, 3.60, 3.68, 3.76, 3.84, 3.92, 4.00,
      4.08, 4.16, 4.24, 4.32, 4.40, 4.48, 4.56, 4.64, 4.72, 4.80, 4.88, 4.96, 5.04
    };

    private List<double> maf1ClosedLoop = new List<double>();
    private List<double> maf2ClosedLoop = new List<double>();
    private List<double> maf1OpenLoop = new List<double>();
    private List<double> maf2OpenLoop = new List<double>();
    private DataTable clDT1 = new DataTable();
    private DataTable clDT2 = new DataTable();
    private DataTable olDT1 = new DataTable();
    private DataTable olDT2 = new DataTable();
    private List<int> hits1 = new List<int>(64);
    private List<int> hits2 = new List<int>(64);
    List<int> tmpRPMlist = BuffDV_FuelComp.FC_RPM;
    List<double> tmpXlist = BuffDV_FuelComp.FC_XdataByte;
    DataTable DT_FC_hits = new DataTable();
    DataTable DT_FC_totals = new DataTable();
    DataTable DT_FC = new DataTable();
    private double accel;
    private double accelChange;
    private double actualAFR1;
    private double actualAFR2;
    private double afr1;
    private double afr2;
    private double coolantTemp;
    private int finaltrim1;
    private int finaltrim2;
    private double OLtrim1;
    private double OLtrim2;
    private int indexFinder1;
    private int indexFinder2;
    private double intakeAirTemp;
    private double intakeAirTempAVG;
    private double longtrim1;
    private double longtrim2;
    private double maf1v;
    private double maf2v = 0;
    private double nextaccel;
    private int nexttime;
    private int shorttrim1;
    private int shorttrim2 = 100;
    private double target;
    private int time;
    private double tmpAdjustment1;
    private double tmpAdjustment2;
    private int totalLines = 0;
    private double upcoming_AFR1;
    private double upcoming_AFR2;
    private string clStatus = string.Empty;
    private string olStatus = string.Empty;
    private int timeDex;
    private int stB1Dex;
    private int stB2Dex;
    private int accelDex;
    private int ltB1Dex;
    private int ltB2Dex;
    private int afrB1Dex;
    private int afrB2Dex;
    private int mafB1Dex;
    private int mafB2Dex;
    private int targetDex;
    private int intakeAirTempDex;
    private int coolantTempDex;
    private int fuelCompTraceDex;
    private int rpmDex;
    private bool dualTB;
    private bool accelAfterDecel;

    public void FindHeader_Indexes(DataGridView tempgrid)
    {
      if (tempgrid.Columns.Contains("Time"))
      {
        this.timeDex = tempgrid.Columns["Time"].Index;
      }
      else
      {
        this.timeDex = -1;
      }

      if (tempgrid.Columns.Contains("A/F CORR-B1 (%)"))
      {
        this.stB1Dex = tempgrid.Columns["A/F CORR-B1 (%)"].Index;
      }
      else
      {
        this.stB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("A/F CORR-B2 (%)"))
      {
        this.stB2Dex = tempgrid.Columns["A/F CORR-B2 (%)"].Index;
      }
      else
      {
        this.stB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("THROTTLE SENSOR 1 - B1(V)"))
      {
        this.accelDex = tempgrid.Columns["THROTTLE SENSOR 1 - B1(V)"].Index;
      }
      else if (tempgrid.Columns.Contains("ACCEL PED POS 1 (V-Accel)"))
      {
        this.accelDex = tempgrid.Columns["ACCEL PED POS 1 (V-Accel)"].Index;
      }
      else
      {
        this.accelDex = -1;
      }

      if (tempgrid.Columns.Contains("LT Fuel Trim B1 (%)"))
      {
        this.ltB1Dex = tempgrid.Columns["LT Fuel Trim B1 (%)"].Index;
      }
      else
      {
        this.ltB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("LT Fuel Trim B2 (%)"))
      {
        this.ltB2Dex = tempgrid.Columns["LT Fuel Trim B2 (%)"].Index;
      }
      else
      {
        this.ltB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("LC-1 (1) AFR"))
      {
        this.afrB1Dex = tempgrid.Columns["LC-1 (1) AFR"].Index;
      }
      else if (tempgrid.Columns.Contains("AFR WB-B1"))
      {
        this.afrB1Dex = tempgrid.Columns["AFR WB-B1"].Index;
      }
      else
      {
        this.afrB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("LC-1 (2) AFR"))
      {
        tempgrid.Columns.Contains("LC-1 (2) AFR");
      }
      else if (tempgrid.Columns.Contains("AFR WB-B2"))
      {
        this.afrB2Dex = tempgrid.Columns["AFR WB-B2"].Index;
      }
      else
      {
        this.afrB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("MAS A/F -B1 (V)"))
      {
        this.mafB1Dex = tempgrid.Columns["MAS A/F -B1 (V)"].Index;
      }
      else
      {
        this.mafB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("MAS A/F -B2 (V)"))
      {
        this.mafB2Dex = tempgrid.Columns["MAS A/F -B2 (V)"].Index;
      }
      else
      {
        this.mafB2Dex = -1;
      }

      if (tempgrid.Columns.Contains("TARGET AFR"))
      {
        this.targetDex = tempgrid.Columns["TARGET AFR"].Index;
      }
      else
      {
        this.targetDex = -1;
      }

      if (tempgrid.Columns.Contains("INTAKE AIR TMP"))
      {
        this.intakeAirTempDex = tempgrid.Columns["INTAKE AIR TMP"].Index;
      }
      else
      {
        this.intakeAirTempDex = -1;
      }

      if (tempgrid.Columns.Contains("COOLANT TEMP"))
      {
        this.coolantTempDex = tempgrid.Columns["COOLANT TEMP"].Index;
      }
      else
      {
        this.coolantTempDex = -1;
      }

      if (tempgrid.Columns.Contains("Fuel Compensation X Trace"))
      {
        this.fuelCompTraceDex = tempgrid.Columns["Fuel Compensation X Trace"].Index;
      }
      else if (tempgrid.Columns.Contains("Fuel Compensation X Trace (%)"))
      {
        this.fuelCompTraceDex = tempgrid.Columns["Fuel Compensation X Trace (%)"].Index;
      }
      else
      {
        this.fuelCompTraceDex = -1;
      }

      if (tempgrid.Columns.Contains("ENGINE RPM (rpm)"))
      {
        this.rpmDex = tempgrid.Columns["ENGINE RPM (rpm)"].Index;
      }
      else
      {
        this.rpmDex = -1;
      }
    }

    public DataTable AdjustMAF(BackgroundWorker bw, DataGridView tempgrid)
    {
      using (DataTable dt = new DataTable())
      {
        // init the adjustment lists and add voltage columns
        foreach (double d in this.mafVolts)
        {
          this.maf1ClosedLoop.Add(100.00);
          this.maf2ClosedLoop.Add(100.00);
          this.maf1OpenLoop.Add(100.00);
          this.maf2OpenLoop.Add(100.00);
          this.hits1.Add(0);
          this.hits2.Add(0);
          this.clDT1.Columns.Add(Convert.ToString(d));
          this.clDT2.Columns.Add(Convert.ToString(d));
          this.olDT1.Columns.Add(Convert.ToString(d));
          this.olDT2.Columns.Add(Convert.ToString(d));
        }

        if (tempgrid.Rows.Count >= 100)
        {
          this.totalLines = tempgrid.Rows.Count;
        }
        else
        {
          MessageBox.Show("The selected CSV log is not long enough.\nPlease log more data and retry.");
          return dt;
        }

        this.FindHeader_Indexes(tempgrid);

        if (Properties.Settings.Default.MAF_IAT && this.intakeAirTempDex != -1)
        {
          this.FindIAT_average(tempgrid);
        }

        // Build first line for the adjustment CL DataTable
        if (this.clDT1.Rows.Count == 0)
        {
          DataRow dr = this.clDT1.NewRow();
          int c = 0;
          foreach (double d in this.mafVolts)
          {
            dr[c] = 1.1;
            ++c;
          }

          this.clDT1.Rows.Add(dr);
        }

        if (this.mafB2Dex != -1)
        {
          this.dualTB = true;

          // Build first line for the adjustment CL DataTable
          if (this.clDT2.Rows.Count == 0)
          {
            DataRow dr = this.clDT2.NewRow();
            int c = 0;
            foreach (double d in this.mafVolts)
            {
              dr[c] = 1.1;
              ++c;
            }

            this.clDT2.Rows.Add(dr);
          }
        }

        // Build first line for the adjustment OL DataTable
        if (this.olDT1.Rows.Count == 0)
        {
          DataRow dr = this.olDT1.NewRow();
          int c = 0;
          foreach (double d in this.mafVolts)
          {
            dr[c] = 1.1;
            ++c;
          }

          this.olDT1.Rows.Add(dr);
        }

        if (this.mafB2Dex != -1)
        {
          this.dualTB = true;

          // Build first line for the adjustment OL DataTable
          if (this.olDT2.Rows.Count == 0)
          {
            DataRow dr = this.olDT2.NewRow();
            int c = 0;
            foreach (double d in this.mafVolts)
            {
              dr[c] = 1.1;
              ++c;
            }

            this.olDT2.Rows.Add(dr);
          }
        }

        if (this.targetDex != -1 && this.mafB1Dex != -1 && this.afrB1Dex != -1 && this.afrB2Dex != -1 && this.coolantTempDex != -1)
        {
          for (int r = 0; r < tempgrid.Rows.Count - 1; ++r)
          {
            // dual MAF check
            if (this.mafB2Dex != -1)
            {
              this.dualTB = true;
            }

            try
            {
              this.target = Convert.ToDouble(tempgrid.Rows[r].Cells[this.targetDex].Value);
              this.maf1v = Convert.ToDouble(tempgrid.Rows[r].Cells[this.mafB1Dex].Value);
              this.afr1 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.afrB1Dex].Value);
              this.afr2 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.afrB2Dex].Value);
            }
            catch
            {
              Console.WriteLine("Error while setting parameter values for row {0}", r);
              continue;
            }

            if (this.timeDex != -1 && this.stB1Dex != -1 && this.stB2Dex != -1)
            {
              try
              {
                this.time = Convert.ToInt32(tempgrid.Rows[r].Cells[this.timeDex].Value);
                this.nexttime = Convert.ToInt32(tempgrid.Rows[r + 1].Cells[this.timeDex].Value);
                this.accel = Convert.ToDouble(tempgrid.Rows[r].Cells[this.accelDex].Value);
                this.nextaccel = Convert.ToDouble(tempgrid.Rows[r + 1].Cells[this.accelDex].Value);
                this.shorttrim1 = Convert.ToInt32(tempgrid.Rows[r].Cells[this.stB1Dex].Value);
                this.shorttrim2 = Convert.ToInt32(tempgrid.Rows[r].Cells[this.stB1Dex].Value);
                this.accelChange = Convert.ToDouble(((this.nextaccel - this.accel) / (this.nexttime - this.time)) * 1000);
                this.coolantTemp = Convert.ToDouble(tempgrid.Rows[r + 1].Cells[this.coolantTempDex].Value);
                this.intakeAirTemp = Convert.ToInt32(tempgrid.Rows[r].Cells[this.intakeAirTempDex].Value);

                if (this.dualTB)
                {
                  this.maf2v = Convert.ToDouble(tempgrid.Rows[r].Cells[this.mafB2Dex].Value);
                }
              }
              catch
              {
                Console.WriteLine(" error while setting parameter values for row {0}", r);
                continue;
              }

              // Back on accel after decel  ** This will skip down rows to avoid skewing values
              if (this.afr1 == 60)
              {
                this.accelAfterDecel = true;
                continue;
              }
              else if (this.afr1 < 20 && this.accelAfterDecel)
              {
                r += 9;
                this.accelAfterDecel = false;
                continue;
              }

              // Closed loop
              if (this.target == 14.7 && this.coolantTemp > 176)
              {
                // Dual throttle bodies and have logged long term trim
                if (this.ltB1Dex != -1 && this.dualTB)
                {
                  this.longtrim1 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.ltB1Dex].Value);
                  this.longtrim2 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.ltB2Dex].Value);
                  this.finaltrim1 = (this.shorttrim1 + Convert.ToInt32(this.longtrim1)) / 2;
                  this.finaltrim2 = (this.shorttrim2 + Convert.ToInt32(this.longtrim2)) / 2;
                }

                // Dual throttle bodies and have NOT logged long term trim
                else if (this.ltB1Dex == -1 && this.dualTB)
                {
                  this.finaltrim1 = this.shorttrim1;
                  this.finaltrim2 = this.shorttrim2;
                }

                // Single throttle body and have logged long term trim
                else if (this.ltB1Dex != -1 && !this.dualTB)
                {
                  this.longtrim1 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.ltB1Dex].Value);
                  this.longtrim2 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.ltB2Dex].Value);
                  this.finaltrim1 = (this.shorttrim1 + Convert.ToInt32(this.longtrim1)) / 2;
                  this.finaltrim2 = (this.shorttrim2 + Convert.ToInt32(this.longtrim2)) / 2;
                  this.finaltrim1 = (this.finaltrim1 + this.finaltrim2) / 2;
                }
                else
                {
                  this.finaltrim1 = (this.shorttrim1 + this.shorttrim2) / 2;
                  this.finaltrim2 = 100;
                }

                this.indexFinder1 = this.mafVolts.BinarySearch(this.maf1v);
                if (this.indexFinder1 < 0)
                {
                  this.indexFinder1 = ~this.indexFinder1;
                }

                if (this.dualTB)
                {
                  this.indexFinder2 = this.mafVolts.BinarySearch(this.maf2v);
                  if (this.indexFinder2 < 0)
                  {
                    this.indexFinder2 = ~this.indexFinder2;
                  }
                }

                // CLOSED LOOP
                //  filter out intake air temp changes  &&  filter out quick accel pedal position changes
                if (Properties.Settings.Default.MAF_IAT && this.intakeAirTempDex != -1
                        && (this.intakeAirTemp >= this.intakeAirTempAVG - 10 && this.intakeAirTemp <= this.intakeAirTempAVG + 10)
                        && Properties.Settings.Default.MAF_ACCEL && this.accelChange > -0.1 && this.accelChange < 0.1
                        && this.indexFinder1 >= 0 && this.indexFinder1 < this.mafVolts.Count)
                {
                  this.ClosedLoop_Start();
                }

                // Filter out quick accel pedal position changes
                else if ((!Properties.Settings.Default.MAF_IAT || this.intakeAirTempDex == -1)
                        && Properties.Settings.Default.MAF_ACCEL && this.accelChange > -0.1 && this.accelChange < 0.1
                        && this.indexFinder1 >= 0 && this.indexFinder1 < this.mafVolts.Count)
                {
                  this.ClosedLoop_Start();
                }

                // Filter out intake air temp changes
                else if (Properties.Settings.Default.MAF_IAT && this.intakeAirTempDex != -1
                        && (this.intakeAirTemp >= this.intakeAirTempAVG - 10 && this.intakeAirTemp <= this.intakeAirTempAVG + 10)
                        && !Properties.Settings.Default.MAF_ACCEL && this.indexFinder1 >= 0 && this.indexFinder1 < this.mafVolts.Count)
                {
                  this.ClosedLoop_Start();
                }
              }

              // Open loop
              else if (this.target < 14.7 && this.shorttrim1 == 100 && this.afr1 < 20 && this.coolantTemp > 176 && Properties.Settings.Default.MAF_OL)
              {
                this.OLtrim1 = this.afr1 / this.target;
                this.OLtrim2 = this.afr2 / this.target;

                this.indexFinder1 = this.mafVolts.BinarySearch(this.maf1v);
                if (this.indexFinder1 < 0)
                {
                  this.indexFinder1 = ~this.indexFinder1;
                }

                if (this.dualTB)
                {
                  this.indexFinder2 = this.mafVolts.BinarySearch(this.maf2v);
                  if (this.indexFinder2 < 0)
                  {
                    this.indexFinder2 = ~this.indexFinder2;
                  }
                }

                // Test "actual AFR"
                if (r > 2)
                {
                  this.actualAFR1 = 0;
                  this.actualAFR2 = 0;

                  try
                  {
                    this.actualAFR1 = Convert.ToDouble(tempgrid.Rows[r + 3].Cells[this.afrB1Dex].Value);
                    if (this.dualTB)
                    {
                      this.actualAFR2 = Convert.ToDouble(tempgrid.Rows[r + 3].Cells[this.afrB2Dex].Value);
                    }
                  }
                  catch
                  {
                    Console.WriteLine(" error while actualAFR values for row {0}", r);
                    continue;
                  }
                }

                this.OpenLoop_Start();
              }
            }
            else
            {
              StringBuilder sb = new StringBuilder();
              sb.Append("Could not find the following headers: \n");
              if (this.timeDex == -1) {sb.Append("Time\n"); }
              if (this.stB1Dex == -1) {sb.Append("A/F CORR-B1 (%)\n"); }
              if (this.stB2Dex == -1) {sb.Append("A/F CORR-B2 (%)\n"); }
              if (this.accelDex == -1) {sb.Append("ACCEL PED POS 1\n"); }
              if (this.ltB1Dex == -1) {sb.Append("LT Fuel Trim B1 (%)\n"); }
              if (this.ltB2Dex == -1) {sb.Append("LT Fuel Trim B2 (%)\n"); }
              if (this.afrB1Dex == -1) {sb.Append("AFR WB-B1\n"); }
              if (this.afrB2Dex == -1) {sb.Append("AFR WB-B2\n"); }
              if (this.mafB1Dex == -1) {sb.Append("MAS A/F -B1 (V)\n"); }
              if (this.mafB2Dex == -1) {sb.Append("MAS A/F -B2 (V)\n"); }
              if (this.targetDex == -1) {sb.Append("TARGET AFR\n"); }
              if (this.intakeAirTempDex == -1) {sb.Append("INTAKE AIR TMP\n"); }
              if (this.coolantTempDex == -1) {sb.Append("COOLANT TEMP\n"); }

              Console.WriteLine(sb.ToString());
              MessageBox.Show("Error", "We could not find minimal parameters needed\nto calculate the MAF scaling adjustments.\n"
                + sb.ToString() + "\n Please add these parameters to the uprev logger and try again.");
            }
          }

          // END of looping rows
          // NOW start reading valuses from DT
          this.ClosedLoop_Finish();

          this.OpenLoop_Finish();

          // Build DataTable for returning values
          dt.Columns.Add("Voltage", typeof(double));
          dt.Columns.Add("ClosedLoop_B1", typeof(double));
          dt.Columns.Add("ClosedLoop_B2", typeof(double));
          dt.Columns.Add("Hits_B1", typeof(int));
          dt.Columns.Add("Hits_B2", typeof(int));
          for (int i = 0; i < this.mafVolts.Count; ++i)
          {
            DataRow dr = dt.NewRow();
            dr[0] = (double)this.mafVolts[i];
            if (this.maf1ClosedLoop[i] == 100 && this.maf1OpenLoop[i] != 100)
            {
              dr[1] = (double)this.maf1OpenLoop[i];
            }
            else
            {
              dr[1] = (double)this.maf1ClosedLoop[i];
            }

            if (this.maf2ClosedLoop[i] == 100 && this.maf2OpenLoop[i] != 100)
            {
              dr[2] = (double)this.maf2OpenLoop[i];
            }
            else
            {
              dr[2] = (double)this.maf2ClosedLoop[i];
            }

            dr[3] = (int)this.hits1[i];
            dr[4] = (int)this.hits2[i];
            dt.Rows.Add(dr);
          }
        }
        else
        {
          StringBuilder sb = new StringBuilder();
          sb.Append("Could not find the following headers: \n");
          if (this.timeDex == -1) { sb.Append("Time\n"); }
          if (this.stB1Dex == -1) { sb.Append("A/F CORR-B1 (%)\n"); }
          if (this.stB2Dex == -1) { sb.Append("A/F CORR-B2 (%)\n"); }
          if (this.accelDex == -1) { sb.Append("ACCEL PED POS 1\n"); }
          if (this.ltB1Dex == -1) { sb.Append("LT Fuel Trim B1 (%)\n"); }
          if (this.ltB2Dex == -1) { sb.Append("LT Fuel Trim B2 (%)\n"); }
          if (this.afrB1Dex == -1) { sb.Append("AFR WB-B1\n"); }
          if (this.afrB2Dex == -1) { sb.Append("AFR WB-B2\n"); }
          if (this.mafB1Dex == -1) { sb.Append("MAS A/F -B1 (V)\n"); }
          if (this.mafB2Dex == -1) { sb.Append("MAS A/F -B2 (V)\n"); }
          if (this.targetDex == -1) { sb.Append("TARGET AFR\n"); }
          if (this.intakeAirTempDex == -1) { sb.Append("INTAKE AIR TMP\n"); }
          if (this.coolantTempDex == -1) { sb.Append("COOLANT TEMP\n"); }

          Console.WriteLine(sb.ToString());
          MessageBox.Show("Error", "We could not find minimal parameters needed\nto calculate Closed Loop MAF scaling adjustments.\n" + sb.ToString());
        }

        return dt;
      }
    }

    private void ClosedLoop_Start()
    {
      this.dualTB = this.dualTB && this.indexFinder2 >= 0 && this.indexFinder2 <= this.mafVolts.Count ? true : false;

      // DUAL MAF
      if (this.dualTB)
      {
        // MAF1
        for (int i = 0; ; )
        {
          // Find empty spot to insert value in DataTable
          if (i == this.clDT1.Rows.Count - 1 || this.clDT1.Rows.Count == 0)
          {
            DataRow dr = this.clDT1.NewRow();
            int c = 0;
            foreach (double d in this.mafVolts)
            {
              dr[c] = 1.1;
              ++c;
            }

            this.clDT1.Rows.Add(dr);
          }

          double cell1 = Convert.ToDouble(this.clDT1.Rows[i][this.indexFinder1]);
          if (this.finaltrim1 < 75 || this.finaltrim1 > 125)
          {
            break;
          }

          if (cell1 == 1.1)
          {
            this.clDT1.Rows[i][this.indexFinder1] = this.finaltrim1;
            break;
          }
          else
          {
            ++i;
          }
        }

        // MAF 2
        for (int i = 0; ; )
        {
          // Find empty spot to insert value in DataTable 2
          if (i == this.clDT2.Rows.Count - 1 || this.clDT2.Rows.Count == 0)
          {
            DataRow dr = this.clDT2.NewRow();
            int c = 0;
            foreach (double d in this.mafVolts)
            {
              dr[c] = 1.1;
              ++c;
            }

            this.clDT2.Rows.Add(dr);
          }

          double cell2 = Convert.ToDouble(this.clDT2.Rows[i][this.indexFinder2]);
          if (this.finaltrim1 < 75 || this.finaltrim1 > 125)
          {
            break;
          }

          if (cell2 == 1.1)
          {
            this.clDT2.Rows[i][this.indexFinder2] = this.finaltrim2;
            break;
          }
          else
          {
            ++i;
          }
        }
      }

      // Single MAF
      else
      {
        for (int i = 0; ;)
        {
          // find empty spot to insert value in DataTable
          double cell1 = Convert.ToDouble(this.clDT1.Rows[i][this.indexFinder1]);

          if (i == this.clDT1.Rows.Count - 1 || this.clDT1.Rows.Count == 0)
          {
            DataRow dr = this.clDT1.NewRow();
            int c = 0;
            foreach (double d in this.mafVolts)
            {
              dr[c] = 1.1;
              ++c;
            }

            this.clDT1.Rows.Add(dr);
          }

          if (this.finaltrim1 < 75 || this.finaltrim1 > 125)
          {
            break;
          }

          if (cell1 == 1.1)
          {
            this.clDT1.Rows[i][this.indexFinder1] = this.finaltrim1;
            break;
          }
          else
          {
            ++i;
          }
        }
      }
    }

    private void ClosedLoop_Finish()
    {
      if (this.dualTB)
      {
        // MAF1 
        for (int c = 0; c < this.clDT1.Columns.Count - 1; ++c) 
        {
          // Read values from DataTable
          List<double> tmpList = new List<double>();
          for (int line = 0; line < this.clDT1.Rows.Count - 1; ++line)
          {
            double cell = Convert.ToDouble(this.clDT1.Rows[line][c]);
            if (cell != 1.1)
            {
              tmpList.Add(Convert.ToDouble(this.clDT1.Rows[line][c]));
            }
            else
            {
              break;
            }
          }

          // Shows how many hits were for each voltage
          if (this.hits1[c] == 0)
          {
            this.hits1[c] = tmpList.Count;
          }
          else
          {
            this.hits1[c] += tmpList.Count;
          }

          if (tmpList.Count > 5)
          {
            this.maf1ClosedLoop[c] = (double)tmpList.Average();
          }
          else
          {
            this.maf1ClosedLoop[c] = 100;
          }
        }

        // MAF2
        for (int c = 0; c < this.clDT2.Columns.Count - 1; ++c)
        {
          // Read values from DataTable 2
          List<double> tmpList = new List<double>();
          for (int line = 0; line < this.clDT2.Rows.Count - 1; ++line)
          {
            double cell2 = Convert.ToDouble(this.clDT2.Rows[line][c]);
            if (cell2 != 1.1)
            {
              tmpList.Add(Convert.ToDouble(this.clDT2.Rows[line][c]));
            }
            else
            {
              break;
            }
          }

          // Shows how many hits were for each voltage
          if (this.hits2[c] == 0)
          {
            this.hits2[c] = tmpList.Count;
          }
          else
          {
            this.hits2[c] += tmpList.Count;
          }

          if (tmpList.Count > 5)
          {
            this.maf2ClosedLoop[c] = (double)tmpList.Average();
          }
          else
          {
            this.maf2ClosedLoop[c] = 100;
          }
        }
      }

      // Single MAF
      else
      {
        // read values from DataTable
        for (int c = 0; c < this.clDT1.Columns.Count - 1; ++c)
        {
          List<double> tmpList = new List<double>();
          for (int line = 0; line < this.clDT1.Rows.Count - 1; ++line)
          {
            double cell = Convert.ToDouble(this.clDT1.Rows[line][c]);
            if (cell != 1.1)
            {
              tmpList.Add(Convert.ToDouble(this.clDT1.Rows[line][c]));
            }
            else
            {
              break;
            }
          }

          // Shows how many hits were for each voltage
          if (this.hits1[c] == 0)
          {
            this.hits1[c] = tmpList.Count;
          }
          else
          {
            this.hits1[c] += tmpList.Count;
          }

          if (tmpList.Count > 5)
          {
            this.maf1ClosedLoop[c] = (double)tmpList.Average();
          }
          else
          {
            this.maf1ClosedLoop[c] = 100;
          }
        }
      }
    }

    private void OpenLoop_Start()
    {
      this.dualTB = this.dualTB && this.mafB2Dex != -1 ? true : false;

      // MAF 1 - write values to datatable
      for (int i = 0; ;)
      {
        double cell1 = Convert.ToDouble(this.olDT1.Rows[i][this.indexFinder1]);

        if (this.actualAFR1 != 0)
        {
          this.tmpAdjustment1 = (this.actualAFR1 / this.target) * 100;
        }

        // Add extra row if close to the end
        if (i == this.olDT1.Rows.Count - 1 || this.clDT1.Rows.Count == 0)
        {
          DataRow dr = this.olDT1.NewRow();
          int c = 0;
          foreach (double d in this.mafVolts)
          {
            dr[c] = 1.1;
            ++c;
          }

          this.olDT1.Rows.Add(dr);
        }

        if (cell1 == 1.1 && this.actualAFR1 != 0 && this.indexFinder1 >= 0 && this.indexFinder1 < this.mafVolts.Count)
        {
          this.olDT1.Rows[i][this.indexFinder1] = this.tmpAdjustment1;
          break;
        }
        else
        {
          ++i;
        }
      }

      if (!this.dualTB)
      {
        return;
      }

      // MAF 2 - write values to datatable
      if (this.actualAFR2 != 0)
      {
        this.tmpAdjustment2 = (this.actualAFR2 / this.target) * 100;
      }

      for (int i = 0; ;)
      {
        double cell2 = Convert.ToDouble(this.olDT2.Rows[i][this.indexFinder2]);

        // Add extra row if close to the end
        if (i == this.olDT2.Rows.Count - 1)
        {
          DataRow dr = this.olDT2.NewRow();
          int c = 0;
          foreach (double d in this.mafVolts)
          {
            dr[c] = 1.1;
            ++c;
          }

          this.olDT2.Rows.Add(dr);
        }

        if (cell2 == 1.1 && this.actualAFR2 != 0 && this.indexFinder1 >= 0 && this.indexFinder1 < this.mafVolts.Count)
        {
          this.olDT2.Rows[i][this.indexFinder2] = this.tmpAdjustment2;
          break;
        }
        else
        {
          ++i;
        }
      }
    }

    private void OpenLoop_Finish()
    {
      // MAF1 - Read values from datatable
      for (int c = 0; c < this.olDT1.Columns.Count - 1; ++c)
      {
        // Read values from DataTable
        List<double> tmpList = new List<double>();
        for (int line = 0; line < this.olDT1.Rows.Count - 1; ++line)
        {
          double cell = Convert.ToDouble(this.olDT1.Rows[line][c]);
          if (cell != 1.1)
          {
            tmpList.Add(Convert.ToDouble(this.olDT1.Rows[line][c]));
          }
          else
          {
            break;
          }
        }

        // Shows how many hits were for each voltage
        if (this.hits1[c] == 0)
        {
          this.hits1[c] = tmpList.Count;
        }
        else
        {
          this.hits1[c] += tmpList.Count;
        }

        if (tmpList.Count > 5)
        {
          this.maf1OpenLoop[c] = (double)tmpList.Average();
        }
        else
        {
          this.maf1OpenLoop[c] = 100;
        }
      }

      if (!this.dualTB)
      {
        return;
      }

      // MAF2 - Read vales from datatable
      for (int c = 0; c < this.olDT2.Columns.Count - 1; ++c)
      {
        // Read values from DataTable 2
        List<double> tmpList = new List<double>();
        for (int line = 0; line < this.olDT2.Rows.Count - 1; ++line)
        {
          double cell2 = Convert.ToDouble(this.olDT2.Rows[line][c]);
          if (cell2 != 1.1)
          {
            tmpList.Add(Convert.ToDouble(this.olDT2.Rows[line][c]));
          }
          else
          {
            break;
          }
        }

        // Shows how many hits were for each voltage
        if (this.hits2[c] == 0)
        {
          this.hits2[c] = tmpList.Count;
        }
        else
        {
          this.hits2[c] += tmpList.Count;
        }

        if (tmpList.Count > 5)
        {
          this.maf2OpenLoop[c] = (double)tmpList.Average();
        }
        else
        {
          this.maf2OpenLoop[c] = 100;
        }
      }
    }

    private void SetConfig()
    {
      if (Properties.Settings.Default.MAF_CL
      && !Properties.Settings.Default.MAF_IAT
      && !Properties.Settings.Default.MAF_ACCEL
      && this.targetDex != -1 && this.mafB1Dex != -1
      && this.afrB1Dex != -1 && this.afrB2Dex != -1
      && this.coolantTempDex != -1)
      {
        this.clStatus = "CL_Basic";
      }
      else if (Properties.Settings.Default.MAF_CL
      && Properties.Settings.Default.MAF_IAT
      && !Properties.Settings.Default.MAF_ACCEL
      && this.targetDex != -1 && this.mafB1Dex != -1
      && this.afrB1Dex != -1 && this.afrB2Dex != -1
      && this.coolantTempDex != -1
      && this.intakeAirTempDex != -1)
      {
        this.clStatus = "CL_IAT";
      }
      else if (Properties.Settings.Default.MAF_CL
      && !Properties.Settings.Default.MAF_IAT
      && Properties.Settings.Default.MAF_ACCEL
      && this.targetDex != -1 && this.mafB1Dex != -1
      && this.afrB1Dex != -1 && this.afrB2Dex != -1
      && this.coolantTempDex != -1
      && this.accelDex != -1)
      {
        this.clStatus = "CL_ACCEL";
      }
      else if (Properties.Settings.Default.MAF_CL
      && Properties.Settings.Default.MAF_IAT
      && Properties.Settings.Default.MAF_ACCEL
      && this.targetDex != -1 && this.mafB1Dex != -1
      && this.afrB1Dex != -1 && this.afrB2Dex != -1
      && this.coolantTempDex != -1
      && this.intakeAirTempDex != -1
      && this.accelDex != -1)
      {
        this.clStatus = "CL_Full";
      }
      else
      {
        this.clStatus = "Error";
      }

      if (Properties.Settings.Default.MAF_OL
      && this.targetDex != -1 && this.mafB1Dex != -1
      && this.afrB1Dex != -1 && this.afrB2Dex != -1
      && this.coolantTempDex != -1)
      {
        this.olStatus = "OL_Full";
      }
      else
      {
        this.olStatus = "Error";
      }
    }

      private void FindIAT_average(DataGridView tempgrid)
    {
      List<double> iatFull = new List<double>();

      for (int r = 0; r < tempgrid.Rows.Count - 1; ++r)
      {
        double intakeAirTemp = Convert.ToDouble(tempgrid.Rows[r].Cells[this.intakeAirTempDex].Value);
        iatFull.Add(intakeAirTemp);
      }

      this.intakeAirTempAVG = (double)iatFull.Average();
      Console.WriteLine("The average Intake Air Temp is: " + Convert.ToString(this.intakeAirTempAVG));
    }

    // *********** Fuel Comp Adjustments below.
    public DataTable AdjustFuelComp(BackgroundWorker bw, DataGridView tempgrid)
    {
      this.FindHeader_Indexes(tempgrid);

      if (this.targetDex != -1 && this.fuelCompTraceDex != -1
        && this.rpmDex != -1 && tempgrid.Rows.Count > 50
        && this.afrB1Dex != -1 && this.afrB2Dex != -1)
      {
        Console.WriteLine(" total Lines are  {0}", tempgrid.Rows.Count);
        double afr1;
        double afr2;
        int finaltrim1;
        int finaltrim2;
        double fuelXtrace;
        double longtrim1;
        double longtrim2;
        int rpm;
        int shorttrim1;
        int shorttrim2 = 100;
        double target;
        double tmpAdjustment1;
        int indexFinderDB;
        int indexFinderRPM;
        bool accelAfterDecel = false;

        foreach (int i in tmpXlist)
        {
          this.DT_FC_hits.Columns.Add(Convert.ToString(i), typeof(int));
          this.DT_FC_totals.Columns.Add(Convert.ToString(i), typeof(double));
          this.DT_FC.Columns.Add(Convert.ToString(i), typeof(decimal));
        }

        foreach (int i in tmpRPMlist)
        {
          this.DT_FC_hits.Rows.Add();
          this.DT_FC_totals.Rows.Add();
          this.DT_FC.Rows.Add();
        }

        for (int row = 0; row < this.DT_FC_totals.Rows.Count; ++row)
        {
          for (int col = 0; col < this.DT_FC_totals.Columns.Count; ++col)
          {
            this.DT_FC_totals.Rows[row][col] = "100";
            this.DT_FC_hits.Rows[row][col] = "1";
            this.DT_FC.Rows[row][col] = "100";
          }
        }

        for (int row = 1; row < tempgrid.Rows.Count; ++row)
        {
          target = Convert.ToDouble(tempgrid.Rows[row].Cells[this.targetDex].Value);
          fuelXtrace = Convert.ToDouble(tempgrid.Rows[row].Cells[this.fuelCompTraceDex].Value);
          rpm = Convert.ToInt32(tempgrid.Rows[row].Cells[this.rpmDex].Value);
          afr1 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.afrB1Dex].Value);
          afr2 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.afrB2Dex].Value);

          if (target > 15 || rpm < 600)
          {
            continue;
          }

          // Back on accel after decel  ** This will skip down rows to avoid skewing values
          if (afr1 == 60)
          {
            accelAfterDecel = true;
            continue;
          }
          else if (afr1 < 20 && accelAfterDecel)
          {
            row += 9;
            accelAfterDecel = false;
            continue;
          }

          indexFinderDB = tmpXlist.BinarySearch(fuelXtrace);
          if (indexFinderDB < 0)
          {
            indexFinderDB = ~indexFinderDB;
          }

          indexFinderRPM = tmpRPMlist.BinarySearch(rpm);
          if (indexFinderRPM < 0)
          {
            indexFinderRPM = ~indexFinderRPM;
          }

          if (indexFinderDB == 16)
          {
            indexFinderDB = 15;
          }

          if (indexFinderRPM == 16)
          {
            indexFinderRPM = 15;
          }

          if (this.stB1Dex != -1 && this.stB2Dex != -1 && afr1 < 25 && afr2 < 25)
          {
            shorttrim1 = Convert.ToInt32(tempgrid.Rows[row].Cells[this.stB1Dex].Value);
            shorttrim2 = Convert.ToInt32(tempgrid.Rows[row].Cells[this.stB2Dex].Value);

            // if long term trimlogged
            if (this.ltB1Dex != -1 && this.ltB2Dex != -1)
            {
              longtrim1 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.ltB1Dex].Value);
              longtrim2 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.ltB2Dex].Value);
              finaltrim1 = (shorttrim1 + Convert.ToInt32(longtrim1)) / 2;
              finaltrim2 = (shorttrim2 + Convert.ToInt32(longtrim2)) / 2;
              finaltrim1 = (finaltrim1 + finaltrim2) / 2;
            }
            else
            {
              finaltrim1 = (shorttrim1 + shorttrim2) / 2;
            }
          }
          else
          {
            continue;
          }

          if (target == 14.7)
          {
            actualAFR1 = Convert.ToDouble(tempgrid.Rows[row + 2].Cells[this.afrB1Dex].Value);
            if (this.dualTB && afr2 < 14.7)
            {
              actualAFR2 = Convert.ToDouble(tempgrid.Rows[row + 2].Cells[this.afrB2Dex].Value);
            }
            else
            {
              actualAFR2 = 0;
            }

            if (actualAFR1 != 0 && actualAFR2 != 0)
            {
              tmpAdjustment1 = ((actualAFR1 + actualAFR2) / target) * 100;
            }
            else
            {
              tmpAdjustment1 = (actualAFR1 / target) * 100;
            }

            if (this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] == "100")
            {
              this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] = tmpAdjustment1;
              this.DT_FC_hits.Rows[indexFinderRPM][indexFinderDB] = "2";
            }
            else
            {
              double test = Convert.ToDouble(this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB]);
              this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] = Convert.ToDouble(this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB]) + tmpAdjustment1;
              int hitCount = Convert.ToInt32(this.DT_FC_hits.Rows[indexFinderRPM][indexFinderDB]);
              this.DT_FC_hits.Rows[indexFinderRPM][indexFinderDB] = Convert.ToString(hitCount + 1);
            }
          }
          else if (target < 14.7 && afr1 < 25 && afr2 < 25)
          {
            actualAFR1 = Convert.ToDouble(tempgrid.Rows[row + 2].Cells[this.afrB1Dex].Value);
            if (this.dualTB && afr2 < 14.7)
            {
              actualAFR2 = Convert.ToDouble(tempgrid.Rows[row + 2].Cells[this.afrB2Dex].Value);
            }
            else
            {
              actualAFR2 = 0;
            }

            if (actualAFR1 != 0 && actualAFR2 != 0)
            {
              tmpAdjustment1 = ((actualAFR1 + actualAFR2) / target) * 100;
            }
            else
            {
              tmpAdjustment1 = (actualAFR1 / target) * 100;
            }

            if (this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] == null
            || string.IsNullOrEmpty(this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB].ToString()))
            {
              this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] = tmpAdjustment1 + 100;
              this.DT_FC_hits.Rows[indexFinderRPM][indexFinderDB] = 2;
            }
            else
            {
              double test = Convert.ToDouble(this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB]);
              this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] = Convert.ToDouble(this.DT_FC_totals.Rows[indexFinderRPM][indexFinderDB]) + tmpAdjustment1;
              int hitCount = Convert.ToInt32(this.DT_FC_hits.Rows[indexFinderRPM][indexFinderDB]);
              this.DT_FC_hits.Rows[indexFinderRPM][indexFinderDB] = hitCount + 1;
            }
          }
        }

        for (int row = 0; row < this.DT_FC_totals.Rows.Count; ++row)
        {
          for (int col = 0; col < this.DT_FC_totals.Columns.Count; ++col)
          {
            double total = 100;
            int hits = 1;
            if (this.DT_FC_totals.Rows[row][col].ToString() == "0" || this.DT_FC_totals.Rows[row][col].ToString() == "100" || string.IsNullOrEmpty(this.DT_FC_totals.Rows[row][col].ToString()))
            {
              total = 100;
            }
            else if (this.DT_FC_totals.Rows[row][col] != null || !string.IsNullOrEmpty(this.DT_FC_totals.Rows[row][col].ToString()))
            {
              string totalvalue = Convert.ToString(this.DT_FC_totals.Rows[row][col]);
              total = Convert.ToDouble(totalvalue);
            }

            if (this.DT_FC_hits.Rows[row][col].ToString() == "0" || this.DT_FC_hits.Rows[row][col].ToString() == "1" || string.IsNullOrEmpty(this.DT_FC_hits.Rows[row][col].ToString()))
            {
              hits = 1;
            }
            else if (this.DT_FC_hits.Rows[row][col] != null || !string.IsNullOrEmpty(this.DT_FC_hits.Rows[row][col].ToString()))
            {
              hits = Convert.ToInt32(this.DT_FC_hits.Rows[row][col]);
            }

            if (total ==100 && hits == 1)
            {
              this.DT_FC.Rows[row][col] = 100;
            }
            else
            {
              this.DT_FC.Rows[row][col] = Convert.ToDouble(Convert.ToDouble(this.DT_FC_totals.Rows[row][col]) / Convert.ToInt32(this.DT_FC_hits.Rows[row][col]));
            }
          }
        }
      }
      else
      {
        StringBuilder sb = new StringBuilder();
        sb.Append("Could not find the following headers: \n");
        if (this.timeDex == -1) {sb.Append("Time\n"); }
        if (this.stB1Dex == -1) {sb.Append("A/F CORR-B1 (%)\n"); }
        if (this.stB2Dex == -1) {sb.Append("A/F CORR-B2 (%)\n"); }
        if (this.accelDex == -1) {sb.Append("ACCEL PED POS 1\n"); }
        if (this.ltB1Dex == -1) {sb.Append("LT Fuel Trim B1 (%)\n"); }
        if (this.ltB2Dex == -1) {sb.Append("LT Fuel Trim B2 (%)\n"); }
        if (this.afrB1Dex == -1) {sb.Append("AFR WB-B1\n"); }
        if (this.afrB2Dex == -1) {sb.Append("AFR WB-B2\n"); }
        if (this.mafB1Dex == -1) {sb.Append("MAS A/F -B1 (V)\n"); }
        if (this.mafB2Dex == -1) {sb.Append("MAS A/F -B2 (V)\n"); }
        if (this.targetDex == -1) {sb.Append("TARGET AFR\n"); }
        if (this.intakeAirTempDex == -1) {sb.Append("INTAKE AIR TMP\n"); }
        if (this.coolantTempDex == -1) {sb.Append("COOLANT TEMP\n"); }
        if (this.fuelCompTraceDex == -1) {sb.Append("Fuel Compensation X Trace\n"); }
        if (this.rpmDex == -1) {sb.Append("ENGINE RPM (rpm)\n"); }

        Console.WriteLine(sb.ToString());
      }

      return DT_FC;
    }
  }
}
