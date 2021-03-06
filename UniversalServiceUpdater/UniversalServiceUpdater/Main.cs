﻿using Moonbyte.UniversalClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace IndieGoat.Net.Updater
{
    public class UniversalServiceUpdater
    {

        #region Vars

        string ApplicationVersion;
        string ApplicationName;
        string _updateURLlocation = "";

        UniversalClient UpdateServer;

        string UniversalApiDirectory = @"C:\IndieGoat\UniversalAPI\";
        string UniversalApplicationName = @"UniversalServiceUpdater.exe";

        #endregion

        #region Required / Startup

        /// <summary>
        /// Set the update url location, if the value is given. If not
        /// you have to set the update url location in UpdateUrlLocation() property.
        /// </summary>
        public UniversalServiceUpdater(string URLLocation = null)
        {
            //Sets the update url
            _updateURLlocation = URLLocation;

            //Checks if Universal API is installed
            if (!CheckUniversalAPI()) { DownloadUniversalAPI(); }
        }

        /// <summary>
        /// Download's the file from this URL
        /// </summary>
        public string UpdateUrlLocation
        {
            get { return _updateURLlocation; }
            set { _updateURLlocation = value; }
        }

        #endregion

        #region UniversalAPI

        /// <summary>
        /// Check if universal API is installed, if not installs Universal API
        /// </summary>
        public bool CheckUniversalAPI()
        {
            //Checks if the file exist's
            if (File.Exists(UniversalApiDirectory + UniversalApplicationName))
            { return true; } else { return false; }
        }

        /// <summary>
        /// Download's the universal API
        /// </summary>
        private void DownloadUniversalAPI()
        {
            //Creates a new list to download a file from a url
            List<string> DownloadURLs = new List<string>();
            DownloadURLs.Add("https://dl.dropbox.com/s/7c4z94rzcm73qb6/UniversalServiceUpdater.exe?dl=0"); //Adds UniversalServiceUpdater application
            DownloadURLs.Add("https://dl.dropbox.com/s/mvak9zh5rbf5d0o/DotNetZip.dll?dl=0"); //Adds DotNetZip Lib
            List<string> DownloadNames = new List<string>();
            DownloadNames.Add("UniversalServiceUpdater.exe");
            DownloadNames.Add("DotNetZip.dll");

            int index = 0;

            //Download's the file per string
            foreach (string url in DownloadURLs)
            {
                string appName = DownloadNames[index];
                index++;
                if (!Directory.Exists(UniversalApiDirectory)) Directory.CreateDirectory(UniversalApiDirectory);
                using (WebClient client = new WebClient()) { client.DownloadFile(url, UniversalApiDirectory + appName); }
            }
        }

        #endregion

        #region Check

        public void CheckUpdate(string ServerIP, int ServerPort)
        {
            //Checks if UniversalAPI is installed.
            if (!CheckUniversalAPI())
            {
                Console.WriteLine("Universal API is currently not installed!");
            }

            //Set private vars
            ApplicationVersion = Application.ProductVersion;
            ApplicationName = Application.ProductName.Replace(" ", "_");

            //Initialize the update server
            UpdateServer = new UniversalClient(false);

            //Connect to the vortex studio update server
            UpdateServer.ConnectToRemoteServer(ServerIP, ServerPort);
            
            //Check if the project exist's
            string ProjectCheck = UpdateServer.SendCommand("Dyn", new string[] { "CHECKPROJECTNAME", ApplicationName });

            if (ProjectCheck == "CHECKPROJECT_FALSE")
            {
                string CreateProject = UpdateServer.SendCommand("Dyn", new string[] { "ADDPROJECT", ApplicationName, ApplicationVersion, "test:test", "braydelritter@gmail.com" });
            }

            //Get the application version
            string ServerVersion = UpdateServer.SendCommand("Dyn", new string[] { "GETVERSION", ApplicationName });

            if (ServerVersion == ApplicationVersion)
            {
                //Does nothing
            }
            else
            {
                //Continues with the update

                //Sets the quote string
                const string quote = "\"";

                //Starts the process
                Process.Start(UniversalApiDirectory + UniversalApplicationName, quote + Application.ProductName + quote + " " + quote + Application.StartupPath + quote + " " + quote + _updateURLlocation + quote);
            }
        }

        #endregion

    }
}
