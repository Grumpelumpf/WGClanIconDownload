using System;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace WGClanIconDownload
{

    public partial class MainWindow : Form
    {
        public static readonly WebClient[] Client = new WebClient[MaxThreads];
        /// <summary>
        /// The backgroundworker object on which the time consuming operation 
        /// shall be executed
        /// </summary>
        // BackgroundWorker m_oWorker;
        private const int MaxThreads = 4;
        private BackgroundWorker[] m_oWorker = new BackgroundWorker[MaxThreads];
        // public Dictionary<string, Dictionary<string, string>> dicData = Settings.regions;
        // public List<ClassDataArray> dataArray = new List<ClassDataArray>();
        public List<ClassDataArray> dataArray = new List<ClassDataArray>() { };

        public MainWindow()
        {
            InitializeComponent();
            Utils.appendLog("MainWindow Constructed");

            // add data to dataArray
            fillDataArray();

            // durchlaufe alle Regionen
            // foreach (var item in Settings.regionArr)
            foreach (var item in dataArray)
            {
                // Schreibe die möglichen Regionen in die ChecklistBox
                checkedListBoxRegion.Items.Add(item.region);
                string fold = Path.Combine(Settings.baseStorageFolder, item.data.storagePath);
                // prüfe ob ein entsprechendes Downloadverzeichnis bereits angelegt ist
                if (!Directory.Exists(fold))
                {
                    Directory.CreateDirectory(fold);
                    Utils.appendLog("Directory created => " + fold);
                }
                else
                {
                    // Utils.appendLog("Directory already exists => " + fold);
                }
            }

            /// https://stackoverflow.com/questions/10694271/c-sharp-multiple-backgroundworkers 
            for (var f = 0; f < MaxThreads; f++)
            {
                m_oWorker[f] = new BackgroundWorker();
                // Create a background worker thread that ReportsProgress &
                // SupportsCancellation
                // Hook up the appropriate events.
                m_oWorker[f].DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
                m_oWorker[f].ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
                m_oWorker[f].RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
                m_oWorker[f].WorkerReportsProgress = true;
                m_oWorker[f].WorkerSupportsCancellation = true;
                Client[f] = new WebClient();
            }
        }

        /// <summary>
        /// On completed do the appropriate task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            // The background process is complete. We need to inspect
            // our response to see if an error occurred, a cancel was
            // requested or if we completed successfully.  
            if (e.Cancelled)
            {
                progressLabel.Text = "Task Cancelled.";
            }
            // Check to see if an error occurred in the background process.
            else if (e.Error != null)
            {
                progressLabel.Text = "Error while performing background operation.";
            }
            else
            {
                // Everything completed normally.
                progressLabel.Text = "Task Completed...";
            }

            //Change the status of the buttons on the UI accordingly
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;

            // Utils.appendLog(ObjectDumper.Dump(dataArray));
        }

        /// <summary>
        /// Notification is performed here to the progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This function fires on the UI thread so it's safe to edit
            // the UI control directly, no funny business with Control.Invoke :)
            // Update the progressBar with the integer supplied to us from the
            // ReportProgress() function.  
            progressBar1.Value = e.ProgressPercentage;
            progressLabel.Text = string.Format("Processing......{0}%", progressBar1.Value.ToString());
        }


        /// <summary>
        /// Time consuming operations go here </br>
        /// i.e. Database operations,Reporting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] parameters = e.Argument as object[];       // the 'argument' parameter resurfaces here
            string region = (string)parameters[0];
            int thread = (int)parameters[1];

            string url = string.Format(Settings.wgApiURL, dataArray.Find(x => x.region == region).data.url, Settings.wgAppID, Settings.limit, dataArray.Find(x => x.region == region).data.currentPage);
            Utils.appendLog("url: " + url);

            //Handle the event for download complete
            Client[thread].DownloadDataCompleted += Client_DownloadAPIRequestCompleted;
            //Start downloading file
            Client[thread].DownloadDataAsync(new Uri(url), e.Argument);
            // WebRequest request = WebRequest.Create(string.Format(Settings.wgApiURL, dicData.GetOrAdd(region)["url"],Settings.wgAppID, Settings.limit, pageRequest));

            // The sender is the BackgroundWorker object we need it to
            // report progress and check for cancellation.
            //NOTE : Never play with the UI thread here...
            while (!e.Cancel)
            {
                // Thread.Sleep(100);

                // Periodically report progress to the main thread so that it can
                // update the UI.  In most cases you'll just need to send an
                // integer that will update a ProgressBar                    
                int progress = 0;
                if (dataArray.Find(x => x.region == region).data.total > 0 && dataArray.Find(x => x.region == region).data.count > 0)
                    progress = dataArray.Find(x => x.region == region).data.count / dataArray.Find(x => x.region == region).data.total * 100;
                m_oWorker[thread].ReportProgress(progress);
                // Periodically check if a cancellation request is pending.
                // If the user clicks cancel the line
                // m_AsyncWorker.CancelAsync(); if ran above.  This
                // sets the CancellationPending to true.
                // You must check this flag in here and react to it.
                // We react to it by setting e.Cancel to true and leaving
                if (m_oWorker[thread].CancellationPending)
                {
                    // Set the e.Cancel flag so that the WorkerCompleted event
                    // knows that the process was cancelled.
                    e.Cancel = true;
                    m_oWorker[thread].ReportProgress(0);
                    return;
                }
            }

            //Report 100% completion on operation completed
            m_oWorker[thread].ReportProgress(100);
        }

        void Client_DownloadAPIRequestCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            object[] parameters = e.UserState as object[];       // the 'argument' parameter resurfaces here
            string region = (string)parameters[0];
            int thread = (int)parameters[1];

            if (e.Error != null)
            {
                Utils.appendLog("Error: download failed\n" + e.Error);
            }
            else
            {
                //Get the data of the file
                string result = System.Text.Encoding.UTF8.GetString(e.Result);
                dynamic resultPageApiJson = JsonConvert.DeserializeObject(result);

                try
                {
                    // dataArray.Find(x => x.region == region).data.resultPageApiJson = resultPageApiJson;
                    // dataArray.Find(x => x.region == region).data.resultPageApiJson
                    if (resultPageApiJson.status != null)
                    {
                        Utils.appendLog((string)resultPageApiJson.status);
                        if (((string)resultPageApiJson.status).Equals("ok"))
                        {
                            Utils.appendLog("Request OK");
                            dataArray.Find(x => x.region == region).data.total = ((int)resultPageApiJson.meta.total);
                            try
                            {
                                if ((int)resultPageApiJson.meta.count > 0)
                                {
                                    clanData c;
                                    for (var f = 0; f < (int)resultPageApiJson.meta.count; f++)
                                    {
                                        c = new clanData();
                                        c.tag = (string)resultPageApiJson.data[f].tag;
                                        c.emblems = (string)resultPageApiJson.data[f].emblems.x32.portal;
                                        dataArray.Find(x => x.region == region).clans.Add(c);
                                    }

                                    // object[] parameters = new object[] { region, dataArray.Find(x => x.region == region).data.thread };
                                    dataArray.Find(x => x.region == region).data.currentPage++;
                                    dataArray.Find(x => x.region == region).data.count += (int)resultPageApiJson.meta.count;
                                    m_oWorker[thread].RunWorkerAsync(parameters);
                                }
                            }
                            catch (Exception ee)
                            {
                                Utils.exceptionLog(ee);
                            }
                        }
                        else
                        {
                            Utils.appendLog("Error. Result of request from API:\n" + ObjectDumper.Dump(dataArray));
                        }
                    }
                    else
                    {
                        Utils.dumpObjectToLog(string.Format("Error: failed to download at Server: {0}, Page {1}", region, dataArray.Find(x => x.region == region).data.currentPage), resultPageApiJson);
                        return;
                    }

                }
                catch (Exception ee)
                {
                    Utils.exceptionLog(ee);
                }
            }

            /*
            //Remove handler as no longer needed, if all m_oWorker are finished
            for (var f = 0; f < MaxThreads; f++)
            {
                if (!m_oWorker[f].IsBusy)
                {
                    Client[f].DownloadDataCompleted -= Client_DownloadAPIRequestCompleted;
                }
            }
            */
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Utils.appendLog("buttonStart_Click started");
            object param1 = 1;
            object param2 = 2;

            // Kickoff the worker thread to begin it's DoWork function.
            for (int i = 0; i < checkedListBoxRegion.Items.Count; i++)
            {
                if (checkedListBoxRegion.GetItemCheckState(i) == CheckState.Checked)
                {
                    //Change the status of the buttons on the UI accordingly
                    //The start button is disabled as soon as the background operation is started
                    //The Cancel button is enabled so that the user can stop the operation 
                    //at any point of time during the execution
                    buttonStart.Enabled = false;
                    buttonStop.Enabled = true;

                    // Do selected stuff
                    // The parameters you want to pass to the do work event of the background worker.
                    string region = (string)checkedListBoxRegion.Items[i];
                    object[] parameters = new object[] { region, i };
                    dataArray.Find(x => x.region == region).data.currentPage = 1;
                    dataArray.Find(x => x.region == region).data.count = 1;
                    dataArray.Find(x => x.region == region).data.thread = i;
                    m_oWorker[i].RunWorkerAsync(parameters);
                }
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < MaxThreads; i++)
            {
                if (m_oWorker[i].IsBusy)
                {
                    // Notify the worker thread that a cancel has been requested.
                    // The cancel will not actually happen until the thread in the
                    // DoWork checks the m_oWorker.CancellationPending flag. 
                    m_oWorker[i].CancelAsync();
                }
            }
        }

        public void fillDataArray()
        {
            ClassDataArray d;

            d = new ClassDataArray();
            d.region = "ASIA";
            d.data = new regionData();
            d.data.url = "worldoftanks.asia";
            d.data.storagePath = Settings.folderStructure.Replace("{reg}", d.region);
            dataArray.Add(d);

            d = new ClassDataArray();
            d.region = "NA";
            d.data = new regionData();
            d.data.url = "worldoftanks.com";
            d.data.storagePath = Settings.folderStructure.Replace("{reg}", d.region);
            dataArray.Add(d);

            d = new ClassDataArray();
            d.region = "EU";
            d.data = new regionData();
            d.data.url = "worldoftanks.eu";
            d.data.storagePath = Settings.folderStructure.Replace("{reg}", d.region);
            dataArray.Add(d);

            d = new ClassDataArray();
            d.region = "RU";
            d.data = new regionData();
            d.data.url = "worldoftanks.ru";
            d.data.storagePath = Settings.folderStructure.Replace("{reg}", d.region);
            dataArray.Add(d);
        }
    }
}
 
 