using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

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
        public static int limitApiPageRequest = 100;
        public static int viaUiThreadsAllowed = 2;
    }

    public class regionData
    {
        public string url { get; set; } = null;
        public int thread { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public int total { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public int countApiRequest { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public int countIconDownload { get; set; } = 0;
        public int currentPage { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public string storagePath { get; set; } = null;
        public ProgressBar progressBar { get; set; } = null;
        public Label nameLabel { get; set; } = null;
        public PictureBox previewIconBox { get; set; } = null;
        public Label progressPage_Label { get; set; } = null;
        public Label showThreadCount_Label { get; set; } = null;
        public Label downloadCounter_Label { get; set; } = null;
        // public dynamic resultPageApiJson { get; set; } = null;
    }

    public class clanData
    {
        public string tag { get; set; } = null;
        public string emblems { get; set; } = null;
        public clanData() { }
    }

    public class threadData
    {
        public List<clanData> clansToProcessBuffer = new List<clanData>();
        public BackgroundWorker fileDownloadWorker { get; set; } = null;
        public int fileDownloadWorkerThreadID { get; set; } = Constants.INVALID_HANDLE_VALUE;
        /// <summary>
        /// ONLY for report back of a finished "fileDownloadWorkerList" thread
        /// </summary>
        public bool threadFinished { get; set; } = false;
        /// <summary>
        /// ONLY for report from "SetfileDownloadWorker" to "fileDownloadWorkerList" thread
        /// </summary>
        public bool threadAdvisedToFinish { get; set; } = false;
        public bool waitToFillBuffer { get; set; } = true;
        public threadData() { }
    }

    public class ClassDataArray
    {
        public string region { get; set; }
        public int dlErrorCounter { get; set; } = 0;
        public int indexOfDataArray { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public bool initialized { get; set; } = false;
        public bool regionFinished { get; set; } = true;
        public regionData data { get; set; }
        public List<clanData> clans = new List<clanData>();
        public List<threadData> threadList = new List<threadData>();
        public ClassDataArray() { }
    }

    static class Constants
    {
        public const double Pi = 3.14159;
        public const int SpeedOfLight = 300000; // km per sec.
        public const int INVALID_HANDLE_VALUE = -1;
    }

    public class EventArgsParameter
    {
        public string region { get; set; } = null;
        public int thread { get; set; } = Constants.INVALID_HANDLE_VALUE;
        /// <summary>
        /// needed to identify the "fileDownloadWorker" object at the threadList
        /// </summary>
        public int threadID { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public int indexOfDataArray { get; set; } = Constants.INVALID_HANDLE_VALUE;
        /// <summary>
        /// ONLY used at communication between downloadThreadHandler_DoWork and SetfileDownloadWorker
        /// </summary>
        public int threadCorrection { get; set; } = 0;
        /// <summary>
        /// ONLY used at communication between apiRequestWorker_DoWork and apiRequestWorker_ProgressChanged
        /// </summary>
        public string progressChangedString { get; set; } = "";
        /// <summary>
        /// ONLY for communikation between fileDownloadWorker_DoWork and fileDownloadWorker_ProgressChanged
        /// </summary>
        public string pictureBoxFileLink { get; set; } = "";
    }

    public static class DataTools                       /// https://stackoverflow.com/questions/26789056/c-sharp-easy-way-to-add-keys-and-values-to-nested-dictionary
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue newValue)
        {
            TValue oldValue;
            if (dictionary.TryGetValue(key, out oldValue))
                return oldValue;
            else
            {
                dictionary.Add(key, newValue);
                return newValue;
            }
        }
        public static void AddMany<TKey1, TKey2, TValue>(this Dictionary<TKey1, Dictionary<TKey2, TValue>> dictionary, TKey1 key1, TKey2 key2, TValue newValue)
        {
            dictionary.GetOrAdd(key1)[key2] = newValue;
        }


        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            TValue oldValue;
            if (dictionary.TryGetValue(key, out oldValue))
                return oldValue;
            else
            {
                var newValue = new TValue();
                dictionary.Add(key, newValue);
                return newValue;
            }
        }
        public static void AddMany<TKey1, TKey2, TKey3, TValue>(this Dictionary<TKey1, Dictionary<TKey2, Dictionary<TKey3, TValue>>> dictionary, TKey1 key1, TKey2 key2, TKey3 key3, TValue newValue)
        {
            dictionary.GetOrAdd(key1).GetOrAdd(key2)[key3] = newValue;
        }
    }
}