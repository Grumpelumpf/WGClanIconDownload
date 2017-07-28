using System;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel;

namespace WGClanIconDownload
{
    public class Utils
    {
        public static void clearLog()
        {
            File.Create(Settings.errorLogFile).Dispose();  
            appendLog("Log opened. (Time zone: " + DateTime.Now.ToString("\"GMT\"zzz") + ")");
        }

        public static void appendLog(string info)
        {
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff   ");
            info = info.Replace("\n", "\n" + string.Concat(Enumerable.Repeat(" ", 26))) + "\n";
            bool _ready = false;
            while (!_ready)
            {
                try
                {
                    File.AppendAllText(Settings.errorLogFile, currentDate + info);
                    _ready = true;
                }
                catch { }
            }
        }

        public static bool isDebugMode()
        {
#if DEBUG
                return true;
#else   
                return false;
#endif
        }

        // print all information about the object to the logfile
        public static void dumpObjectToLog(string objectName, object n)
        {
            Utils.appendLog(String.Format("----- dump of object {0} ------", objectName));
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(n))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(n);
                switch (value)
                {
                    case null:
                        value = "(null)";
                        break;
                    case "":
                        value = "(string with lenght 0)";
                        break;
                    default:
                        break;
                }
                Utils.appendLog(string.Format("{0}={1}", name, value));
            }
            Utils.appendLog("----- end of dump ------");
        }

        /// <summary>
        /// default logging function of exception informations
        /// </summary>
        /// <param e=Exception>the exception object that would be catched</param>
        public static void exceptionLog(Exception e)
        {
            Utils.appendLog("EXCEPTION (call stack traceback):");
            try { Utils.appendLog(e.StackTrace); } catch { };
            try { Utils.appendLog("message: " + e.Message); } catch { };
            try { Utils.appendLog("source: " + e.Source); } catch { };
            try { Utils.appendLog("target: " + e.TargetSite); } catch { };
            try { Utils.appendLog("InnerException: " + e.InnerException); } catch { };
            try { if (e.Data != null) Utils.dumpObjectToLog("Data",e.Data); } catch { };             /// https://msdn.microsoft.com/de-de/library/system.exception.data(v=vs.110).aspx
        }
    }
}
