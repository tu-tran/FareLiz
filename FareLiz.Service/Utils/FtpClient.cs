using log4net;
using SkyDean.FareLiz.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace SkyDean.FareLiz.Service.Utils
{
    /// <summary>
    /// Provides easy access to common operations against FTP servers
    /// </summary>    
    public class FtpClient
    {
        public event FtpConnectionEventHandler MessageReceived;

        protected FtpClient()
        {
            UseBinaryMode = true;
            TimeOut = Timeout.Infinite;
        }

        public FtpClient(ILog logger, string host, int port, string proxyHost, int proxyPort, string userName, string password)
            : this()
        {
            Logger = logger;
            Host = host;
            Port = port;
            ProxyHost = proxyHost;
            ProxyPort = proxyPort;
            UserName = userName;
            Password = password;
        }

        public ILog Logger { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public int TimeOut { get; set; }
        public string LocalDirectory { get; set; }
        public bool UseBinaryMode { get; set; }

        /// <param name="fileName">The name of the file to upload</param>
        /// <param name="remoteFileName">The name of the destination file</param>        
        public void Upload(string fileName, string remoteFileName)
        {
            //Mark the beginning of the transfer
            DateTime startTime = DateTime.Now;

            //Check to see if we have a relative or absolute path to the file to transfer
            fileName = Path.GetFullPath(fileName);

            string remotePath = Host + "/" + remoteFileName;
            OnMessageReceived(string.Format("Uploading {0} to {1}", fileName, remotePath));

            //Create a request for the upload
            FtpWebRequest ftpWebRequest = CreateWebRequest(WebRequestMethods.Ftp.UploadFile, remotePath);

            var uri = ftpWebRequest.RequestUri;
            string curDir = "";
            for (int i = 0; i < uri.Segments.Length - 1; i++)
            {
                var segment = uri.Segments[i];
                if (segment.Length > 1)
                {
                    curDir += "/" + segment.Trim('/');
                    MakeDirectory(curDir);
                }
            }

            // Try deleting existing file first
            try
            {
                Logger.InfoFormat("Removing existing file if any [{0}]", remoteFileName);
                DeleteFile(remoteFileName);
            }
            catch { }

            Logger.InfoFormat("Uploading file [{0}] - {1}", remoteFileName, StringUtil.FormatSize(new FileInfo(fileName).Length));
            //Get a reference to the upload stream
            using (var uploadStream = ftpWebRequest.GetRequestStream())
            {
                int bytesRead = 0;
                int bytesTotalWritten = 0;

                //Create a filestream for the local file to upload
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[1024];
                    //Start uploading
                    while (true)
                    {
                        bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;
                        uploadStream.Write(buffer, 0, bytesRead);
                        bytesTotalWritten += bytesRead;
                    }

                    //Close the upload stream
                    uploadStream.Close();
                    uploadStream.Dispose();
                }

                //Get the respone from the server
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                OnMessageReceived(string.Format("Status: {0} {1}", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
                //Report the number of bytes transferred
                OnMessageReceived(string.Format("Transferred {0} bytes in {1} seconds", bytesTotalWritten.ToString(), DateTime.Now.Subtract(startTime).Seconds));
            }
        }

        /// <param name="filter">The filter to apply when listing files (ex *.txt)</param>        
        public List<string> ListDirectory(string filter)
        {
            List<string> files = new List<string>();
            string line = "";
            string remotePath = Host + "/" + filter;

            try
            {
                //Create a request for directory listing
                FtpWebRequest ftpWebRequest = CreateWebRequest(WebRequestMethods.Ftp.ListDirectory, remotePath);

                //Get a reference to the response stream
                using (var ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse())
                {
                    using (var responseStream = new StreamReader(ftpWebResponse.GetResponseStream()))
                    {
                        while ((line = responseStream.ReadLine()) != null)
                        {
                            files.Add(line);
                        }
                        OnMessageReceived(string.Format("Status: {0} {1} (FTP NLIST)", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
                    }
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    HandleException(ex);
                    throw;
                }
                return null;
            }

            //Return the list of files
            return files;
        }

        /// <summary>
        /// Deletes a file from the FTP server
        /// </summary>
        /// <param name="fileName">The name of the file to delete</param>
        public void DeleteFile(string fileName)
        {
            string remotePath = Host + "/" + fileName;
            try
            {
                Logger.InfoFormat("Removing file [{0}]", remotePath);
                FtpWebRequest ftpWebRequest = CreateWebRequest(WebRequestMethods.Ftp.DeleteFile, remotePath);
                //get the response from the server
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                OnMessageReceived(string.Format("Status: {0} {1}", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
                OnMessageReceived(string.Format("Deleted file {0}", remotePath));
            }
            catch (WebException ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a timestamp indicating what time the remote file was modified
        /// </summary>
        /// <param name="fileName">The name of the remote file</param>
        /// <returns>The date and time for when the file was last modified</returns> 
        public DateTime GetFileTimeStamp(string fileName)
        {
            string remotePath = Host + "/" + fileName;
            DateTime lastModified = DateTime.MinValue;
            try
            {

                FtpWebRequest ftpWebRequest = CreateWebRequest(WebRequestMethods.Ftp.GetDateTimestamp, remotePath);
                //get the response from the server
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                lastModified = ftpWebResponse.LastModified;
                OnMessageReceived(string.Format("Status: {0} {1}", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
            }
            catch (WebException ex)
            {
                HandleException(ex);
                throw;
            }
            return lastModified;
        }

        /// <summary>
        /// Creates a new directory on the FTP Server
        /// </summary>
        /// <param name="directoryName">The name of the directory to create</param>
        public void MakeDirectory(string directoryName)
        {
            string remotePath = Host + "/" + directoryName.Trim('/');
            try
            {
                Logger.InfoFormat("Creating dir [{0}]", remotePath);
                FtpWebRequest ftpWebRequest = CreateWebRequest(WebRequestMethods.Ftp.MakeDirectory, remotePath);
                //get the response from the server
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                OnMessageReceived(string.Format("Status: {0} {1}", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    HandleException(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete a new directory on the FTP Server
        /// </summary>
        /// <param name="directoryName">The name of the directory to create</param>
        public void RemoveDirectory(string directoryName)
        {
            try
            {
                FtpItem ftpItem = new FtpItem();
                var dirList = new List<string>(); //Create a collection to hold list of directories within the root and including the root directory.
                dirList.Add(directoryName); //Add the root folder to the collection

                for (int i = 0; i < dirList.Count; i++)
                {
                    string currentDir = dirList[i].ToString() + "/";
                    string[] dirContents = GetDirectoryList(currentDir);
                    for (int c = 0; c < dirContents.Length; c++)
                    {
                        string curContent = dirContents[c];
                        if (curContent.Length < 1)
                            break;

                        curContent = curContent.Replace("\r", "");
                        ftpItem.GetFtpFileInfo(curContent, currentDir);
                        if (ftpItem.FileType == FtpItem.DirectoryEntryType.Directory)
                            dirList.Add(ftpItem.FullName); //If Directory add to the collection.
                        else if (ftpItem.FileType == FtpItem.DirectoryEntryType.File)
                            DeleteFile(ftpItem.FullName); //If file,then delete.
                    }
                }

                // Remove the directories in the collection from bottom toward top.
                // This would ensure that all the sub directories were deleted first before deleting the root folder.
                for (int count = dirList.Count; count > 0; count--)
                {
                    var dirToDelete = dirList[count - 1];
                    string remotePath = Host + "/" + dirToDelete;
                    Logger.InfoFormat("Removing dir [{0}]", remotePath);
                    FtpWebRequest ftpWebRequest = CreateWebRequest(WebRequestMethods.Ftp.RemoveDirectory, remotePath);
                    //get the response from the server
                    FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                    OnMessageReceived(string.Format("Status: {0} {1}", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
                    OnMessageReceived(string.Format("Deleted directory {0}", dirToDelete));
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                    HandleException(ex);
            }
        }

        //Gets the directory listing given the path
        public string[] GetDirectoryList(string directoryName)
        {
            string[] result = null;
            string remotePath = Host + "/" + directoryName.Trim('/');
            FtpWebRequest ftpReq = CreateWebRequest(WebRequestMethods.Ftp.ListDirectoryDetails, remotePath); //Create the request
            FtpWebResponse ftpResp = (FtpWebResponse)ftpReq.GetResponse();//Fire the command
            Stream ftpResponseStream = ftpResp.GetResponseStream(); //Get the output 
            StreamReader reader = new StreamReader(ftpResponseStream, System.Text.Encoding.UTF8);//Encode the output to UTF8 format
            result = (reader.ReadToEnd().Split('\n')); //Split the output for newline characters. 
            ftpResp.Close(); //Close the response object.
            return result; // return the output 
        }

        private FtpWebRequest CreateWebRequest(string method, string uri)
        {
            UriBuilder uriBuilder = new UriBuilder(uri);
            uriBuilder.Port = Port;
            uriBuilder.Scheme = "ftp";
            FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(uriBuilder.Uri);
            ftpWebRequest.Method = method;
            ftpWebRequest.UseBinary = UseBinaryMode;
            ftpWebRequest.KeepAlive = false;
            ftpWebRequest.Timeout = TimeOut;
            ftpWebRequest.Credentials = new NetworkCredential(UserName, Password);

            if (!String.IsNullOrEmpty(ProxyHost))
            {
                ftpWebRequest.UsePassive = true;
                ftpWebRequest.Proxy = new WebProxy(ProxyHost, ProxyPort) { Credentials = CredentialCache.DefaultCredentials };
            }

            return ftpWebRequest;
        }

        private void OnMessageReceived(string message)
        {
            if (MessageReceived != null)
                MessageReceived(this, new FtpConnectionEventArgs(message.Replace(Environment.NewLine, string.Empty)));
        }

        private void HandleException(WebException ex)
        {
            OnMessageReceived(string.Format("Status: {0} {1}", (ex.Response as FtpWebResponse).StatusCode, (ex.Response as FtpWebResponse).StatusDescription));
        }
    }
}
