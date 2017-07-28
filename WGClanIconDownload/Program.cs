﻿using System;
using System.Windows.Forms;

namespace WGClanIconDownload
{
        public static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            Utils.clearLog();
            string[] temp = CiInfo.BuildTag.Split(',');
            string BuildTag = temp[0].Trim();
            Utils.appendLog("program " + BuildTag);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new MainWindow());
            }
            catch (Exception ex)
            {
                Utils.appendLog("EXCEPTION: Program section");
                Utils.appendLog(ex.ToString());
            }
        }
    }
}
