using System;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Collections.Specialized;
using TinyJson;

namespace WGClanIconDownload
{

    public partial class MainWindow : Form
    {
        /// <summary>
        /// The backgroundworker object on which the time consuming operation 
        /// shall be executed
        /// </summary>
        private BackgroundWorker[] apiRequestWorker = new BackgroundWorker[] { };
        public List<ClassDataArray> dataArray = new List<ClassDataArray>() { };
        private List<BackgroundWorker> apiRequestWorkerList = new List<BackgroundWorker>() { };
        private List<WebClient> apiRequestWorkerList_WebClient = new List<WebClient>() { };

        public MainWindow()
        {
            InitializeComponent();
            Utils.appendLog("MainWindow Constructed");

            // add data to dataArray
            fillDataArray();

            // durchlaufe alle Regionen
            foreach (var item in dataArray)
            {
                // Schreibe die möglichen Regionen in die ChecklistBox
                checkedListBoxRegion.Items.Add(item.region);
                dataArray[item.indexOfDataArray].data.nameLabel.Text = item.region;
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
        }

        /// <summary>
        /// On completed do the appropriate task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void apiRequestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                // The background process is complete. We need to inspect
                // our response to see if an error occurred, a cancel was
                // requested or if we completed successfully.  
                if (e.Cancelled)
                {
                    progressLabel.Text = "Tasks cancelled.";
                    Utils.appendLog("Task cancelled");
                }
                // Check to see if an error occurred in the background process.
                else if (e.Error != null)
                {
                    progressLabel.Text = "Error while performing background operation.";
                    Utils.appendLog("Test 1");
                    var result = e.Result;
                    Utils.dumpObjectToLog("result", result);
                }
                else
                {
                    EventArgsParameter parameters = (EventArgsParameter)e.Result;       // the 'argument' parameter resurfaces here
                    string region = parameters.region;
                    int thread = parameters.thread;
                    int indexOfDataArray = parameters.indexOfDataArray;
                    // Everything completed normally.
                    progressLabel.Text = "Task "+region+" completed...";
                }

                for (var f = 0; f < apiRequestWorkerList.Count(); f++)
                {
                    if (apiRequestWorkerList[f].IsBusy)
                    {
                        return;
                    }
                }
                //Change the status of the buttons on the UI accordingly
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
            }
            catch (Exception ee)
            {
                Utils.exceptionLog(ee);
            }
        }

        /// <summary>
        /// Notification is performed here to the progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void apiRequestWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.UserState != null)
                {
                    EventArgsParameter parameters = (EventArgsParameter)e.UserState;       // the 'argument' parameter resurfaces here
                    string region = parameters.region;
                    int thread = parameters.thread;
                    int indexOfDataArray = parameters.indexOfDataArray;

                    // This function fires on the UI thread so it's safe to edit
                    // the UI control directly, no funny business with Control.Invoke :)
                    // Update the progressBar with the integer supplied to us from the
                    // ReportProgress() function.  
                    dataArray[indexOfDataArray].data.progressBar.Value = e.ProgressPercentage;
                    // progressLabel.Text = string.Format("Processing......{0}%", progressBar1.Value.ToString());
                }
            }
            catch (Exception ee)
            {
                Utils.exceptionLog(ee);
            }
        }

        /// <summary>
        /// Time consuming operations go here </br>
        /// i.e. Database operations,Reporting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void apiRequestWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                EventArgsParameter parameters = (EventArgsParameter)e.Argument;       // the 'argument' parameter resurfaces here
                string region = parameters.region;
                int thread = parameters.thread;
                int indexOfDataArray = parameters.indexOfDataArray;

                string url = string.Format(Settings.wgApiURL, dataArray[indexOfDataArray].data.url, Settings.wgAppID, Settings.limit, dataArray[indexOfDataArray].data.currentPage);
                //Handle the event for download complete
                apiRequestWorkerList_WebClient[thread].DownloadDataCompleted += apiRequestWorker_DownloadDataCompleted;
                //Start downloading file
                apiRequestWorkerList_WebClient[thread].DownloadDataAsync(new Uri(url), parameters);

                // The sender is the BackgroundWorker object we need it to
                // report progress and check for cancellation.
                //NOTE : Never play with the UI thread here...
                int progress = 0;
                while (!e.Cancel && progress != 100)
                {
                    Thread.Sleep(100);
                    e.Result = parameters;
                    // Periodically report progress to the main thread so that it can
                    // update the UI.  In most cases you'll just need to send an
                    // integer that will update a ProgressBar                    
                    if (dataArray[indexOfDataArray].data.total > 0 && dataArray[indexOfDataArray].data.count > 0)
                    {
                        progress = dataArray[indexOfDataArray].data.count * 100 / dataArray[indexOfDataArray].data.total;
                    }
                    apiRequestWorkerList[thread].ReportProgress(progress, parameters);
                    // Periodically check if a cancellation request is pending.
                    // If the user clicks cancel the line
                    // m_AsyncWorker.CancelAsync(); if ran above.  This
                    // sets the CancellationPending to true.
                    // You must check this flag in here and react to it.
                    // We react to it by setting e.Cancel to true and leaving
                    if (apiRequestWorkerList[thread].CancellationPending)
                    {
                        // Set the e.Cancel flag so that the WorkerCompleted event
                        // knows that the process was cancelled.
                        e.Cancel = true;
                        apiRequestWorkerList[thread].ReportProgress(0);
                        return;
                    }
                }

                //Report 100% completion on operation completed
                apiRequestWorkerList[thread].ReportProgress(100);
            }
            catch (Exception ee)
            {
                Utils.exceptionLog(ee);
            }
        }

        void apiRequestWorker_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                EventArgsParameter parameters = (EventArgsParameter)e.UserState;       // the 'argument' parameter resurfaces here
                string region = parameters.region;
                int thread = parameters.thread;
                int indexOfDataArray = parameters.indexOfDataArray;

                if (e.Error != null)
                {
                    Utils.appendLog("Error: download failed\n" + e.Error);
                }
                else
                {
                    try
                    {
                        //Get the data of the file
                        string result = System.Text.Encoding.UTF8.GetString(e.Result);
                        dynamic resultPageApiJson = result.FromJson<dynamic>();
                        try
                        {
                            if (resultPageApiJson != null)
                            {
                                Utils.appendLog((string)resultPageApiJson["status"]);
                                if (((string)resultPageApiJson["status"]).Equals("ok"))
                                {
                                    dataArray[indexOfDataArray].data.total = ((int)resultPageApiJson["meta"]["total"]);
                                    try
                                    {
                                        if ((int)resultPageApiJson["meta"]["count"] > 0)
                                        {
                                            clanData c;
                                            for (var f = 0; f < (int)resultPageApiJson["meta"]["count"]; f++)
                                            {
                                                c = new clanData();
                                                c.tag = (string)resultPageApiJson["data"][f]["tag"];
                                                c.emblems = (string)resultPageApiJson["data"][f]["emblems"]["x32"]["portal"];
                                                dataArray[indexOfDataArray].clans.Add(c);
                                            }
                                            dataArray[indexOfDataArray].data.currentPage++;
                                            dataArray[indexOfDataArray].data.count += (int)resultPageApiJson["meta"]["count"];
                                            string url = string.Format(Settings.wgApiURL, dataArray[indexOfDataArray].data.url, Settings.wgAppID, Settings.limit, dataArray[indexOfDataArray].data.currentPage);
                                            apiRequestWorkerList_WebClient[thread].DownloadDataAsync(new Uri(url), parameters);
                                            if (dataArray[indexOfDataArray].data.count == dataArray[indexOfDataArray].data.total)
                                            {
                                                apiRequestWorkerList_WebClient[thread].DownloadDataCompleted -= apiRequestWorker_DownloadDataCompleted;
                                                Utils.appendLog("apiRequestWorker_DownloadDataCompleted killed");
                                            }
                                        }
                                    }
                                    catch (Exception ee)
                                    {
                                        Utils.exceptionLog(ee);
                                    }
                                }
                                else
                                {
                                    Utils.appendLog("Error. Result of request from API:\n" + ObjectDumper.Dump(resultPageApiJson));
                                }
                            }
                            else
                            {
                                Utils.dumpObjectToLog(string.Format("Error: failed to download at Server: {0}, Page {1}", region, dataArray[indexOfDataArray].data.currentPage), result);
                                return;
                            }
                        }
                        catch (Exception ee)
                        {
                            Utils.exceptionLog(ee);
                        }
                    }
                    catch (Exception ej)
                    {
                        Utils.exceptionLog(ej);
                    }
                }
            }
            catch (Exception eh)
            {
                Utils.exceptionLog(eh);
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Utils.appendLog("buttonStart_Click");
            if (checkedListBoxRegion.Items.Count > 0)
            {
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
                        EventArgsParameter parameters = new EventArgsParameter();//  = e.Argument as EventArgsParameter;       // the 'argument' parameter resurfaces here
                        parameters.region = (string)checkedListBoxRegion.Items[i];
                        parameters.thread = apiRequestWorkerList.Count;
                        parameters.indexOfDataArray = dataArray.Find(x => x.region == parameters.region).indexOfDataArray;

                        dataArray[parameters.indexOfDataArray].data.currentPage = 1;
                        dataArray[parameters.indexOfDataArray].data.count = 0;
                        dataArray[parameters.indexOfDataArray].data.thread = parameters.thread;

                        /// https://stackoverflow.com/questions/10694271/c-sharp-multiple-backgroundworkers 
                        /// Create a background worker thread that ReportsProgress &
                        /// SupportsCancellation
                        /// Hook up the appropriate events.
                        BackgroundWorker apiRequestWorker = new BackgroundWorker();
                        apiRequestWorker.DoWork += new DoWorkEventHandler(apiRequestWorker_DoWork);
                        apiRequestWorker.ProgressChanged += new ProgressChangedEventHandler(apiRequestWorker_ProgressChanged);
                        apiRequestWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(apiRequestWorker_RunWorkerCompleted);
                        apiRequestWorker.WorkerReportsProgress = true;
                        apiRequestWorker.WorkerSupportsCancellation = true;
                        apiRequestWorkerList.Add(apiRequestWorker);
                        WebClient apiRequestWorker_WebClient = new WebClient();
                        apiRequestWorkerList_WebClient.Add(apiRequestWorker_WebClient);
                        apiRequestWorkerList[parameters.thread].RunWorkerAsync(parameters);
                        Utils.appendLog("RunWorkerAsync thread " + parameters.region + " started");
                    }
                }
            }
            else
            {
                Utils.appendLog("no selection, no work ;-)");
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < apiRequestWorkerList.Count; i++)
                {
                    if (apiRequestWorkerList[i].IsBusy)
                    {
                        // Notify the worker thread that a cancel has been requested.
                        // The cancel will not actually happen until the thread in the
                        // DoWork checks the apiRequestWorker.CancellationPending flag. 
                        apiRequestWorkerList[i].CancelAsync();
                    }
                }
            }
            catch (Exception ee)
            {
                Utils.exceptionLog(ee);
            }
        }

        public void fillDataArray()
        {
            ClassDataArray d;

            d = new ClassDataArray();
            d.region = "ASIA";
            d.indexOfDataArray = dataArray.Count;
            d.data = new regionData();
            d.data.url = "worldoftanks.asia";
            d.data.storagePath = Settings.folderStructure.Replace("{reg}", d.region);
            d.data.progressBar = progressBar1;
            d.data.nameLabel = progressName_Label1;
            d.data.previewIconBox = clanIconPreview_PictureBox1;
            dataArray.Add(d);

            d = new ClassDataArray();
            d.region = "NA";
            d.indexOfDataArray = dataArray.Count;
            d.data = new regionData();
            d.data.url = "worldoftanks.com";
            d.data.storagePath = Settings.folderStructure.Replace("{reg}", d.region);
            d.data.progressBar = progressBar3;
            d.data.nameLabel = progressName_Label3;
            d.data.previewIconBox = clanIconPreview_PictureBox3;
            dataArray.Add(d);

            d = new ClassDataArray();
            d.region = "EU";
            d.indexOfDataArray = dataArray.Count;
            d.data = new regionData();
            d.data.url = "worldoftanks.eu";
            d.data.storagePath = Settings.folderStructure.Replace("{reg}", d.region);
            d.data.progressBar = progressBar2;
            d.data.nameLabel = progressName_Label2;
            d.data.previewIconBox = clanIconPreview_PictureBox2;
            dataArray.Add(d);

            d = new ClassDataArray();
            d.region = "RU";
            d.indexOfDataArray = dataArray.Count;
            d.data = new regionData();
            d.data.url = "worldoftanks.ru";
            d.data.storagePath = Settings.folderStructure.Replace("{reg}", d.region);
            d.data.progressBar = progressBar4;
            d.data.nameLabel = progressName_Label4;
            d.data.previewIconBox = clanIconPreview_PictureBox4;
            dataArray.Add(d);
        }
    }
}
 
 