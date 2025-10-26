#region Using Statements
using System;
using System.IO;
#endregion

namespace CTR_MonoGame
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static CTRGame game;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Disable OpenTK input system to prevent registry access errors
            Environment.SetEnvironmentVariable("OPENGL_DISABLE_INPUT", "1");
            Environment.SetEnvironmentVariable("OPENGL_NO_INPUT", "1");
            Environment.SetEnvironmentVariable("OPENGL_SKIP_INPUT", "1");
            Environment.SetEnvironmentVariable("OPENGL_DISABLE_RAW_INPUT", "1");
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            FCOptionsLocation.FILE_LOCATION = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CTR/");
            
            game = new CTRGame();
            game.Run();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Console.WriteLine(ex.ToString());
        }
    }
}
