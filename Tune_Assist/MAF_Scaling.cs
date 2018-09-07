using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTune
{
  public class MAF_Scaling
  {
    public List<double> maf_volts = new List<double>
    {
      0.08, 0.16, 0.23, 0.31, 0.39, 0.47, 0.55, 0.63, 0.70, 0.78, 0.86, 0.94, 1.02, 1.09, 1.17, 1.25, 1.33,
      1.41, 1.48, 1.56, 1.64, 1.72, 1.80, 1.88, 1.95, 2.03, 2.11, 2.19, 2.27, 2.34, 2.42, 2.50, 2.58, 2.66,
      2.73, 2.81, 2.89, 2.97, 3.05, 3.13, 3.20, 3.28, 3.36, 3.44, 3.52, 3.59, 3.67, 3.75, 3.83, 3.91, 3.98,
      4.06, 4.14, 4.22, 4.30, 4.38, 4.45, 4.53, 4.61, 4.69, 4.77, 4.84, 4.92, 5.00
    };

    //List<int> maf1values = AutoTune.autotune.MAFb1UserInput;
    //List<int> maf2values = AutoTune.autotune.MAFb2UserInput;
    List<double> maf1ClosedLoop = new List<double>();
    List<double> maf2ClosedLoop = new List<double>();
    List<double> maf1OpenLoop = new List<double>();
    List<double> maf2OpenLoop = new List<double>();
    DataTable CL_DT1 = new DataTable();
    DataTable CL_DT2 = new DataTable();
    DataTable OL_DT1 = new DataTable();
    DataTable OL_DT2 = new DataTable();
    List<int> hits1 = new List<int>(64);
    List<int> hits2 = new List<int>(64);
    int timeDex; int STb1Dex; int STb2Dex; int accelDex; int LTb1Dex; int LTb2Dex;
    int afrB1Dex; int afrB2Dex; int mafB1Dex; int mafB2Dex; int targetDex;
    private int indexFinder1 = 0;
    private int indexFinder2 = 0;
    private int time;
    private int nexttime;
    private double accel;
    private double nextaccel;
    private double afr1;
    private double upcoming_AFR1 = 0;
    private double afr2;
    private double upcoming_AFR2 = 0;
    private double actualAFR1;
    private double actualAFR2;
    private double tmpAdjustment1;
    private double tmpAdjustment2;
    private int shorttrim1;
    private int shorttrim2 = 100;
    private double target;
    private double accelChange;
    private double maf1v;
    private double maf2v = 0;
    private double longtrim1;
    private double longtrim2;
    private int finaltrim1;
    private int finaltrim2;
    private int totalLines = 0;
    private int readLines = 0;
    private bool dualTB = false;

  public void MAF_Scaler(object sender, DoWorkEventArgs e)
    {
    }

    public DataTable AdjustMAF_CL(BackgroundWorker bw, DataGridView tempgrid)
    {
      using (DataTable dt = new DataTable())
      {
        // init the adjustment lists and add voltage columns
        foreach (double d in maf_volts)
        {
          maf1ClosedLoop.Add(100.00);
          maf2ClosedLoop.Add(100.00);
          //maf1OpenLoop.Add(100.00);
          //maf2OpenLoop.Add(100.00);
          CL_DT1.Columns.Add(Convert.ToString(d));
          CL_DT2.Columns.Add(Convert.ToString(d));
          OL_DT1.Columns.Add(Convert.ToString(d));
          OL_DT2.Columns.Add(Convert.ToString(d));
        }

        if (tempgrid.Rows.Count >= 100)
          totalLines = tempgrid.Rows.Count;

        if (mafB2Dex != -1)   // dual MAF check
         dualTB = true;
        FindHeader_Indexes(tempgrid);

        if (OL_DT1.Rows.Count == 0)  //Build first line for the adjustment DataTable
        {
          DataRow dr = OL_DT1.NewRow();
          int c = 0;
          foreach (double d in maf_volts)
          {
            dr[c] = 1.1;
            ++c;
          }
          OL_DT1.Rows.Add(dr);
        }

        if (dualTB)
        {
          if (CL_DT2.Rows.Count == 0)  //Build first line for the adjustment DataTable
          {
            DataRow dr = CL_DT2.NewRow();
            int c = 0;
            foreach (double d in maf_volts)
            {
              dr[c] = 1.1;
              ++c;
            }
            CL_DT2.Rows.Add(dr);
          }
        }

        if (targetDex != -1 && mafB1Dex != -1 && afrB1Dex != -1 && afrB2Dex != -1)
        {
          for (int r = 0; r < tempgrid.Rows.Count - 1; ++r)   // Row loop
          {
            if (mafB2Dex != -1)   // dual MAF check
              dualTB = true;
            try
            {
              target = Convert.ToDouble(tempgrid.Rows[r].Cells[targetDex].Value);
              maf1v = Convert.ToDouble(tempgrid.Rows[r].Cells[mafB1Dex].Value);
              afr1 = Convert.ToDouble(tempgrid.Rows[r].Cells[afrB1Dex].Value);
              afr2 = Convert.ToDouble(tempgrid.Rows[r].Cells[afrB2Dex].Value);
            }
            catch
            {
              Console.WriteLine(" error while setting parameter values for row {0}", r);
              continue;
            }
            if (timeDex != -1 && STb1Dex != -1 && STb2Dex != -1)
            {
              try
              {
                time = Convert.ToInt32(tempgrid.Rows[r].Cells[timeDex].Value);
                nexttime = Convert.ToInt32(tempgrid.Rows[r + 1].Cells[timeDex].Value);
                accel = Convert.ToDouble(tempgrid.Rows[r].Cells[accelDex].Value);
                nextaccel = Convert.ToDouble(tempgrid.Rows[r + 1].Cells[accelDex].Value);
                shorttrim1 = Convert.ToInt32(tempgrid.Rows[r].Cells[STb1Dex].Value);
                shorttrim2 = Convert.ToInt32(tempgrid.Rows[r].Cells[STb1Dex].Value);
                accelChange = Convert.ToDouble(((nextaccel - accel) / (nexttime - time)) * 1000);
                if (dualTB)
                {
                  maf2v = Convert.ToDouble(tempgrid.Rows[r].Cells[mafB2Dex].Value);
                }
                if (r > 5 && r < CL_DT1.Rows.Count - 5 && afr1 < 14.7)
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
              }
              catch
              {
                Console.WriteLine(" error while setting parameter values for row {0}", r);
                continue;
              }

              if (LTb1Dex != -1 && dualTB) // Dual throttle bodies and have logged long term trim
              {
                longtrim1 = Convert.ToDouble(tempgrid.Rows[r].Cells[LTb1Dex].Value);
                longtrim2 = Convert.ToDouble(tempgrid.Rows[r].Cells[LTb2Dex].Value);
                finaltrim1 = (shorttrim1 + Convert.ToInt32(longtrim1)) / 2;
                finaltrim2 = (shorttrim2 + Convert.ToInt32(longtrim2)) / 2;
              }
              else if (LTb1Dex == -1 && dualTB) // Dual throttle bodies and have NOT logged long term trim
              {
                finaltrim1 = shorttrim1;
                finaltrim2 = shorttrim2;
              }
              else if (LTb1Dex != -1 && !dualTB) // Single throttle body and have logged long term trim
              {
                longtrim1 = Convert.ToDouble(tempgrid.Rows[r].Cells[LTb1Dex].Value);
                longtrim2 = Convert.ToDouble(tempgrid.Rows[r].Cells[LTb2Dex].Value);
                finaltrim1 = (shorttrim1 + Convert.ToInt32(longtrim1)) / 2;
                finaltrim2 = (shorttrim2 + Convert.ToInt32(longtrim2)) / 2;
                finaltrim1 = (finaltrim1 + finaltrim2) / 2;
              }
              else
              {
                finaltrim1 = (shorttrim1 + shorttrim2) / 2;
                finaltrim2 = 100;
              }

              indexFinder1 = maf_volts.BinarySearch(maf1v);
              indexFinder1 = ~indexFinder1;
              if (dualTB)
              {
                indexFinder2 = maf_volts.BinarySearch(maf2v);
                indexFinder2 = ~indexFinder2;
              }
              // CLOSED LOOP
              if (target == 14.7 && accelChange > -0.1 && accelChange < 0.1 && indexFinder1 >= 0 && indexFinder1 < maf_volts.Count)
              {
                ClosedLoop_Start();
              }
            }
            else
            {
              Console.WriteLine("Not enough parameters to calculate close loop maf scaling");
            }

          /*  if (target < 14.7 && afrB1Dex != -1 && afrB2Dex != -1 && mafB1Dex != -1 && indexFinder1 >= 0 && indexFinder1 < maf_volts.Count)
            {
              OpenLoop_Start();
            }  */
          }  //END of looping rows
            //NOW start reading valuses from DT

          ClosedLoop_Finish();
          ///OpenLoop_Finish();
          
          //Build DataTable for returning values
          dt.Columns.Add("Voltage", typeof(double));
          dt.Columns.Add("ClosedLoop_B1", typeof(double));
          dt.Columns.Add("ClosedLoop_B2", typeof(double));
          for (int i = 0; i<maf_volts.Count; ++i)
          {
            DataRow dr = dt.NewRow();
            dr[0] = (double)maf_volts[i];
            dr[1] = (double)maf1ClosedLoop[i];
            dr[2] = (double)maf2ClosedLoop[i];
            dt.Rows.Add(dr);
          }
        }
        else
        {
          MessageBox.Show("Error", "We could not find enough parameters to\ncalculate MAF scaling adjustments.");
        }
        return dt;
      }
    }

    private void ClosedLoop_Start()
    {
      if (dualTB && indexFinder2 >= 0 && indexFinder2 <= maf_volts.Count)
        dualTB = true;
      else
        dualTB = false;

      if (dualTB) //DUAL MAF
      {
        //MAF1
        for (int i = 0; ;)   //find empty spot to insert value in DataTable
        {
          if (i == CL_DT1.Rows.Count - 1 || CL_DT1.Rows.Count == 0)
          {
            DataRow dr = CL_DT1.NewRow();
            int c = 0;
            foreach (double d in maf_volts)
            {
              dr[c] = 1.1;
              ++c;
            }
            CL_DT1.Rows.Add(dr);
          }
          double cell1 = Convert.ToDouble(CL_DT1.Rows[i][indexFinder1]);
          if (finaltrim1 < 75 || finaltrim1 > 125)
            break;
          if (cell1 == 1.1 && indexFinder1 >= 0 && indexFinder1 < maf_volts.Count)
          {
            CL_DT1.Rows[i][indexFinder1] = finaltrim1;
            break;
          }
          else
          {
            ++i;
          }
        }
        //MAF 2
        for (int i = 0; ;)  //Find empty spot to insert value in DataTable 2
        {
          if (i == CL_DT2.Rows.Count - 1 || CL_DT2.Rows.Count == 0)
          {
            DataRow dr = CL_DT2.NewRow();
            int c = 0;
            foreach (double d in maf_volts)
            {
              dr[c] = 1.1;
              ++c;
            }
            CL_DT2.Rows.Add(dr);
          }
          double cell2 = Convert.ToDouble(CL_DT2.Rows[i][indexFinder2]);
          if (finaltrim2 < 75 || finaltrim2 > 125)
            break;
          if (cell2 == 1.1 && indexFinder2 >= 0 && indexFinder2 < maf_volts.Count)
          {
            CL_DT2.Rows[i][indexFinder2] = finaltrim2;
            break;
          }
          else
          {
            ++i;
          }
        }
      }
      else  //Single MAF
      {
        for (int i = 0; ;)   // find empty spot to insert value in DataTable
        {
          double cell1 = Convert.ToDouble(CL_DT1.Rows[i][indexFinder1]);

          if (i == CL_DT1.Rows.Count - 1 || CL_DT1.Rows.Count == 0)
          {
            DataRow dr = CL_DT1.NewRow();
            int c = 0;
            foreach (double d in maf_volts)
            {
              dr[c] = 1.1;
              ++c;
            }
            CL_DT1.Rows.Add(dr);
          }

          if (finaltrim1 < 75 || finaltrim1 > 125)
            break;
          if (cell1 == 1.1 && indexFinder1 >= 0 && indexFinder1 < maf_volts.Count)
          {
            CL_DT1.Rows[i][indexFinder1] = finaltrim1;
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
      if (dualTB)
      {
        //MAF1 
        for (int c = 0; c < CL_DT1.Columns.Count - 1; ++c) // read values from DataTable 
        {
          List<double> tmpList = new List<double>();
          for (int line = 0; line < CL_DT1.Rows.Count - 1; ++line) //each row
          {
            double cell = Convert.ToDouble(CL_DT1.Rows[line][c]);
            if (cell != 1.1)
            {
              tmpList.Add(Convert.ToDouble(CL_DT1.Rows[line][c]));
            }
            else
            {
              break;
            }
          }
          if (tmpList.Count > 10)
          {
            maf1ClosedLoop[c] = (double)tmpList.Average();
          }
          else
          {
            maf1ClosedLoop[c] = 100;
          }
        }
        //MAF2
        for (int c = 0; c < CL_DT2.Columns.Count - 1; ++c) //Read values from DataTable 2
        {
          List<double> tmpList = new List<double>();
          for (int line = 0; line < CL_DT2.Rows.Count - 1; ++line) //each row
          {
            double cell2 = Convert.ToDouble(CL_DT2.Rows[line][c]);
            if (cell2 != 1.1)
            {
              tmpList.Add(Convert.ToDouble(CL_DT2.Rows[line][c]));
            }
            else
            {
              break;
            }
          }
          if (tmpList.Count > 5)
          {
            maf2ClosedLoop[c] = (double)tmpList.Average();
          }
          else
          {
            maf2ClosedLoop[c] = 100;
          }
        }
      }
      else  //Single MAF
      {
        for (int c = 0; c < CL_DT1.Columns.Count - 1; ++c) // read values from DataTable 
        {
          List<double> tmpList = new List<double>();
          for (int line = 0; line < CL_DT1.Rows.Count - 1; ++line) //each row
          {
            double cell = Convert.ToDouble(CL_DT1.Rows[line][c]);
            if (cell != 1.1)
            {
              tmpList.Add(Convert.ToDouble(CL_DT1.Rows[line][c]));
            }
            else
            {
              break;
            }
          }
          if (tmpList.Count > 10)
          {
            maf1ClosedLoop[c] = (double)tmpList.Average();
          }
          else
          {
            maf1ClosedLoop[c] = 100;
          }
        }
      }
    }


    private void OpenLoop_Start()
    {
      if (dualTB && mafB2Dex != -1)
        dualTB = true;
      else
        dualTB = false;
      //DUAL THROTTLE BODIES
      if (dualTB)
      {
        if (OL_DT2.Rows.Count == 0)  //Build first line for 2nd adjustment DataTable
        {
          DataRow dr = OL_DT2.NewRow();
          int c = 0;
          foreach (double d in maf_volts)
          {
            dr[c] = 1.1;
            ++c;
          }
          OL_DT2.Rows.Add(dr);
        }
        //MAF 1 - write values to datatable
        for (int i = 0; ;)
        {
          double cell1 = Convert.ToDouble(OL_DT1.Rows[i][indexFinder1]);

          if (actualAFR1 != 0)
          {
            tmpAdjustment1 = (actualAFR1 / target) * 100;
          }
          
          if (i == OL_DT1.Rows.Count - 1) // Add extra row if close to the end
          {
            DataRow dr = OL_DT1.NewRow();
            int c = 0;
            foreach (double d in maf_volts)
            {
              dr[c] = 1.1;
              ++c;
            }
            OL_DT1.Rows.Add(dr);
          }

          if (cell1 == 1.1 && actualAFR1 != 0 && indexFinder1 >= 0 && indexFinder1 < maf_volts.Count)
          {
            OL_DT1.Rows[i][indexFinder1] = tmpAdjustment1;
            break;
          }
          else
          {
            ++i;
            continue;
          }
        }
        //MAF 2 - write values to datatable
        if (actualAFR2 != 0)
        {
          tmpAdjustment2 = (actualAFR2 / target) * 100;
        }
        
        for (int i = 0; ;)
        {
          double cell2 = Convert.ToDouble(OL_DT2.Rows[i][indexFinder2]);
          if (i == OL_DT2.Rows.Count - 1) // Add extra row if close to the end
          {
            DataRow dr = OL_DT2.NewRow();
            int c = 0;
            foreach (double d in maf_volts)
            {
              dr[c] = 1.1;
              ++c;
            }
            OL_DT2.Rows.Add(dr);
          }

          if (cell2 == 1.1 && actualAFR2 != 0 && indexFinder1 >= 0 && indexFinder1 < maf_volts.Count)
          {
            OL_DT2.Rows[i][indexFinder2] = tmpAdjustment2;
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
        //Single MAF - Write values to DataTable
        for (int i = 0; ;)
        {
          double cell1 = Convert.ToDouble(OL_DT1.Rows[i][indexFinder1]);

          if (actualAFR1 !=0 && actualAFR2 != 0)
          {
            tmpAdjustment1 = (actualAFR1 / target) * 100;
            tmpAdjustment2 = (actualAFR1 / target) * 100;
          }

          if (i == OL_DT1.Rows.Count - 1) // Add extra row if close to the end
          {
            DataRow dr = OL_DT1.NewRow();
            int c = 0;
            foreach (double d in maf_volts)
            {
              dr[c] = 1.1;
              ++c;
            }
            OL_DT1.Rows.Add(dr);
          }

          if (cell1 == 1.1 && actualAFR1 != 0 && actualAFR2 != 0 && indexFinder1 >= 0 && indexFinder1 < maf_volts.Count)
          {
            OL_DT1.Rows[i][indexFinder1] = (tmpAdjustment1 + tmpAdjustment1) / 2;
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
      //DUAL THROTTLE BODIES
      if (dualTB)
      {
        if (OL_DT2.Rows.Count == 0)  //Build first line for 2nd adjustment DataTable
        {
          DataRow dr = OL_DT2.NewRow();
          int c = 0;
          foreach (double d in maf_volts)
          {
            dr[c] = 1.1;
            ++c;
          }
          OL_DT2.Rows.Add(dr);
        }
        //MAF1 - Read values from datatable
        for (int c = 0; c < OL_DT1.Columns.Count - 1; ++c)
        {
          List<double> tmpList = new List<double>();
          for (int line = 0; line < OL_DT1.Rows.Count - 1; ++line) //each row
          {
            double cell = Convert.ToDouble(OL_DT1.Rows[line][c]);
            if (cell != 1.1)
            {
              tmpList.Add(Convert.ToDouble(OL_DT1.Rows[line][c]));
            }
            else
            {
              break;
            }
          }
          if (tmpList.Count > 5)
          {
            maf1OpenLoop[c] = (double)tmpList.Average();
          }
          else
          {
            maf1OpenLoop[c] = 100;
          }
        }
        //MAF2 - Read vales from datatable
        for (int c = 0; c < OL_DT2.Columns.Count - 1; ++c) //Read values from DataTable 2
        {
          List<double> tmpList = new List<double>();
          for (int line = 0; line < OL_DT2.Rows.Count - 1; ++line) //each row
          {
            double cell2 = Convert.ToDouble(OL_DT2.Rows[line][c]);
            if (cell2 != 1.1)
            {
              tmpList.Add(Convert.ToDouble(OL_DT2.Rows[line][c]));
            }
            else
            {
              break;
            }
          }
          if (tmpList.Count > 5)
          {
            maf2OpenLoop[c] = (double)tmpList.Average();
          }
          else
          {
            maf2OpenLoop[c] = 100;
          }
        }
      }
      else
      {
        //Single MAF - Write values to DataTable
        for (int c = 0; c < OL_DT1.Columns.Count - 1; ++c)
        {
          List<double> tmpList = new List<double>();
          for (int line = 0; line < OL_DT1.Rows.Count - 1; ++line) //each row
          {
            double cell = Convert.ToDouble(OL_DT1.Rows[line][c]);
            if (cell != 1.1)
            {
              tmpList.Add(Convert.ToDouble(OL_DT1.Rows[line][c]));
            }
            else
            {
              break;
            }
          }
          if (tmpList.Count > 5)
          {
            maf1OpenLoop[c] = (double)tmpList.Average();
          }
          else
          {
            maf1OpenLoop[c] = 100;
          }
        }
      }
    }

    private void FindHeader_Indexes(DataGridView tempgrid)
    {
      //Time,A/F CORR-B1 (%),A/F CORR-B2 (%),ACCEL PED POS 1 (V-Accel), LT Fuel Trim B1 (%), LT Fuel Trim B2 (%), AFR WB-B1,AFR WB-B2, MAS A/F -B1 (V),MAS A/F -B2 (V),TARGET AFR
      if (tempgrid.Columns.Contains("Time"))
        timeDex = tempgrid.Columns["Time"].Index;
      else
        timeDex = -1;
      if (tempgrid.Columns.Contains("A/F CORR-B1 (%)"))
        STb1Dex = tempgrid.Columns["A/F CORR-B1 (%)"].Index;
      else
        STb1Dex = -1;
      if (tempgrid.Columns.Contains("A/F CORR-B2 (%)"))
        STb2Dex = tempgrid.Columns["A/F CORR-B2 (%)"].Index;
      else
        STb2Dex = -1;
      if (tempgrid.Columns.Contains("ACCEL PED POS 1 (V-Accel)"))
        accelDex = tempgrid.Columns["ACCEL PED POS 1 (V-Accel)"].Index;
      else
        accelDex = -1;
      if (tempgrid.Columns.Contains("LT Fuel Trim B1 (%)"))
        LTb1Dex = tempgrid.Columns["LT Fuel Trim B1 (%)"].Index;
      else
        LTb1Dex = -1;
      if (tempgrid.Columns.Contains("LT Fuel Trim B2 (%)"))
        LTb2Dex = tempgrid.Columns["LT Fuel Trim B2 (%)"].Index;
      else
        LTb2Dex = -1;
      if (tempgrid.Columns.Contains("AFR WB-B1"))
        afrB1Dex = tempgrid.Columns["AFR WB-B1"].Index;
      else
        afrB1Dex = -1;
      if (tempgrid.Columns.Contains("AFR WB-B2"))
        afrB2Dex = tempgrid.Columns["AFR WB-B2"].Index;
      else
        afrB2Dex = -1;
      if (tempgrid.Columns.Contains("MAS A/F -B1 (V)"))
        mafB1Dex = tempgrid.Columns["MAS A/F -B1 (V)"].Index;
      else
        mafB1Dex = -1;
      if (tempgrid.Columns.Contains("MAS A/F -B2 (V)"))
        mafB2Dex = tempgrid.Columns["MAS A/F -B2 (V)"].Index;
      else
        mafB2Dex = -1;
      if (tempgrid.Columns.Contains("TARGET AFR"))
        targetDex = tempgrid.Columns["TARGET AFR"].Index;
      else
        targetDex = -1;
    }
  }
}
