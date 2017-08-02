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
// using TinyJson;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Drawing;
using Newtonsoft.Json;

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
        private List<AwesomeWebClient> apiRequestWorkerList_WebClient = new List<AwesomeWebClient>() { };
        private BackgroundWorker downloadThreadHandler;

        public MainWindow()
        {
            InitializeComponent();

            threads_numericUpDown.Value = Settings.viaUiThreadsAllowed;


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
                    var result = e.Result;
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
                    string progressChangedString = parameters.progressChangedString;

                    // This function fires on the UI thread so it's safe to edit
                    // the UI control directly, no funny business with Control.Invoke :)
                    // Update the progressBar with the integer supplied to us from the
                    // ReportProgress() function.  
                    dataArray[indexOfDataArray].data.progressPage_Label.Text = progressChangedString;
                    // dataArray[indexOfDataArray].data.progressBar.Value = e.ProgressPercentage;
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

                string url = string.Format(Settings.wgApiURL, dataArray[indexOfDataArray].data.url, Settings.wgAppID, Settings.limitApiPageRequest, dataArray[indexOfDataArray].data.currentPage);
                //Handle the event for download complete
                apiRequestWorkerList_WebClient[thread].DownloadDataCompleted += apiRequestWorker_DownloadDataCompleted;
                //Start downloading file
                apiRequestWorkerList_WebClient[thread].DownloadDataAsync(new Uri(url), parameters);

                // The sender is the BackgroundWorker object we need it to
                // report progress and check for cancellation.
                // NOTE : Never play with the UI thread here...
                int progress = 0;
                while (!e.Cancel && progress != 100 && dataArray[indexOfDataArray].data.currentPage != Math.Ceiling((decimal)dataArray[indexOfDataArray].data.total / (decimal)Settings.limitApiPageRequest))
                {
                    Thread.Sleep(100);
                    e.Result = parameters;
                    // Periodically report progress to the main thread so that it can
                    // update the UI.  In most cases you'll just need to send an
                    // integer that will update a ProgressBar                    
                    parameters.progressChangedString = string.Format("{0}/{1}", dataArray[indexOfDataArray].data.currentPage, Math.Ceiling((decimal)dataArray[indexOfDataArray].data.total/(decimal)Settings.limitApiPageRequest));
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
                    string result = System.Text.Encoding.UTF8.GetString(e.Result);
                    try
                    {
                        //Get the data of the file
                        // tinyJSON dynamic resultPageApiJson = result.FromJson<dynamic>();
                        dynamic resultPageApiJson = JsonConvert.DeserializeObject(result);
                        try
                        {
                            if (resultPageApiJson != null)
                            {
                                try
                                {
                                    // tinyJSON if (((string)resultPageApiJson["status"]).Equals("ok"))
                                    if (((string)resultPageApiJson.status).Equals("ok"))
                                    {
                                        // tinyJSON dataArray[indexOfDataArray].data.total = ((int)resultPageApiJson["meta"]["total"]);
                                        dataArray[indexOfDataArray].data.total = ((int)resultPageApiJson.meta.total);
                                        dataArray[indexOfDataArray].data.progressBar.Maximum = dataArray[indexOfDataArray].data.total;
                                        dataArray[indexOfDataArray].initialized = true;
                                        try
                                        {
                                            // tinyJSON if ((int)resultPageApiJson["meta"]["count"] > 0)
                                            if ((int)resultPageApiJson.meta.count > 0)
                                            {
                                                clanData c;
                                                // tinyJSON for (var f = 0; f < (int)resultPageApiJson["meta"]["count"]; f++)
                                                for (var f = 0; f < (int)resultPageApiJson.meta.count; f++)
                                                {
                                                    c = new clanData();
                                                    // tinyJSON c.tag = (string)resultPageApiJson["data"][f]["tag"];
                                                    c.tag = (string)resultPageApiJson.data[f].tag;
                                                    // tinyJSON c.emblems = (string)resultPageApiJson["data"][f]["emblems"]["x32"]["portal"];
                                                    c.emblems = (string)resultPageApiJson.data[f].emblems.x32.portal;
                                                    dataArray[indexOfDataArray].clans.Add(c);
                                                }
                                                dataArray[indexOfDataArray].data.currentPage++;
                                                // tinyJSON dataArray[indexOfDataArray].data.countApiRequest += (int)resultPageApiJson["meta"]["count"];
                                                dataArray[indexOfDataArray].data.countApiRequest += (int)resultPageApiJson.meta.count;
                                                string url = string.Format(Settings.wgApiURL, dataArray[indexOfDataArray].data.url, Settings.wgAppID, Settings.limitApiPageRequest, dataArray[indexOfDataArray].data.currentPage);
                                                apiRequestWorkerList_WebClient[thread].DownloadDataAsync(new Uri(url), parameters);
                                                if (dataArray[indexOfDataArray].data.countApiRequest == dataArray[indexOfDataArray].data.total)
                                                {
                                                    apiRequestWorkerList_WebClient[thread].DownloadDataCompleted -= apiRequestWorker_DownloadDataCompleted;
                                                    Utils.appendLog("apiRequestWorker_DownloadDataCompleted killed");
                                                }
                                            }
                                            else                // es gibt keine Datensätze mehr und das holen der "Pages" ist abgeschlossen.
                                            {
                                                apiRequestWorkerList_WebClient[thread].DownloadDataCompleted -= apiRequestWorker_DownloadDataCompleted;
                                                Utils.appendLog("apiRequestWorker_DownloadDataCompleted killed");
                                            }
                                        }
                                        catch (Exception ee)
                                        {
                                            Utils.appendLog("Error at apiRequestWorker_DownloadDataCompleted => inner Block");
                                            Utils.exceptionLog(ee);
                                            recallApiRequestWorkerList(thread, indexOfDataArray, parameters);
                                        }
                                    }
                                    else
                                    {
                                        // Utils.appendLog("if ((int)resultPageApiJson['meta']['count'] > 0)' + (string)resultPageApiJson['status']");
                                        // Utils.appendLog("Error. Result of request from API:\n" + resultPageApiJson.ToString());
                                        recallApiRequestWorkerList(thread, indexOfDataArray, parameters);
                                    }
                                }
                                catch (Exception ac)

                                {
                                    // Utils.appendLog("if (((string)resultPageApiJson['status']).Equals('ok'))");
                                    // Utils.exceptionLog(ac);
                                    recallApiRequestWorkerList(thread, indexOfDataArray, parameters);
                                }
                            }
                            else
                            {
                                recallApiRequestWorkerList(thread, indexOfDataArray, parameters);
                            }

                        }
                        catch (Exception ab)
                        {
                            // Utils.appendLog("if (resultPageApiJson != null)");
                            // Utils.exceptionLog(ab);
                            recallApiRequestWorkerList(thread, indexOfDataArray, parameters);
                        }
                    }
                    catch (Exception aa)
                    {
                        // Utils.appendLog("dynamic resultPageApiJson = result.FromJson<dynamic>();");
                        // Utils.exceptionLog(aa);
                        recallApiRequestWorkerList(thread, indexOfDataArray, parameters);
                    }
                }
            }
            catch (Exception eh)
            {
                Utils.exceptionLog(eh);
            }
        }

        private void recallApiRequestWorkerList(int thread, int indexOfDataArray, EventArgsParameter parameters)
        {
            string url = string.Format(Settings.wgApiURL, dataArray[indexOfDataArray].data.url, Settings.wgAppID, Settings.limitApiPageRequest, dataArray[indexOfDataArray].data.currentPage);
            apiRequestWorkerList_WebClient[thread].DownloadDataAsync(new Uri(url), parameters);
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
                        dataArray[parameters.indexOfDataArray].data.countApiRequest = 0;
                        dataArray[parameters.indexOfDataArray].data.thread = parameters.thread;
                        dataArray[parameters.indexOfDataArray].regionFinished = false;

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
                        AwesomeWebClient apiRequestWorker_WebClient = new AwesomeWebClient();
                        apiRequestWorkerList_WebClient.Add(apiRequestWorker_WebClient);
                        apiRequestWorkerList[parameters.thread].RunWorkerAsync(parameters);
                        Utils.appendLog("apiRequest RunWorkerAsync thread " + parameters.region + " started");
                    }
                }
                try
                {
                    downloadThreadHandler = new BackgroundWorker();
                    downloadThreadHandler.DoWork += downloadThreadHandler_DoWork;
                    // downloadThreadHandler.ProgressChanged += downloadThreadHandler_ProgressChanged;
                    downloadThreadHandler.WorkerReportsProgress = false;
                    downloadThreadHandler.RunWorkerCompleted += downloadThreadHandler_RunWorkerCompleted;
                    downloadThreadHandler.RunWorkerAsync();
                }
                catch (Exception ee)
                {
                    Utils.exceptionLog(ee);
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

        // private void btnGo_Click(object sender, RoutedEventArgs e)
        private void downloadThreadHandler_DoWork(object sender, DoWorkEventArgs e)
        {
            Utils.appendLog("downloadThreadHandler_DoWork started");
            Thread.Sleep(10000);
            // Utils.appendLog("downloadThreadHandler_DoWork finished Sleep 10000");
            // hier läuft die "Support"-Schleife für alle Threats ob:
            // die Download Threats erhöhen oder reduzieren
            // der "Nachschub" an Listelemeten tag und URL für die einzelenn Threats (wenn kein Threat nicht rausgenommen werden soll: Vorrat der Arbeitsliste (tag, URL) < 5 Stück, dann 10 holen und dazu schieben.
            // Reduzierung von Threats: Marker setzten und auslaufen lassen.
            int totalDownloadsLeft = Constants.INVALID_HANDLE_VALUE;
            bool allFinished = false;
            // && totalDownloadsLeft != 0
            while (!allFinished)
            {
                totalDownloadsLeft = 0;
                int threadsAllowed = Settings.viaUiThreadsAllowed;
                allFinished = true;
                foreach (var r in dataArray)
                {
                    // if (!r.regionFinished && r.clans.Count > 0)
                    // {
                    try
                    {
                        totalDownloadsLeft += r.clans.Count;
                        EventArgsParameter parameters = new EventArgsParameter();
                        parameters.region = r.region;
                        parameters.indexOfDataArray = r.indexOfDataArray;
                        parameters.thread = Constants.INVALID_HANDLE_VALUE;
                        parameters.threadID = Constants.INVALID_HANDLE_VALUE;
                        parameters.threadCorrection = threadsAllowed - r.threadList.Count;
                        if (r.threadList.Count != threadsAllowed && !r.regionFinished && r.data.currentPage > 0 )
                        {
                            Thread.Sleep(50);
                            SetFileDownloadWorker(sender, parameters);
                        }
                        r.data.showThreadCount_Label.Text = r.threadList.Count().ToString();

                        for (var f = r.threadList.Count() - 1; f >= 0; f--)
                        {
                            if (r.threadList[f].clansToProcessBuffer.Count > 0) { r.threadList[f].waitToFillBuffer = false; };
                            if (r.threadList[f].threadFinished)
                            {
                                Thread.Sleep(15);
                                var itemToRemove = r.threadList.SingleOrDefault(t2 => t2.fileDownloadWorkerThreadID == r.threadList[f].fileDownloadWorkerThreadID);
                                if (itemToRemove == null)
                                {
                                    Utils.appendLog("Error: threadID to kill not exists");
                                }
                                else
                                {
                                    r.threadList.Remove(itemToRemove);
                                    r.data.showThreadCount_Label.Text = r.threadList.Count().ToString();
                                    continue;
                                }
                            }

                            // jetzt müssen die Puffer mit Arbeitsdaten für die Downloader gefüllt werden.
                            if (r.clans.Count == 0 && !r.threadList[f].waitToFillBuffer)                          // wenn alle API request List leer ist, alle threads runterfahren.
                            {
                                r.threadList[f].threadAdvisedToFinish = true;
                            }
                            else if (r.threadList[f].clansToProcessBuffer.Count < 5 && !r.threadList[f].threadAdvisedToFinish)       // wenn weniger als 5 Elemente im Puffer sind, die nächsten holen und zum Puffer schieben
                            {
                                int range = 20;
                                // if (t.clansToProcessBuffer.Count == 0) range = 100; // bei beginn kann etwas mehr drauf ...
                                if (r.clans.Count < range) { range = r.clans.Count; };
                                var movingList = r.clans.GetRange(0, range);
                                r.threadList[f].clansToProcessBuffer.AddRange(movingList);
                                r.clans.RemoveRange(0, range);
                                r.threadList[f].waitToFillBuffer = false;
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        Utils.appendLog("Exception: totalDownloadsLeft += r.clans.Count;");
                        Utils.exceptionLog(ee);
                    }
                    if (r.threadList.Count == 0 && r.data.total > 0)
                    {
                        Utils.appendLog("downloadThreadHandler_DoWork threadList null");
                        r.regionFinished = true;
                        r.data.progressBar.Value = 0;
                    }
                    if (r.threadList.Count > 0 || r.data.total > 0 || !r.initialized)
                    {
                        allFinished = false;
                    }
                }
                Thread.Sleep(50);
            }
            Utils.appendLog("downloadThreadHandler_DoWork finished");
        }

        private void SetFileDownloadWorker(object sender, EventArgsParameter parameters)
        {
            // Utils.appendLog("SetFileDownloadWorker started");
            if (parameters.threadCorrection < 0)
            {
                foreach (var r in dataArray[parameters.indexOfDataArray].threadList)
                {
                    // var usedThreadIDs = new List<int>() { };
                    // foreach (var u in dataArray[parameters.indexOfDataArray].threadList)
                    if (r.fileDownloadWorkerThreadID > Settings.viaUiThreadsAllowed-1)
                    {
                        r.threadAdvisedToFinish = true;
                        r.waitToFillBuffer = false;
                    }
                    else
                    {
                        r.threadAdvisedToFinish = false;
                        // r.waitToFillBuffer = true;
                    }
                }
                // mark threads to finish after last item in "clansToProcessBuffer"
            }
            else if (parameters.threadCorrection > 0 && !dataArray[parameters.indexOfDataArray].regionFinished)
            {
                // add threads to the "fileDownloadWorkerList"
                foreach (var r in dataArray[parameters.indexOfDataArray].threadList)
                {
                    r.threadAdvisedToFinish = false;
                    r.waitToFillBuffer = true;
                }

                if (dataArray[parameters.indexOfDataArray].clans.Count() > 0)
                {
                    // for (int i = 0; i < parameters.threadCorrection; i++)
                // {
                    //BackgroundWorker is event-driven. We use events to control what happens
                    //during and after calculations.
                    //First, we need to set up the different events.
                    BackgroundWorker b = new BackgroundWorker();
                    b.DoWork += fileDownloadWorker_DoWork;
                    b.ProgressChanged += fileDownloadWorker_ProgressChanged;
                    b.WorkerReportsProgress = true;
                    b.RunWorkerCompleted += fileDownloadWorker_RunWorkerCompleted;
                    threadData d = new threadData();
                    d.fileDownloadWorker = b;
                    d.threadAdvisedToFinish = false;
                    d.waitToFillBuffer = true;
                    // seach an unused ThreadID at the threadList
                    var usedThreadIDs = new List<int>() { };
                    foreach (var r in dataArray[parameters.indexOfDataArray].threadList)
                    {
                        usedThreadIDs.Add(r.fileDownloadWorkerThreadID);
                    }
                    int unusedThreadID = Enumerable.Range(0, Settings.viaUiThreadsAllowed).Except(usedThreadIDs).FirstOrDefault();
                    d.fileDownloadWorkerThreadID = unusedThreadID;
                    dataArray[parameters.indexOfDataArray].threadList.Add(d);
                    int newListElementIndex = dataArray[parameters.indexOfDataArray].threadList.Count-1;

                    EventArgsParameter pushParameters = new EventArgsParameter();
                    pushParameters.indexOfDataArray = parameters.indexOfDataArray;
                    pushParameters.region = parameters.region;
                    pushParameters.threadID = unusedThreadID;
                    //Then, we set the Worker off. 
                    //This triggers the DoWork event.
                    //Notice the word Async - it means that Worker gets its own thread,
                    //and the main thread will carry on with its own calculations separately.
                    //We can pass any data that the worker needs as a parameter.
                    dataArray[parameters.indexOfDataArray].threadList[newListElementIndex].fileDownloadWorker.RunWorkerAsync(pushParameters);
                }
            }
            else if (dataArray[parameters.indexOfDataArray].regionFinished)
            {

            }
            else
            {
                // what is wrong with you?
                Utils.appendLog(string.Format("Error: called SetfileDownloadWorker with parameters:\nregion: {0}\nindexOfDataArray: {1}\nthread: {2}\nthreadCorrection: {3}", parameters.region, parameters.indexOfDataArray, parameters.thread, parameters.threadCorrection));
            }
            // Utils.appendLog("SetFileDownloadWorker finished");
        }

        private void downloadThreadHandler_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // *****************************
            // currently not used
            // *****************************
        }

        private void downloadThreadHandler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //This method is optional but very useful. 
            //It is called once Worker_DoWork has finished.
            Utils.appendLog("downloadThreadHandler_RunWorkerCompleted");
            downloadThreadHandler.DoWork -= downloadThreadHandler_DoWork;
            if (e.Cancelled)
            {
                // EventArgsParameter parameters = (EventArgsParameter)e.Result;
                Utils.appendLog("downloadThreadHandler was stopped by cancel: " + ((EventArgsParameter)e.Result).region + " threadID: " + ((EventArgsParameter)e.Result).threadID);
            }
            else if (e.Error != null)
            {
                Utils.appendLog(string.Format("Error: downloadThreadHandler was not stopped regularly\n{0}", e.Error.ToString()));
            }
            else
            {
                Utils.appendLog("downloadThreadHandler finished his regular job");
            }
        }

        private void fileDownloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            //DoWork is the most important event. It is where the actual calculations are done.
            EventArgsParameter parameters = (EventArgsParameter)e.Argument;
            // push this informations to RunWorkerComplete
            e.Result = parameters;

            Utils.appendLog("fileDownloadWorker_DoWork started: "+parameters.region+" threadID: "+parameters.threadID);
            while (dataArray[parameters.indexOfDataArray].threadList[dataArray[parameters.indexOfDataArray].threadList.FindIndex(r => r.fileDownloadWorkerThreadID == parameters.threadID)].waitToFillBuffer)
            {
                Thread.Sleep(300);
                // Utils.appendLog("fileDownloadWorker_DoWork sleeping 300: " + parameters.region + " threadID: " + parameters.threadID);
            }

            // Utils.appendLog("fileDownloadWorker_DoWork sleeping finished: " + parameters.region + " threadID: " + parameters.threadID);
            try
            {
                while (dataArray[parameters.indexOfDataArray].threadList[dataArray[parameters.indexOfDataArray].threadList.FindIndex(r => r.fileDownloadWorkerThreadID == parameters.threadID)].clansToProcessBuffer.Count > 0)
                {
                    clanData r = dataArray[parameters.indexOfDataArray].threadList[dataArray[parameters.indexOfDataArray].threadList.FindIndex(w => w.fileDownloadWorkerThreadID == parameters.threadID)].clansToProcessBuffer[0];
                    if (!Settings.prohibitedFilenames.Contains(r.tag))
                    {

                        AwesomeWebClient client = new AwesomeWebClient();
                        //download image
                        string filename = Path.Combine(Settings.baseStorageFolder, @"" + string.Format(dataArray[parameters.indexOfDataArray].data.storagePath + @"{0}.png", r.tag));
                        // parameters.pictureBoxFileLink = filename;
                        // e.Result = parameters;
                        client.DownloadFile(r.emblems, filename);
                    }
                    else
                    {
                        Utils.appendLog(string.Format("Warning: found prohibited filename: {0} region: {1} threadID: {2}",r.tag,parameters.region,parameters.threadID));
                    }
                    dataArray[parameters.indexOfDataArray].threadList[dataArray[parameters.indexOfDataArray].threadList.FindIndex(d => d.fileDownloadWorkerThreadID == parameters.threadID)].clansToProcessBuffer.RemoveAt(0);
                    //Now that the image is saved, we can update the Worker's progress.
                    //We do this by going back to the Worker with a cast
                    dataArray[parameters.indexOfDataArray].threadList[dataArray[parameters.indexOfDataArray].threadList.FindIndex(d => d.fileDownloadWorkerThreadID == parameters.threadID)].fileDownloadWorker.ReportProgress(1, parameters);
                    dataArray[parameters.indexOfDataArray].data.countIconDownload += 1;
                }
            }
            catch (Exception ee)
            {
                Utils.exceptionLog(ee);
            }
            dataArray[parameters.indexOfDataArray].threadList[dataArray[parameters.indexOfDataArray].threadList.FindIndex(d => d.fileDownloadWorkerThreadID == parameters.threadID)].threadFinished = true;
            //When finished, the thread will close itself. We don't need to close or stop the thread ourselves.  
            Utils.appendLog("fileDownloadWorker_DoWork finished: " + parameters.region + " threadID: " + parameters.threadID);
        }

        private void fileDownloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            EventArgsParameter parameters = (EventArgsParameter)e.UserState;
            dataArray[parameters.indexOfDataArray].data.progressBar.Value = dataArray[parameters.indexOfDataArray].data.countIconDownload;
            dataArray[parameters.indexOfDataArray].data.downloadCounter_Label.Text = string.Format("{0}/{1}", dataArray[parameters.indexOfDataArray].data.countIconDownload, dataArray[parameters.indexOfDataArray].data.total);
            //This method is called whenever we call ReportProgress()
            //Note that progress is not calculated automatically. 
            //We need to calculate the progress ourselves inside Worker_DoWork.
            //This method is optional.
        }

        private void fileDownloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // EventArgsParameter parameters = (EventArgsParameter)e.Result;
                Utils.appendLog("fileDownloadWorker was stopped by cancel: " + ((EventArgsParameter)e.Result).region + " threadID: " + ((EventArgsParameter)e.Result).threadID);
            }
            else if (e.Error != null)
            {
                Utils.appendLog(string.Format("Error: fileDownloadWorker was not stopped regularly\n{0}",e.Error.ToString()));
            }
            else
            {
                EventArgsParameter parameters = (EventArgsParameter)e.Result;
                if (parameters != null)
                {
                    //This method is optional but very useful. 
                    //It is called once Worker_DoWork has finished.
                    Utils.appendLog("fileDownloadWorker_RunWorkerCompleted finished: " + parameters.region + " threadID: " + parameters.threadID);
                    dataArray[parameters.indexOfDataArray].threadList[dataArray[parameters.indexOfDataArray].threadList.FindIndex(d => d.fileDownloadWorkerThreadID == parameters.threadID)].fileDownloadWorker.DoWork -= fileDownloadWorker_DoWork;
                }
                else
                {
                    Utils.appendLog("Error: fileDownloadWorker_RunWorkerCompleted was finished without parameters");
                }
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
            d.data.progressPage_Label = progressPage_Label1;
            d.data.showThreadCount_Label = showThreadCount_Label1;
            d.data.downloadCounter_Label = downloadCounter_Label1;
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
            d.data.progressPage_Label = progressPage_Label3;
            d.data.showThreadCount_Label = showThreadCount_Label3;
            d.data.downloadCounter_Label = downloadCounter_Label3;
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
            d.data.progressPage_Label = progressPage_Label2;
            d.data.showThreadCount_Label = showThreadCount_Label2;
            d.data.downloadCounter_Label = downloadCounter_Label2;
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
            d.data.progressPage_Label = progressPage_Label4;
            d.data.showThreadCount_Label = showThreadCount_Label4;
            d.data.downloadCounter_Label = downloadCounter_Label4;
            dataArray.Add(d);
        }

        private void threads_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Settings.viaUiThreadsAllowed = (int)threads_numericUpDown.Value;
            Utils.appendLog("viaUiThreadsAllowed set to: " + Settings.viaUiThreadsAllowed);
        }

        private void dump_button_Click(object sender, EventArgs e)
        {
            Utils.appendLog("Clans: " + dataArray[0].clans.ToString());
            Utils.appendLog("countApiRequest: " + dataArray[0].data.countApiRequest);
            Utils.appendLog("countIconDownload: " + dataArray[0].data.countIconDownload);
            Utils.appendLog("currentPage: " + dataArray[0].data.currentPage);
            Utils.appendLog("total: " + dataArray[0].data.total);
            Utils.appendLog("initialized: " + dataArray[0].initialized.ToString());
            Utils.appendLog("region: " + dataArray[0].region);
            Utils.appendLog("regionFinished: " + dataArray[0].regionFinished.ToString());
            int i = 0;
            foreach (var t in dataArray[0].threadList)
            {
                Utils.appendLog("---------- thread " + i + " ----------");
                Utils.appendLog("clansToProcessBuffer: " + t.clansToProcessBuffer.ToString());
                Utils.appendLog("threadAdvisedToFinish: " + t.threadAdvisedToFinish.ToString());
                Utils.appendLog("threadFinished: " + t.threadFinished.ToString());
                Utils.appendLog("waitToFillBuffer: " + t.waitToFillBuffer.ToString());
                Utils.appendLog("fileDownloadWorkerThreadID: " + t.fileDownloadWorkerThreadID);
            }
            Utils.appendLog("----------------------------------------");
        }
    }
}
 
 