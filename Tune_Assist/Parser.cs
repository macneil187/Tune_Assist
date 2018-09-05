using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTune
{
  public class Parser
  {
    DataGridView AT = AutoTune.autotune.buffDV1;
    List<string> cleanedHeaders = new List<string>();
    private List<string> matchedHeaders = new List<string>();
    
    public void parser(object sender, DoWorkEventArgs e)
    {
    }

    //Parse User Data
    public DataTable ParseLog(BackgroundWorker bw, string fileName)
    {
      int numLines = 0;
      
      using (DataTable DT = new DataTable())
      using (StreamReader sr = new StreamReader(fileName))
      {
        string full = sr.ReadToEnd();
        if (full.EndsWith("\n") || full.EndsWith("\r"))
        {
          full = full.TrimEnd('\n');
          full = full.TrimEnd('\r');
        }
        string[] lines = full.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        if (lines[0].EndsWith(","))
          lines[0].TrimEnd(',');
        if (lines.Length < 10)
          return DT;
        List<string> headers = new List<string>(lines[0].Split(','));
        List<int> skipValues = new List<int>();
        FileInfo fi = new FileInfo(fileName);
        long totalBytes = fi.Length;
        long bytesRead = 0;
        int headerindex = 0;

        foreach (var label in headers)
        {
          if (!matchedHeaders.Contains(label))
          {
            matchedHeaders.Add(label);
          }
          else
          {
            skipValues.Add(headerindex);
          }
          headerindex++;
        }
        //Create columns from headers
        for (int h = 0; h < matchedHeaders.Count; ++h)
        {
          if (matchedHeaders[h] == null)
            break;
          DT.Columns.Add();
          DT.Columns[h].ColumnName = matchedHeaders[h];
          DT.Columns[h].ReadOnly = true;
        }
        for (int x = 1; x < lines.Length - 1; ++x)
        {
          if (lines[x] == null)
          {
            continue;
          }
          else if (lines[x].EndsWith(","))
          {
            lines[x] = lines[x].TrimEnd(',');
          }
          string[] line = lines[x].Split(',');
          List<string> cells = new List<string>();
          //Update ProgressBar
          bytesRead += sr.CurrentEncoding.GetByteCount(lines[x]);
          numLines++;
          int pctComplete = (int)(((double)bytesRead / (double)totalBytes) * 100);
          bw.ReportProgress(pctComplete);

          if (line == null || line.Length > headers.Count || line.Length < matchedHeaders.Count)
            continue;

          for (int i = 0; i < matchedHeaders.Count; ++i)
          {
            if (line[i] == null)
            {
              cells[i] = " ";
            }

            if (!skipValues.Contains(i))
            {
              cells.Add(line[i]);
            }
            else
            {
              break;
            }
          }
          if (cells != null)
            DT.Rows.Add(cells.ToArray());
        }
        return DT;
      }
    }

    private void CleanHeaders()   //Currently not used..
    {
      if (matchedHeaders == null)
        return;
      for (int a = 0; a < matchedHeaders.Count; ++a)
      {
        switch (matchedHeaders[a])
        {
          case "Time":                      cleanedHeaders.Add("Time");               break;
          case "A/F CORR-B1 (%)":           cleanedHeaders.Add("AFR_Cor_B1");         break;
          case "A/F CORR-B2 (%)":           cleanedHeaders.Add("AFR_Cor_B2");         break;
          case "AFR WB-B1":                 cleanedHeaders.Add("AFR WB_B1");          break;
          case "AFR WB-B2":                 cleanedHeaders.Add("AFR WB_B2");          break;
          case "ACCEL PED POS 1 (V-Accel)": cleanedHeaders.Add("Accel_Pos");          break;
          case "ACCEL PED POS 2 (V-Accel)": cleanedHeaders.Add("Accel_Pos2");         break;
          case "B-FUEL SCHDL (ms)":         cleanedHeaders.Add("Base Fuel Schdl");    break;
          case "CAL/LD VALUE (%)":          cleanedHeaders.Add("Calc Load %");        break;
          case "Calculated Load Y Trace":   cleanedHeaders.Add("Calc Load Y Trace");  break;
          case "COOLANT TEMP":              cleanedHeaders.Add("Coolant Temp");       break;
          case "ENG OIL TEMP":              cleanedHeaders.Add("Oil Temp");           break;
          case "ENGINE RPM (rpm)":          cleanedHeaders.Add("RPM");                break;
          case "Fuel Compensation X Trace": cleanedHeaders.Add("Fuel Comp X Trace");  break;
          case "Fuel Compensation Y Trace": cleanedHeaders.Add("Fuel Comp Y Trace");  break;
          case "Fuel Target X Trace":       cleanedHeaders.Add("Fuel Trace X Trace"); break;
          case "Fuel Target Y Trace":       cleanedHeaders.Add("Fuel Trace Y Trace"); break;
          case "IGN TIMING(BTDC)":          cleanedHeaders.Add("Ign Timing");         break;
          case "INTAKE AIR TMP":            cleanedHeaders.Add("Intake_Temp");        break;
          case "KNOCK STRENGTH":            cleanedHeaders.Add("Knock");              break;
          case "LT Fuel Trim B1 (%)":       cleanedHeaders.Add("Long Trim B1");       break;
          case "LT Fuel Trim B2 (%)":       cleanedHeaders.Add("Long Trim B2");       break;
          case "MAS A/F -B1 (V)":           cleanedHeaders.Add("MAF_B1");             break;
          case "MAS A/F -B2 (V)":           cleanedHeaders.Add("MAF_B2");             break;
          case "MAF GM/s (gm/s)":           cleanedHeaders.Add("MAF GM/S");           break;
          case "TARGET AFR":                cleanedHeaders.Add("Target_AFR");         break;
          case "VACUUM SENS 1 (V)":         cleanedHeaders.Add("Vacuum");             break;
          case "VEHICLE SPEED":             cleanedHeaders.Add("Speed");              break;
          default: cleanedHeaders.Add("MissingLabel @ index " + a); break;
        }
        //ARC TIMING INTAKE AIR COMPENSATION Y TRACE,MAS ,Throttle Y Trace,BATTERY VOLT(Volts),,INTK CAM TIM-B2(Deg CA),
        //INJ PULSE-B1(ms),MAF GM/S(gm/s),THROTTLE SENSOR 1-B1(V),TURBINE REV(RPM),INJ DUTY(%),INT/V TIM(B1) (Deg CA),INT/V TIM(B2) (Deg CA),ARC TIMING COOLANT TEMP COMPENSATION Y TRACE,ARC TIMING INTAKE AIR COMPENSATION Y TRACE,ARC TIMING KNOCK Y TRACE,ARC TIMING OIL TEMP COMPENSATION Y TRACE,VVEL TIM-B1(deg),VVEL TIM-B2(deg)

      }
    }
  }
}
