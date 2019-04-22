using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureParser
{
    public static class Log
    {
        private static readonly string Time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        private static readonly string LogFolder = AppDomain.CurrentDomain.BaseDirectory + "Log";
        private static readonly string File = "Log" + @Time + ".log";
        private static readonly string Path = LogFolder + @"\" + File;
        private static readonly StreamWriter StreamWriter;
        private static readonly FileStream FileStream;

        static Log()
        {
            try
            {
#if DEBUG
                StreamWriter = new StreamWriter(Console.OpenStandardOutput()) {AutoFlush = true};
                Console.SetOut(StreamWriter);
#else
                if (!Directory.Exists(LogFolder))
                {
                    Directory.CreateDirectory(LogFolder);
                }

                FileStream = new FileStream(Path, FileMode.Append);
                StreamWriter = new StreamWriter(FileStream, Encoding.UTF8) {AutoFlush = true};
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Exception happened when redirect stream to console out: " + e);
#else
                Console.WriteLine("Exception happened when create folder or new StreamWriter: " + e);
#endif
                throw;
            }

            AppDomain.CurrentDomain.ProcessExit += Log_Dtor;
        }

        public static void WriteLine(string message, Exception exception = null)
        {
            var nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            StreamWriter.WriteLine("-------" + nowTime + "-------");
            StreamWriter.WriteLine(message);
            if (exception != null)
            {
                StreamWriter.WriteLine(exception);
            }
            StreamWriter.WriteLine("-----------------------------");
        }

        private static void Log_Dtor(object sender, EventArgs e)
        {
            StreamWriter.Dispose();
            FileStream.Dispose();
        }
    }
}
