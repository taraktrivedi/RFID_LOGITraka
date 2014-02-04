using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZTraka_App
{
    /// <summary>
    /// LogFile class to write logs about app events  in the user hard disk
    /// </summary>
    public class LogFile
    {

        /// <summary>
        /// File location variable
        /// </summary>
        public static string fileLoc = "";

        /// <summary>
        /// Constructor of the <see cref="LogFile" /> class.
        /// </summary>
        static LogFile()
        {
            // Get the app data \roaming path and create a folder if it doesnt exist
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ZigTraka")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ZigTraka"));
            }
            LogFile.fileLoc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ZigTraka\LOGITraka App Event Logs.log");

            //Check if file exists
            FileStream fs = null;
            if (!File.Exists(fileLoc))
            {

                using (fs = File.Create(fileLoc))
                {

                }

            }
            else
            {
                //do nothing
            }
            

            using (StreamWriter sw = new StreamWriter(fileLoc))
            {
                
                sw.WriteLine(@"************************************************************************************************");
                sw.WriteLine(@"----------------------Logging Events, Tasks and Operations... ----------------------------------");
                sw.WriteLine(@"************************************************************************************************");
                sw.WriteLine("");
                sw.WriteLine("Start Application Loggging...");
                sw.WriteLine("");
                sw.Write(DateTime.Now.ToString() + ": ");
                sw.Write("App Start Event -Done! Time: " + DateTime.Now.ToLongTimeString() + " ");
                
            }

 
        }

        /// <summary>
        /// Logs the specified log message.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        public static void Log(String logMessage)
        {
            using (TextWriter tWriter = File.AppendText(fileLoc))
            {
                tWriter.WriteLine("");
                tWriter.Write(DateTime.Now.ToString() + ": ");
                tWriter.Write( logMessage + " ");
                tWriter.Write("-Complete-|");
                // Update the underlying file.
                tWriter.Flush(); // No need actually.
            }
        }



    }
}
