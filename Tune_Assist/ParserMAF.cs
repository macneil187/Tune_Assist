namespace AutoTune
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Linq;
  using System.Text;
  using System.Windows.Forms;

  public class ParserMAF
  {
    private static List<double> mafVolts = new List<double>
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
    private IndexFinder indexer = new IndexFinder();
    private double accel;
    private double accelChange;
    private double actualAFR1;
    private double actualAFR2;
    private double afr1;
    private double afr2;
    private double coolantTemp;
    private double finaltrim1;
    private double finaltrim2;
    private double olTrim1;
    private double olTrim2;
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
    private int totalLines = 0;
    private string clStatus = string.Empty;
    private string olStatus = string.Empty;
    private bool dualTB;
    private bool accelAfterDecel;

    public List<double> MafVolts
    {
      get
      {
        return mafVolts;
      }
    }

    public DataTable AdjustMAF(BackgroundWorker bw, DataGridView tempgrid)
    {
      using (DataTable dt = new DataTable())
      {
        // Init the adjustment lists and add voltage columns
        foreach (double d in mafVolts)
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

        this.indexer.FindHeader_Indexes(tempgrid);
        if (Properties.Settings.Default.MAF_IAT && this.indexer.IntakeAirTempDex != -1)
        {
          this.FindIAT_average(tempgrid);
        }

        // Build first line for the adjustment CL DataTable
        if (this.clDT1.Rows.Count == 0)
        {
          DataRow dr = this.clDT1.NewRow();
          int c = 0;
          foreach (double d in mafVolts)
          {
            dr[c] = 1.1;
            ++c;
          }

          this.clDT1.Rows.Add(dr);
        }

        if (this.indexer.MafB2Dex != -1)
        {
          this.dualTB = true;

          // Build first line for the adjustment CL DataTable
          if (this.clDT2.Rows.Count == 0)
          {
            DataRow dr = this.clDT2.NewRow();
            int c = 0;
            foreach (double d in mafVolts)
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
          foreach (double d in mafVolts)
          {
            dr[c] = 1.1;
            ++c;
          }

          this.olDT1.Rows.Add(dr);
        }

        if (this.indexer.MafB2Dex != -1)
        {
          this.dualTB = true;

          // Build first line for the adjustment OL DataTable
          if (this.olDT2.Rows.Count == 0)
          {
            DataRow dr = this.olDT2.NewRow();
            int c = 0;
            foreach (double d in mafVolts)
            {
              dr[c] = 1.1;
              ++c;
            }

            this.olDT2.Rows.Add(dr);
          }
        }

        if (this.indexer.TargetDex != -1 && this.indexer.MafB1Dex != -1 && this.indexer.AfrB1Dex != -1 && this.indexer.AfrB2Dex != -1 && this.indexer.CoolantTempDex != -1)
        {
          for (int r = 0; r < tempgrid.Rows.Count - 1; ++r)
          {
            // dual MAF check
            if (this.indexer.MafB2Dex != -1)
            {
              this.dualTB = true;
            }

            try
            {
              this.target = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.TargetDex].Value);
              this.maf1v = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.MafB1Dex].Value);
              this.afr1 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.AfrB1Dex].Value);
              this.afr2 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.AfrB2Dex].Value);
            }
            catch
            {
              Console.WriteLine("Error while setting parameter values for row {0}", r);
              continue;
            }

            if (this.indexer.TimeDex != -1 && this.indexer.StB1Dex != -1 && this.indexer.StB2Dex != -1)
            {
              try
              {
                this.time = Convert.ToInt32(tempgrid.Rows[r].Cells[this.indexer.TimeDex].Value);
                this.nexttime = Convert.ToInt32(tempgrid.Rows[r + 1].Cells[this.indexer.TimeDex].Value);
                this.accel = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.AccelDex].Value);
                this.nextaccel = Convert.ToDouble(tempgrid.Rows[r + 1].Cells[this.indexer.AccelDex].Value);
                this.shorttrim1 = Convert.ToInt32(tempgrid.Rows[r].Cells[this.indexer.StB1Dex].Value);
                this.shorttrim2 = Convert.ToInt32(tempgrid.Rows[r].Cells[this.indexer.StB1Dex].Value);
                this.accelChange = Convert.ToDouble(((this.nextaccel - this.accel) / (this.nexttime - this.time)) * 1000);
                this.coolantTemp = Convert.ToDouble(tempgrid.Rows[r + 1].Cells[this.indexer.CoolantTempDex].Value);
                this.intakeAirTemp = Convert.ToInt32(tempgrid.Rows[r].Cells[this.indexer.IntakeAirTempDex].Value);

                if (this.dualTB)
                {
                  this.maf2v = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.MafB2Dex].Value);
                }
              }
              catch
              {
                Console.WriteLine(" error while setting parameter values for row {0}", r);
                continue;
              }

              // Back on accel after decel  ** This will skip down rows to avoid skewing values
              if (this.afr1 == 60 || this.target == 30)
              {
                this.accelAfterDecel = true;
                continue;
              }
              else if (this.afr1 < 20 && this.target < 15 && this.accelAfterDecel)
              {
                r += 9;
                this.accelAfterDecel = false;
                continue;
              }

              // Closed loop
              if (this.target == 14.7 && this.coolantTemp > 176)
              {
                // Dual throttle bodies and have logged long term trim
                if (this.indexer.LtB1Dex != -1 && this.indexer.LtB2Dex != -1 && this.dualTB)
                {
                  this.longtrim1 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.LtB1Dex].Value);
                  this.longtrim2 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.LtB2Dex].Value);
                  this.finaltrim1 = (this.shorttrim1 + Convert.ToInt32(this.longtrim1)) / 2;
                  this.finaltrim2 = (this.shorttrim2 + Convert.ToInt32(this.longtrim2)) / 2;
                }

                // Dual throttle bodies and have NOT logged long term trim
                else if (this.dualTB && (this.indexer.LtB1Dex == -1 || this.indexer.LtB2Dex == -1))
                {
                  this.finaltrim1 = this.shorttrim1;
                  this.finaltrim2 = this.shorttrim2;
                }

                // Single throttle body and have logged long term trim
                else if (!this.dualTB && (this.indexer.LtB1Dex != -1 || this.indexer.LtB2Dex != -1 ))
                {
                  this.longtrim1 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.LtB1Dex].Value);
                  this.longtrim2 = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.LtB2Dex].Value);
                  this.finaltrim1 = (double) (this.shorttrim1 + Convert.ToInt32(this.longtrim1)) / 2;
                  this.finaltrim2 = (double) (this.shorttrim2 + Convert.ToInt32(this.longtrim2)) / 2;
                  this.finaltrim1 = (double) (this.finaltrim1 + this.finaltrim2) / 2;
                }
                else
                {
                  this.finaltrim1 = (this.shorttrim1 + this.shorttrim2) / 2;
                  this.finaltrim2 = 100;
                }

                this.indexFinder1 = mafVolts.BinarySearch(this.maf1v);
                if (this.indexFinder1 < 0)
                {
                  this.indexFinder1 = ~this.indexFinder1;
                }

                if (this.dualTB)
                {
                  this.indexFinder2 = mafVolts.BinarySearch(this.maf2v);
                  if (this.indexFinder2 < 0)
                  {
                    this.indexFinder2 = ~this.indexFinder2;
                  }
                }

                // CLOSED LOOP
                //  filter out intake air temp changes  &&  filter out quick accel pedal position changes
                if (Properties.Settings.Default.MAF_IAT && this.indexer.IntakeAirTempDex != -1
                        && (this.intakeAirTemp >= this.intakeAirTempAVG - 10 && this.intakeAirTemp <= this.intakeAirTempAVG + 10)
                        && Properties.Settings.Default.MAF_ACCEL && this.accelChange > -0.1 && this.accelChange < 0.1
                        && this.indexFinder1 >= 0 && this.indexFinder1 < mafVolts.Count)
                {
                  this.ClosedLoop_Start();
                }

                // Filter out quick accel pedal position changes
                else if ((!Properties.Settings.Default.MAF_IAT || this.indexer.IntakeAirTempDex == -1)
                        && Properties.Settings.Default.MAF_ACCEL && this.accelChange > -0.1 && this.accelChange < 0.1
                        && this.indexFinder1 >= 0 && this.indexFinder1 < mafVolts.Count)
                {
                  this.ClosedLoop_Start();
                }

                // Filter out intake air temp changes
                else if (Properties.Settings.Default.MAF_IAT && this.indexer.IntakeAirTempDex != -1
                        && (this.intakeAirTemp >= this.intakeAirTempAVG - 10 && this.intakeAirTemp <= this.intakeAirTempAVG + 10)
                        && !Properties.Settings.Default.MAF_ACCEL && this.indexFinder1 >= 0 && this.indexFinder1 < mafVolts.Count)
                {
                  this.ClosedLoop_Start();
                }
              }

              // Open loop
              else if (this.target < 14.7 && this.shorttrim1 == 100 && this.afr1 < 20 && this.coolantTemp > 176 && Properties.Settings.Default.MAF_OL)
              {
                this.olTrim1 = this.afr1 / this.target;
                this.olTrim2 = this.afr2 / this.target;

                this.indexFinder1 = mafVolts.BinarySearch(this.maf1v);
                if (this.indexFinder1 < 0)
                {
                  this.indexFinder1 = ~this.indexFinder1;
                }

                if (this.dualTB)
                {
                  this.indexFinder2 = mafVolts.BinarySearch(this.maf2v);
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
                    this.actualAFR1 = Convert.ToDouble(tempgrid.Rows[r + 2].Cells[this.indexer.AfrB1Dex].Value);
                    if (this.dualTB)
                    {
                      this.actualAFR2 = Convert.ToDouble(tempgrid.Rows[r + 2].Cells[this.indexer.AfrB2Dex].Value);
                    }
                  }
                  catch
                  {
                    Console.WriteLine(" error while actualAFR values for row {0}", r);
                    continue;
                  }
                }

                Console.Write(this.accelChange + "\n");
                // Open Loop Starter
                //  filter out intake air temp changes  &&  filter out quick accel pedal position changes
                if (Properties.Settings.Default.MAF_IAT && this.indexer.IntakeAirTempDex != -1
                        && (this.intakeAirTemp >= this.intakeAirTempAVG - 10 && this.intakeAirTemp <= this.intakeAirTempAVG + 10)
                        && Properties.Settings.Default.MAF_ACCEL && this.accelChange > -0.1 && this.accelChange < 0.1
                        && this.indexFinder1 >= 0 && this.indexFinder1 < mafVolts.Count)
                {
                  this.OpenLoop_Start();
                }

                // Filter out quick accel pedal position changes
                else if ((!Properties.Settings.Default.MAF_IAT || this.indexer.IntakeAirTempDex == -1)
                        && Properties.Settings.Default.MAF_ACCEL && this.accelChange > -0.1 && this.accelChange < 0.1
                        && this.indexFinder1 >= 0 && this.indexFinder1 < mafVolts.Count)
                {
                  this.OpenLoop_Start();
                }

                // Filter out intake air temp changes
                else if (Properties.Settings.Default.MAF_IAT && this.indexer.IntakeAirTempDex != -1
                        && (this.intakeAirTemp >= this.intakeAirTempAVG - 10 && this.intakeAirTemp <= this.intakeAirTempAVG + 10)
                        && !Properties.Settings.Default.MAF_ACCEL && this.indexFinder1 >= 0 && this.indexFinder1 < mafVolts.Count)
                {
                  this.OpenLoop_Start();
                }
              }
            }
            else
            {
              StringBuilder sb = new StringBuilder();
              sb.Append("Could not find the following headers: \n");
              if (this.indexer.TimeDex == -1) {sb.Append("Time\n"); }
              if (this.indexer.StB1Dex == -1) {sb.Append("A/F CORR-B1 (%)\n"); }
              if (this.indexer.StB2Dex == -1) {sb.Append("A/F CORR-B2 (%)\n"); }
              if (this.indexer.AccelDex == -1) {sb.Append("ACCEL PED POS 1\n"); }
              if (this.indexer.LtB1Dex == -1) {sb.Append("LT Fuel Trim B1 (%)\n"); }
              if (this.indexer.LtB2Dex == -1) {sb.Append("LT Fuel Trim B2 (%)\n"); }
              if (this.indexer.AfrB1Dex == -1) {sb.Append("AFR WB-B1\n"); }
              if (this.indexer.AfrB2Dex == -1) {sb.Append("AFR WB-B2\n"); }
              if (this.indexer.MafB1Dex == -1) {sb.Append("MAS A/F -B1 (V)\n"); }
              if (this.indexer.MafB2Dex == -1) {sb.Append("MAS A/F -B2 (V)\n"); }
              if (this.indexer.TargetDex == -1) {sb.Append("TARGET AFR\n"); }
              if (this.indexer.IntakeAirTempDex == -1) {sb.Append("INTAKE AIR TMP\n"); }
              if (this.indexer.CoolantTempDex == -1) {sb.Append("COOLANT TEMP\n"); }

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
          for (int i = 0; i < mafVolts.Count; ++i)
          {
            DataRow dr = dt.NewRow();
            double CL1 = this.maf1ClosedLoop[i];
            double CL2 = this.maf2ClosedLoop[i];
            double OL1 = this.maf1OpenLoop[i];
            double OL2 = this.maf2OpenLoop[i];
            double final1 = 0;
            double final2 = 0;
            double diff = 0;
            double diff1 = 0;
            double diff2 = 0;
            
            // Minimal changes to the maf
            if (Properties.Settings.Default.Maf_MINIMAL)
            {
              // Console.WriteLine("CL1: {0}, CL2: {1}, OL1: {2}, OL2: {3}", CL1, CL2, OL1, OL2);
              if (CL1 == 100 && OL1 != 100)
              {
                final1 = OL1;
                //  Console.WriteLine(" Value found at OL1, but not at CL1. final1 is now {0}", final1);
              }
              else if (CL1 != 100 && OL1 != 100)
              {
                //  Console.WriteLine(" Value found at OL1 & CL1. ");
                final1 = (CL1 + OL1) / 2;
                // Console.WriteLine("(CL1 + OL1) = final1 |  {1} + {2} = final1", CL1, OL1, final1);
              }
              else
              {
                final1 = CL1;
              }

              if (CL2 == 100 && OL2 != 100)
              {
                final2 = OL2;
                //  Console.WriteLine(" Value found at OL2, but not at CL2. final2 is now {0}", final2);
              }
              else if (CL2 != 100 && OL2 != 100)
              {
                // Console.WriteLine(" Value found at CL2 & OL2. ");
                final2 = (CL2 + OL2) / 2;
                 // Console.WriteLine("(CL2 + OL2) = final2 |  {1} + {2} = final2", CL2, OL2, final2);
              }
              else
              {
                final2 = CL2;
              }

              // Finding difference between finals
              diff = final1 - final2;
              //  Console.WriteLine("diff = final1 - final2", diff, final1, final2);

              if (diff < 0)
              {
                diff = -diff;
               // Console.WriteLine("diff was negitive, we have changed it to {0}", diff);
              }


              if (final1 < 100 && final2 < 100)
              {
               // Console.WriteLine("final1 & final2 are both UNDER 100.");
                if (final1 < final2)
                {
                 // Console.WriteLine("final1: {0}  was found to be LESS than final2: {1} && both UNDER 100 >> attempting to balance final1 <<", final1, final2);
                  final1 = final2 = 100;
                  final1 -= diff;
                 // Console.WriteLine("final1 is now {0}  |  Final2 is now {1}", final1, final2);
                }
                else if (final1 > final2)
                {
                //  Console.WriteLine("final1: {0}  was found to be MORE than final2: {1} && both UNDER 100 >> attempting to balance final2 <<", final1, final2);
                  final1 = final2 = 100;
                  final2 -= diff;
                 // Console.WriteLine("final1 is now {0}  |  Final2 is now {1}", final1, final2);
                }
              }
              else if (final1 > 100 && final2 > 100)
              {
                //Console.WriteLine("final1 & final2 are both OVER 100.");
                if (final1 < final2)
                {
                 // Console.WriteLine("final1: {0}  was found to be LESS than final2: {1} && both OVER 100 >> attempting to balance final1 <<", final1, final2);
                  final1 = final2 = 100;
                  final2 += diff;
                 // Console.WriteLine("final1 is now {0}  |  Final2 is now {1}", final1, final2);
                }
                else if (final1 > final2)
                {
                  //Console.WriteLine("final1: {0}  was found to be MORE than final2: {1} && both OVER 100 >> attempting to balance final1 <<", final1, final2);
                  final1 = final2 = 100;
                  final1 += diff;
                 // Console.WriteLine("final1 is now {0}  |  Final2 is now {1}", final1, final2);
                }
              }
              else
              {
                //Console.WriteLine("final1 & final2 are NOT same direction.");

                diff = final1 - final2;
                diff1 = final1 - 100;
                diff2 = final2 - 100;
                if (diff1 < 0)
                {
                  diff1 = -diff;
                }

                if (diff2 < 0)
                {
                  diff2 = -diff;
                }

                diff1 /= 4;
                diff2 /= 4;
                if (final1 < 100 && (final1 + diff1) < 100)
                {
                  final1 += diff1;
                }
                else if (final1 > 100 && (final1 - diff1) > 100)
                {
                  final1 -= diff1;
                }
                else
                { 
                  final1 = 100;
                }

                if (final2 < 100 && (final2 + diff2) < 100)
                {
                  final2 += diff2;
                }
                else if (final2 > 100 && (final2 - diff2) > 100)
                {
                  final2 -= diff2;
                }
                else
                {
                  final2 = 100;
                }
              }
            }
            else
            {
              if (CL1 == 100 && OL1 != 100)
              {
                final1 = (double)OL1;
              }
              else if (CL1 != 100 && OL1 != 100)
              {
                final1 = (double)(CL1 + OL1) / 2;
              }
              else
              {
                final1 = 100;
              }

              if (CL2 == 100 && OL2 != 100)
              {
                final2 = (double)OL2;
              }
              else if (CL2 != 100 && OL2 != 100)
              {
                final2 = (double)(CL2 + OL2) / 2;
              }
              else
              {
                final2 = 100;
              }
            }

            dr[0] = (double)mafVolts[i];
            dr[1] = final1;
            dr[2] = final2;
            dr[3] = (int)this.hits1[i];
            dr[4] = (int)this.hits2[i];
            dt.Rows.Add(dr);
          }
        }
        else
        {
          StringBuilder sb = new StringBuilder();
          sb.Append("Could not find the following headers: \n");
          if (this.indexer.TimeDex == -1) { sb.Append("Time\n"); }
          if (this.indexer.StB1Dex == -1) { sb.Append("A/F CORR-B1 (%)\n"); }
          if (this.indexer.StB2Dex == -1) { sb.Append("A/F CORR-B2 (%)\n"); }
          if (this.indexer.AccelDex == -1) { sb.Append("ACCEL PED POS 1\n"); }
          if (this.indexer.LtB1Dex == -1) { sb.Append("LT Fuel Trim B1 (%)\n"); }
          if (this.indexer.LtB2Dex == -1) { sb.Append("LT Fuel Trim B2 (%)\n"); }
          if (this.indexer.AfrB1Dex == -1) { sb.Append("AFR WB-B1\n"); }
          if (this.indexer.AfrB2Dex == -1) { sb.Append("AFR WB-B2\n"); }
          if (this.indexer.MafB1Dex == -1) { sb.Append("MAS A/F -B1 (V)\n"); }
          if (this.indexer.MafB2Dex == -1) { sb.Append("MAS A/F -B2 (V)\n"); }
          if (this.indexer.TargetDex == -1) { sb.Append("TARGET AFR\n"); }
          if (this.indexer.IntakeAirTempDex == -1) { sb.Append("INTAKE AIR TMP\n"); }
          if (this.indexer.CoolantTempDex == -1) { sb.Append("COOLANT TEMP\n"); }
          Console.WriteLine(sb.ToString());
          MessageBox.Show("Error", "We could not find minimal parameters needed\nto calculate Closed Loop MAF scaling adjustments.\n" + sb.ToString());
        }

        return dt;
      }
    }

    private void ClosedLoop_Start()
    {
      this.dualTB = this.dualTB && this.indexFinder2 >= 0 && this.indexFinder2 <= mafVolts.Count ? true : false;

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
            foreach (double d in mafVolts)
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
            foreach (double d in mafVolts)
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
            foreach (double d in mafVolts)
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
      this.dualTB = this.dualTB && this.indexer.MafB2Dex != -1 ? true : false;

      // MAF 1 - write values to datatable
      for (int i = 0; ;)
      {
        double cell1 = Convert.ToDouble(this.olDT1.Rows[i][this.indexFinder1]);

        if (this.actualAFR1 != 0)
        {
          this.finaltrim1 = (this.actualAFR1 / this.target) * 100;
        }

        // Add extra row if close to the end
        if (i == this.olDT1.Rows.Count - 1 || this.clDT1.Rows.Count == 0)
        {
          DataRow dr = this.olDT1.NewRow();
          int c = 0;
          foreach (double d in mafVolts)
          {
            dr[c] = 1.1;
            ++c;
          }

          this.olDT1.Rows.Add(dr);
        }

        if (cell1 == 1.1 && this.actualAFR1 != 0 && this.indexFinder1 >= 0 && this.indexFinder1 < mafVolts.Count)
        {
          this.olDT1.Rows[i][this.indexFinder1] = this.finaltrim1;
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
        this.finaltrim2 = (this.actualAFR2 / this.target) * 100;
      }

      for (int i = 0; ;)
      {
        double cell2 = Convert.ToDouble(this.olDT2.Rows[i][this.indexFinder2]);

        // Add extra row if close to the end
        if (i == this.olDT2.Rows.Count - 1)
        {
          DataRow dr = this.olDT2.NewRow();
          int c = 0;
          foreach (double d in mafVolts)
          {
            dr[c] = 1.1;
            ++c;
          }

          this.olDT2.Rows.Add(dr);
        }

        if (cell2 == 1.1 && this.actualAFR2 != 0 && this.indexFinder1 >= 0 && this.indexFinder1 < mafVolts.Count)
        {
          this.olDT2.Rows[i][this.indexFinder2] = this.finaltrim2;
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
      && this.indexer.TargetDex != -1 && this.indexer.MafB1Dex != -1
      && this.indexer.AfrB1Dex != -1 && this.indexer.AfrB2Dex != -1
      && this.indexer.CoolantTempDex != -1)
      {
        this.clStatus = "CL_Basic";
      }
      else if (Properties.Settings.Default.MAF_CL
      && Properties.Settings.Default.MAF_IAT
      && !Properties.Settings.Default.MAF_ACCEL
      && this.indexer.TargetDex != -1 && this.indexer.MafB1Dex != -1
      && this.indexer.AfrB1Dex != -1 && this.indexer.AfrB2Dex != -1
      && this.indexer.CoolantTempDex != -1
      && this.indexer.IntakeAirTempDex != -1)
      {
        this.clStatus = "CL_IAT";
      }
      else if (Properties.Settings.Default.MAF_CL
      && !Properties.Settings.Default.MAF_IAT
      && Properties.Settings.Default.MAF_ACCEL
      && this.indexer.TargetDex != -1 && this.indexer.MafB1Dex != -1
      && this.indexer.AfrB1Dex != -1 && this.indexer.AfrB2Dex != -1
      && this.indexer.CoolantTempDex != -1
      && this.indexer.AccelDex != -1)
      {
        this.clStatus = "CL_ACCEL";
      }
      else if (Properties.Settings.Default.MAF_CL
      && Properties.Settings.Default.MAF_IAT
      && Properties.Settings.Default.MAF_ACCEL
      && this.indexer.TargetDex != -1 && this.indexer.MafB1Dex != -1
      && this.indexer.AfrB1Dex != -1 && this.indexer.AfrB2Dex != -1
      && this.indexer.CoolantTempDex != -1
      && this.indexer.IntakeAirTempDex != -1
      && this.indexer.AccelDex != -1)
      {
        this.clStatus = "CL_Full";
      }
      else
      {
        this.clStatus = "Error";
      }

      if (Properties.Settings.Default.MAF_OL
      && this.indexer.TargetDex != -1 && this.indexer.MafB1Dex != -1
      && this.indexer.AfrB1Dex != -1 && this.indexer.AfrB2Dex != -1
      && this.indexer.CoolantTempDex != -1)
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
        double intakeAirTemp = Convert.ToDouble(tempgrid.Rows[r].Cells[this.indexer.IntakeAirTempDex].Value);
        iatFull.Add(intakeAirTemp);
      }

      this.intakeAirTempAVG = (double)iatFull.Average();
      Console.WriteLine("The average Intake Air Temp is: " + Convert.ToString(this.intakeAirTempAVG));
    }
  }
}
