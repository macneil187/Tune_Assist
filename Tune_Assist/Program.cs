namespace AutoTune
{
  using System;
  using System.Windows.Forms;

  static class Program
  {
    [STAThread]
    static void Main(string[] arg)
    {
      try
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        if (arg.Length != 0)
        {
          AutoTune.FileName = arg[0];
        }

        Application.Run(new AutoTune());
      }
      catch
      {
        Console.Out.WriteLine("Error starting program!");
      }
    }
  }
}
