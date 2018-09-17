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
    public List<double> mafVolts = new List<double>
    {
      0.08, 0.16, 0.23, 0.31, 0.39, 0.47, 0.55, 0.63, 0.70, 0.78, 0.86, 0.94, 1.02, 1.09, 1.17, 1.25, 1.33,
      1.41, 1.48, 1.56, 1.64, 1.72, 1.80, 1.88, 1.95, 2.03, 2.11, 2.19, 2.27, 2.34, 2.42, 2.50, 2.58, 2.66,
      2.73, 2.81, 2.89, 2.97, 3.05, 3.13, 3.20, 3.28, 3.36, 3.44, 3.52, 3.59, 3.67, 3.75, 3.83, 3.91, 3.98,
      4.06, 4.14, 4.22, 4.30, 4.38, 4.45, 4.53, 4.61, 4.69, 4.77, 4.84, 4.92, 5.00
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
    private double accel;
    private double accelChange;
    private double actualAFR1;
    private double actualAFR2;
    private double afr1;
    private double afr2;
    private double coolantTemp;
    private int finaltrim1;
    private int finaltrim2;
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

      if (tempgrid.Columns.Contains("ACCEL PED POS 1 (V-Accel)"))
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

      if (tempgrid.Columns.Contains("AFR WB-B1"))
      {
        this.afrB1Dex = tempgrid.Columns["AFR WB-B1"].Index;
      }
      else if (tempgrid.Columns.Contains("LC-1 (1) AFR"))
      {
        this.afrB1Dex = tempgrid.Columns["LC-1 (1) AFR"].Index;
      }
      else
      {
        this.afrB1Dex = -1;
      }

      if (tempgrid.Columns.Contains("AFR WB-B2"))
      {
        this.afrB2Dex = tempgrid.Columns["AFR WB-B2"].Index;
      }
      else if (tempgrid.Columns.Contains("LC-1 (2) AFR"))
      {
        tempgrid.Columns.Contains("LC-1 (2) AFR");
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

    public DataTable AdjustMAF_CL(BackgroundWorker bw, DataGridView tempgrid)
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

        // Build first line for the adjustment DataTable
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

          // Build first line for the adjustment DataTable
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
            }
            else
            {
              StringBuilder sb = new StringBuilder();
              sb.Append("Could not find the following headers: \n");
              if (this.timeDex == -1)
              {
                sb.Append("Time\n");
              }

              if (this.stB1Dex == -1)
              {
                sb.Append("A/F CORR-B1 (%)\n");
              }

              if (this.stB2Dex != -1)
              {
                sb.Append("A/F CORR-B2 (%)\n");
              }

              Console.WriteLine(sb.ToString());
              MessageBox.Show("Error", "We could not find minimal parameters needed\nto calculate the MAF scaling adjustments.\n{0}" + sb.ToString());
            }

            /*  if (target < 14.7 && afrB1Dex != -1 && afrB2Dex != -1 && mafB1Dex != -1 && indexFinder1 >= 0 && indexFinder1 < maf_volts.Count)
              {
                OpenLoop_Start();
              }  */

          }

           // END of looping rows
          // NOW start reading valuses from DT
          this.ClosedLoop_Finish();

          // OpenLoop_Finish();

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
            dr[1] = (double)this.maf1ClosedLoop[i];
            dr[2] = (double)this.maf2ClosedLoop[i];
            dr[3] = (int)this.hits1[i];
            dr[4] = (int)this.hits2[i];
            dt.Rows.Add(dr);
          }
        }
        else
        {
          StringBuilder sb = new StringBuilder();
          sb.Append("Could not find the following headers: \n");
          if (this.targetDex == -1)
          {
            sb.Append("Target AFR\n");
          }

          if (this.mafB1Dex == -1)
          {
            sb.Append("MAS A/F -B2 (V)\n");
          }

          if (this.afrB1Dex != -1)
          {
            sb.Append("AFR WB-B1 / LC-1 (1) AFR\n");
          }

          if (this.afrB2Dex != -1)
          {
            sb.Append("AFR WB-B2 / LC-1 (2) AFR\n");
          }

          if (this.coolantTempDex != -1)
          {
            sb.Append("Coolant Temp\n");
          }

          Console.WriteLine(sb.ToString());
          MessageBox.Show("Error", "We could not find minimal parameters needed\nto calculate Closed Loop MAF scaling adjustments.\n" + sb.ToString());
        }

        return dt;
      }
    }

    public DataTable AdjustMAF_CL_test(BackgroundWorker bw, DataGridView tempgrid)
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

        this.FindHeader_Indexes(tempgrid);
        this.SetConfig();

        if (tempgrid.Rows.Count >= 50)
        {
          this.totalLines = tempgrid.Rows.Count;
        }
        else
        {
          MessageBox.Show("The selected CSV log is not long enough.\nPlease log more data and retry.");
          return dt;
        }

        if (this.coolantTempDex == -1)
        {
          MessageBox.Show("Coolant Temp", "Could not find the coolant temperature values.\nPlease verify you are logging the coolant temperatures.");
          return dt;
        }

        if (this.clStatus == "Error" && this.olStatus == "Error")
        {
          Console.WriteLine("Could not find the required parameters for MAF scaling.\nPlease log more parameters if MAF scaling is needed.");
          return dt;
        }

        if (this.clStatus == "CL_Full" || this.clStatus == "CL_IAT") //if (Properties.Settings.Default.MAF_CL && Properties.Settings.Default.MAF_IAT && intakeAirTempDex != -1)
        {
          this.FindIAT_average(tempgrid);
        }

        if (this.olDT1.Rows.Count == 0)  //Build first line for the adjustment DataTable
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
          this.dualTB = true;  // Set dual MAF bool
          if (this.clDT2.Rows.Count == 0)  //Build first line for the adjustment DataTable
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

        // if Closed Loop option is true and log has required parameters
        if (this.clStatus != "Error")
        {
          for (int r = 0; r < tempgrid.Rows.Count - 1; ++r)   // Row loop
          {
            this.target = Convert.ToDouble(tempgrid.Rows[r].Cells[this.targetDex].Value);
            this.coolantTemp = Convert.ToDouble(tempgrid.Rows[r + 1].Cells[this.coolantTempDex].Value);
            if (this.target == 14.7 && this.coolantTemp > 176)
            {
              this.time = Convert.ToInt32(tempgrid.Rows[r].Cells[this.timeDex].Value);
              this.nexttime = Convert.ToInt32(tempgrid.Rows[r + 1].Cells[this.timeDex].Value);
              this.maf1v = Convert.ToDouble(tempgrid.Rows[r].Cells[this.mafB1Dex].Value);
              this.afr1 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.afrB1Dex].Value);
              this.afr2 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.afrB2Dex].Value);
              this.shorttrim1 = Convert.ToInt32(tempgrid.Rows[r].Cells[this.stB1Dex].Value);
              this.shorttrim2 = Convert.ToInt32(tempgrid.Rows[r].Cells[this.stB1Dex].Value);
              this.accelChange = Convert.ToDouble(((this.nextaccel - this.accel) / (this.nexttime - this.time)) * 1000);

              this.indexFinder1 = this.mafVolts.BinarySearch(this.maf1v);
              if (this.indexFinder1 < 0)
              {
                this.indexFinder1 = ~this.indexFinder1;
              }

              if (this.dualTB)
              {
                this.maf2v = Convert.ToDouble(tempgrid.Rows[r].Cells[this.mafB2Dex].Value);
                this.indexFinder2 = this.mafVolts.BinarySearch(this.maf2v);
                if (this.indexFinder2 < 0)
                {
                  this.indexFinder2 = ~this.indexFinder2;
                }
              }

              if (this.ltB1Dex != -1 && this.dualTB) // Dual throttle bodies and have logged long term trim
              {
                this.longtrim1 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.ltB1Dex].Value);
                this.longtrim2 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.ltB2Dex].Value);
                this.finaltrim1 = (this.shorttrim1 + Convert.ToInt32(this.longtrim1)) / 2;
                this.finaltrim2 = (this.shorttrim2 + Convert.ToInt32(this.longtrim2)) / 2;
              }
              else if (this.ltB1Dex == -1 && this.dualTB) // Dual throttle bodies and have NOT logged long term trim
              {
                this.finaltrim1 = this.shorttrim1;
                this.finaltrim2 = this.shorttrim2;
              }
              else if (this.ltB1Dex != -1 && !this.dualTB) // Single throttle body and have logged long term trim
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

              if (this.clStatus == "CL_ACCEL" || this.clStatus == "CL_Full")
              {
                this.accel = Convert.ToDouble(tempgrid.Rows[r].Cells[this.accelDex].Value);
                this.nextaccel = Convert.ToDouble(tempgrid.Rows[r + 1].Cells[this.accelDex].Value);
              }

              if (this.clStatus == "CL_IAT" || this.clStatus == "CL_Full")
              {
                this.intakeAirTemp = Convert.ToInt32(tempgrid.Rows[r].Cells[this.intakeAirTempDex].Value);
              }

              if (this.clStatus == "CL_Basic")
              {
                this.ClosedLoop_Start();
              }
              else if (this.clStatus == "CL_IAT"
                   && this.intakeAirTemp >= this.intakeAirTempAVG - 10
                   && this.intakeAirTemp <= this.intakeAirTempAVG + 10
                   && this.indexFinder1 < this.mafVolts.Count)
              {
                this.ClosedLoop_Start();
              }
              else if (this.clStatus == "CL_ACCEL"
                   && this.accelChange > -0.1
                   && this.accelChange < 0.1
                   && this.indexFinder1 < this.mafVolts.Count)
              {
                this.ClosedLoop_Start();
              }
              else if (this.clStatus == "CL_Full"
                   && this.intakeAirTemp >= this.intakeAirTempAVG - 10
                   && this.intakeAirTemp <= this.intakeAirTempAVG + 10
                   && this.accelChange > -0.1
                   && this.accelChange < 0.1
                   && this.indexFinder1 < this.mafVolts.Count)
              {
                this.ClosedLoop_Start();
              }
            }
          }

          this.ClosedLoop_Finish();

          // OpenLoop_Finish();

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
            dr[1] = (double)this.maf1ClosedLoop[i];
            dr[2] = (double)this.maf2ClosedLoop[i];
            dr[3] = (int)this.hits1[i];
            dr[4] = (int)this.hits2[i];
            dt.Rows.Add(dr);
          }
        }
        else
        {
          StringBuilder sb = new StringBuilder();
          sb.Append("Could not find the following headers: \n");
          if (this.targetDex == -1)
          {
            sb.Append("Target AFR\n");
          }

          if (this.mafB1Dex == -1)
          {
            sb.Append("MAS A/F -B2 (V)\n");
          }

          if (this.afrB1Dex != -1)
          {
            sb.Append("AFR WB-B1 / LC-1 (1) AFR\n");
          }

          if (this.afrB2Dex != -1)
          {
            sb.Append("AFR WB-B2 / LC-1 (2) AFR\n");
          }

          if (this.coolantTempDex != -1)
          {
            sb.Append("Coolant Temp\n");
          }

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
          this.hits1[c] = tmpList.Count;
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
          this.hits2[c] = tmpList.Count;
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
          this.hits1[c] = tmpList.Count;
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
      if (this.dualTB)
      {
        // Build first line for 2nd adjustment DataTable
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

        // MAF 1 - write values to datatable
        for (int i = 0; ; )
        {
          double cell1 = Convert.ToDouble(this.olDT1.Rows[i][this.indexFinder1]);

          if (this.actualAFR1 != 0)
          {
            this.tmpAdjustment1 = (this.actualAFR1 / this.target) * 100;
          }

          if (i == this.olDT1.Rows.Count - 1) // Add extra row if close to the end
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

          /*   might use afr from few rows down to get more accurate reading
             if (r > 2 && r < CL_DT1.Rows.Count - 2 && afr1 < 14.7)
             {
               actualAFR1 = Convert.ToDouble(tempgrid.Rows[r + 3].Cells[afrB1Dex].Value);
               if (dualTB && afr2 < 14.7)
                 actualAFR2 = Convert.ToDouble(tempgrid.Rows[r + 3].Cells[afrB2Dex].Value);
               else
                 actualAFR2 = 0;
             }
             else
             {
               actualAFR1 = 0;
             }
          */

          if (cell1 == 1.1 && this.actualAFR1 != 0 && this.indexFinder1 >= 0 && this.indexFinder1 < this.mafVolts.Count)
          {
            this.olDT1.Rows[i][this.indexFinder1] = this.tmpAdjustment1;
            break;
          }
          else
          {
            ++i;
            continue;
          }
        }

        // MAF 2 - write values to datatable
        if (this.actualAFR2 != 0)
        {
          this.tmpAdjustment2 = (this.actualAFR2 / this.target) * 100;
        }

        for (int i = 0; ; )
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
      else
      {
        // Single MAF - Write values to DataTable
        for (int i = 0; ;)
        {
          double cell1 = Convert.ToDouble(this.olDT1.Rows[i][this.indexFinder1]);

          if (this.actualAFR1 !=0 && this.actualAFR2 != 0)
          {
            this.tmpAdjustment1 = (this.actualAFR1 / this.target) * 100;
            this.tmpAdjustment2 = (this.actualAFR1 / this.target) * 100;
          }

          // Add extra row if close to the end
          if (i == this.olDT1.Rows.Count - 1)
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

          if (cell1 == 1.1 && this.actualAFR1 != 0 && this.actualAFR2 != 0 && this.indexFinder1 >= 0 && this.indexFinder1 < this.mafVolts.Count)
          {
            this.olDT1.Rows[i][this.indexFinder1] = (this.tmpAdjustment1 + this.tmpAdjustment1) / 2;
            break;
          }
          else
          {
            ++i;
          }
        }
      }
    }

    private void OpenLoop_Finish()
    {
      // DUAL THROTTLE BODIES
      if (this.dualTB)
      {
        // Build first line for 2nd adjustment DataTable
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

        // MAF1 - Read values from datatable
        for (int c = 0; c < this.olDT1.Columns.Count - 1; ++c)
        {
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
          if (tmpList.Count > 5)
          {
            this.maf1OpenLoop[c] = (double)tmpList.Average();
          }
          else
          {
            this.maf1OpenLoop[c] = 100;
          }
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
      else
      {
        // Single MAF - Write values to DataTable
        for (int c = 0; c < this.olDT1.Columns.Count - 1; ++c)
        {
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

          if (tmpList.Count > 5)
          {
            this.maf1OpenLoop[c] = (double)tmpList.Average();
          }
          else
          {
            this.maf1OpenLoop[c] = 100;
          }
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
      List<int> tmpRPMlist = BuffDV_FuelComp.FC_RPM;
      List<int> tmpXlist = BuffDV_FuelComp.FC_XdataByte;
      DataTable DT_FC_hits = new DataTable();
      DataTable DT_FC_totals = new DataTable();
      DataTable DT_FC = new DataTable();

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
        int fuelXtrace;
        double longtrim1;
        double longtrim2;
        int rpm;
        int shorttrim1;
        int shorttrim2 = 100;
        double target;
        int trim;
        double tmpAdjustment1;
        int indexFinderDB;
        int indexFinderRPM;
        foreach (int i in tmpXlist)
        {
          DT_FC_hits.Columns.Add(Convert.ToString(i), typeof(int));
          DT_FC_totals.Columns.Add(Convert.ToString(i), typeof(double));
          DT_FC.Columns.Add(Convert.ToString(i), typeof(decimal));
        }

        foreach (int i in tmpRPMlist)
        {
          DT_FC_hits.Rows.Add();
          DT_FC_totals.Rows.Add();
          DT_FC.Rows.Add();
        }

        for (int row = 1; row < tempgrid.Rows.Count; ++row)
        {

          target = Convert.ToDouble(tempgrid.Rows[row].Cells[this.targetDex].Value);
          fuelXtrace = Convert.ToInt32(tempgrid.Rows[row].Cells[this.fuelCompTraceDex].Value);
          rpm = Convert.ToInt32(tempgrid.Rows[row].Cells[this.rpmDex].Value);
          afr1 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.afrB1Dex].Value);
          afr2 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.afrB2Dex].Value);

          if (target > 15)
          {
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

          if (target == 14.7 && this.stB1Dex != -1 && this.stB2Dex != -1 && afr1 < 25 && afr2 < 25)
          {
            shorttrim1 = Convert.ToInt32(tempgrid.Rows[row].Cells[this.stB1Dex].Value);
            shorttrim2 = Convert.ToInt32(tempgrid.Rows[row].Cells[this.stB2Dex].Value);

            // Dual throttle bodies and have logged long term trim
            if (this.ltB1Dex != -1 && this.dualTB)
            {
              longtrim1 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.ltB1Dex].Value);
              longtrim2 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.ltB2Dex].Value);
              finaltrim1 = (shorttrim1 + Convert.ToInt32(longtrim1)) / 2;
              finaltrim2 = (shorttrim2 + Convert.ToInt32(longtrim2)) / 2;
            }

            // Dual throttle bodies and have NOT logged long term trim
            else if (this.ltB1Dex == -1 && this.dualTB)
            {
              finaltrim1 = shorttrim1;
              finaltrim2 = shorttrim2;
            }

            // Single throttle body and have logged long term trim
            else if (this.ltB1Dex != -1 && !this.dualTB)
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
              finaltrim2 = 100;
            }

            trim = (finaltrim1 + finaltrim2) / 2;

            if (DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] == null
              || string.IsNullOrEmpty(DT_FC_totals.Rows[indexFinderRPM][indexFinderDB].ToString()))
            {
              DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] = trim;
            }
            else
            {
              DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] = Convert.ToDecimal(DT_FC_totals.Rows[indexFinderRPM][indexFinderDB]) + trim;
            }

            if (DT_FC_hits.Rows[indexFinderRPM][indexFinderDB] == null
              || string.IsNullOrEmpty(DT_FC_hits.Rows[indexFinderRPM][indexFinderDB].ToString()))
            {
              DT_FC_hits.Rows[indexFinderRPM][indexFinderDB] = 1;
            }
            else
            {
              DT_FC_hits.Rows[indexFinderRPM][indexFinderDB] = Convert.ToInt32(DT_FC_hits.Rows[indexFinderRPM][indexFinderDB]) + 1;
            }
          }
          else if (target < 14.7 && afr1 < 25 && afr2 < 25)
          {
            afr1 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.afrB1Dex].Value);
            afr2 = Convert.ToDouble(tempgrid.Rows[row].Cells[this.afrB2Dex].Value);
            fuelXtrace = Convert.ToInt32(tempgrid.Rows[row].Cells[this.fuelCompTraceDex].Value);
            rpm = Convert.ToInt32(tempgrid.Rows[row].Cells[this.rpmDex].Value);

            this.actualAFR1 = Convert.ToDouble(tempgrid.Rows[row + 2].Cells[this.afrB1Dex].Value);
            if (this.dualTB && afr2 < 14.7)
            {
              this.actualAFR2 = Convert.ToDouble(tempgrid.Rows[row + 2].Cells[this.afrB2Dex].Value);
            }
            else
            {
              this.actualAFR2 = 0;
            }

            if (this.actualAFR1 != 0 && this.actualAFR2 != 0)
            {
              tmpAdjustment1 = ((this.actualAFR1 + this.actualAFR2) / target) * 100;
            }
            else
            {
              tmpAdjustment1 = (this.actualAFR1 / target) * 100;
            }

            if (DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] == null
            || string.IsNullOrEmpty(DT_FC_totals.Rows[indexFinderRPM][indexFinderDB].ToString()))
            {
              DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] = tmpAdjustment1;
            }
            else
            {
              DT_FC_totals.Rows[indexFinderRPM][indexFinderDB] = Convert.ToDouble(DT_FC_totals.Rows[indexFinderRPM][indexFinderDB]) + tmpAdjustment1;
            }

            if (DT_FC_hits.Rows[indexFinderRPM][indexFinderDB] == null
            || string.IsNullOrEmpty(DT_FC_hits.Rows[indexFinderRPM][indexFinderDB].ToString()))
            {
              DT_FC_hits.Rows[indexFinderRPM][indexFinderDB] = 1;
            }
            else
            {
              DT_FC_hits.Rows[indexFinderRPM][indexFinderDB] = Convert.ToInt32(DT_FC_hits.Rows[indexFinderRPM][indexFinderDB]) + 1;
            }
          }
        }

        for (int row = 0; row < DT_FC_totals.Rows.Count; ++row)
        {
          for (int col = 0; col < DT_FC_totals.Columns.Count; ++col)
          {
            if (!string.IsNullOrEmpty(DT_FC_totals.Rows[row][col].ToString())
            || !string.IsNullOrEmpty(DT_FC_hits.Rows[row][col].ToString()))
            {
              double total = Convert.ToDouble(DT_FC_totals.Rows[row][col]);
              int hit = Convert.ToInt32(DT_FC_hits.Rows[row][col]);
              decimal final = Convert.ToDecimal(Convert.ToDouble(DT_FC_totals.Rows[row][col]) / Convert.ToInt32(DT_FC_hits.Rows[row][col]));
              DT_FC.Rows[row][col] = Convert.ToDecimal(Convert.ToDouble(DT_FC_totals.Rows[row][col]) / Convert.ToInt32(DT_FC_hits.Rows[row][col]));
            }
          }
        }
      }

      return DT_FC;
    }
  }
}
