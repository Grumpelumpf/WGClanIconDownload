using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using System.Net.Http;
using System.Net;

namespace WGClanIconDownload
{
    public partial class Settings
    {
        public static readonly WebClient Client = new WebClient();
        // public static string baseStorageFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        // public static string baseStorageFolder = Path.GetDirectoryName(Application.ExecutablePath);
        public static string baseStorageFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        // public static string baseStorageFolder = Path.GetDirectoryName(Process.GetCurrentProcess).MainModule.FileName);
        // public static string ErrorLogFile = Path.Combine(baseStorageFolder, "logs", "error.log");
        public static string errorLogFile = @"C:\Users\Ich\Desktop\ClanDownload.log";
        public static string processLogFile = Path.Combine(baseStorageFolder,"logs","process.dat");
        public static string folderStructure = @"download\{reg}\res_mods\mods\shared_resources\xvm\res\clanicons\{reg}\clan\";
        public static string wgAppID = "d0bfec3ab1967d9582a73fef7d86ff02";
        /// <summary>
        /// the URL to wg API with removed unneeded fields
        /// </summary>
        /// <param {0}="baseURLRegion"></param>
        /// <param {1}="wgAppID"></param>
        /// <param {2}="limit"></param>
        /// <param {3}="page"></param>
        public static string wgApiURL = @"https://api.{0}/wgn/clans/list/?application_id={1}&fields=-emblems.x195,-emblems.x24,-emblems.x256,-emblems.x64,-created_at,-color,-clan_id,-members_count,-name&game=wot&limit={2}&page_no={3}";
        public static string[] prohibitedFilenames = new string[] {
            "CON","PRN","AUX","CLOCK$","NUL","COM0","COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9","LPT0","LPT1","LPT2","LPT3","LPT4","LPT5","LPT6","LPT7","LPT8","LPT9"
        };
        public static string[] imagesRes = new string[] {
            "x32","x24","x64","x195","x256"
        };
        public static string[] imageIndex = new string[] {
            "wot","portal","wowp"
        };
        // public static List<ClassDataArray> dataArray = new List<ClassDataArray>() { };
        // public int total = 0;
        // public int clanCounter = 0;
        public static int limit = 100;
        public bool resumeProcess = false;

        // System.Console.WriteLine
    }
}