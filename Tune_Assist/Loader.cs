namespace AutoTune
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.IO;

  public class Loader
  {
    private List<string> matchedHeaders = new List<string>();

    public DataTable LoadLog(BackgroundWorker bw, string fileName)
    {
      int numLines = 0;
      using (DataTable dt = new DataTable())
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
        {
          lines[0].TrimEnd(',');
        }

        if (lines.Length < 10)
        {
          return dt;
        }

        List<string> headers = new List<string>(lines[0].Split(','));
        List<int> skipValues = new List<int>();
        FileInfo fi = new FileInfo(fileName);
        long totalBytes = fi.Length;
        long bytesRead = 0;
        int headerindex = 0;

        foreach (var label in headers)
        {
          if (!this.matchedHeaders.Contains(label))
          {
            this.matchedHeaders.Add(label);
          }
          else
          {
            skipValues.Add(headerindex);
          }

          headerindex++;
        }

        // Create columns from headers
        for (int h = 0; h < this.matchedHeaders.Count; ++h)
        {
          if (this.matchedHeaders[h] == null)
          {
            break;
          }

          dt.Columns.Add();
          dt.Columns[h].ColumnName = this.matchedHeaders[h];
          dt.Columns[h].ReadOnly = true;
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

          // Update ProgressBar
          bytesRead += sr.CurrentEncoding.GetByteCount(lines[x]);
          numLines++;
          int pctComplete = (int)(((double)bytesRead / (double)totalBytes) * 100);
          bw.ReportProgress(pctComplete);

          if (line == null || line.Length > headers.Count || line.Length < this.matchedHeaders.Count)
          {
            continue;
          }

          for (int i = 0; i < this.matchedHeaders.Count; ++i)
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
          {
            dt.Rows.Add(cells.ToArray());
          }
        }

        return dt;
      }
    }
  }
}
