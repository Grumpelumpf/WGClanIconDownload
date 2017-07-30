using System.Collections.Generic;
using System.Windows.Forms;

namespace WGClanIconDownload
{
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
        public static void AddMany<TKey1, TKey2, TKey3, TValue>(this Dictionary<TKey1, Dictionary<TKey2, Dictionary<TKey3, TValue>>> dictionary,TKey1 key1,TKey2 key2,TKey3 key3,TValue newValue)
        {
            dictionary.GetOrAdd(key1).GetOrAdd(key2)[key3] = newValue;
        }
    }

    public class regionData
    {
        public string url { get; set; } = null;
        public int thread { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public int total { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public int count { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public int currentPage { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public string storagePath { get; set; } = null;
        public ProgressBar progressBar { get; set; } = null;
        public Label nameLabel { get; set; } = null;
        public PictureBox previewIconBox { get; set; } = null;
        // public dynamic resultPageApiJson { get; set; } = null;
    }

    public class clanData
    {
        public string tag { get; set; } = null;
        public string emblems { get; set; } = null;
        public clanData() { }
    }

    public class ClassDataArray
    {
        public string region { get; set; }
        public int dlErrorCounter { get; set; } = 0;
        public int indexOfDataArray { get; set; } = Constants.INVALID_HANDLE_VALUE;
        public regionData data { get; set; }
        public List<clanData> clans = new List<clanData>();
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
        public int indexOfDataArray { get; set; } = Constants.INVALID_HANDLE_VALUE;
    }
}
