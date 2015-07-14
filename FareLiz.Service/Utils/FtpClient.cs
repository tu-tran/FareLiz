namespace SkyDean.FareLiz.Service.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>Provides easy access to common operations against FTP servers</summary>
    public class FtpClient
    {
        /// <summary>Initializes a new instance of the <see cref="FtpClient" /> class.</summary>
        protected FtpClient()
        {
            this.UseBinaryMode = true;
            this.TimeOut = Timeout.Infinite;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpClient"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="host">
        /// The host.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <param name="proxyHost">
        /// The proxy host.
        /// </param>
        /// <param name="proxyPort">
        /// The proxy port.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        public FtpClient(ILogger logger, string host, int port, string proxyHost, int proxyPort, string userName, string password)
            : this()
        {
            this.Logger = logger;
            this.Host = host;
            this.Port = port;
            this.ProxyHost = proxyHost;
            this.ProxyPort = proxyPort;
            this.UserName = userName;
            this.Password = password;
        }

        /// <summary>Gets or sets the logger.</summary>
        public ILogger Logger { get; set; }

        /// <summary>Gets or sets the host.</summary>
        public string Host { get; set; }

        /// <summary>Gets or sets the port.</summary>
        public int Port { get; set; }

        /// <summary>Gets or sets the proxy host.</summary>
        public string ProxyHost { get; set; }

        /// <summary>Gets or sets the proxy port.</summary>
        public int ProxyPort { get; set; }

        /// <summary>Gets or sets the user name.</summary>
        public string UserName { get; set; }

        /// <summary>Gets or sets the password.</summary>
        public string Password { get; set; }

        /// <summary>Gets or sets the time out.</summary>
        public int TimeOut { get; set; }

        /// <summary>Gets or sets the local directory.</summary>
        public string LocalDirectory { get; set; }

        /// <summary>Gets or sets a value indicating whether use binary mode.</summary>
        public bool UseBinaryMode { get; set; }

        /// <summary>The message received.</summary>
        public event FtpConnectionEventHandler MessageReceived;

        /// <summary>
        /// The upload.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to upload
        /// </param>
        /// <param name="remoteFileName">
        /// The name of the destination file
        /// </param>
        public void Upload(string fileName, string remoteFileName)
        {
            // Mark the beginning of the transfer
            var startTime = DateTime.Now;

            // Check to see if we have a relative or absolute path to the file to transfer
            fileName = Path.GetFullPath(fileName);

            var remotePath = this.Host + "/" + remoteFileName;
            this.OnMessageReceived(string.Format("Uploading {0} to {1}", fileName, remotePath));

            // Create a request for the upload
            var ftpWebRequest = this.CreateWebRequest(WebRequestMethods.Ftp.UploadFile, remotePath);

            var uri = ftpWebRequest.RequestUri;
            var curDir = string.Empty;
            for (var i = 0; i < uri.Segments.Length - 1; i++)
            {
                var segment = uri.Segments[i];
                if (segment.Length > 1)
                {
                    curDir += "/" + segment.Trim('/');
                    this.MakeDirectory(curDir);
                }
            }

            // Try deleting existing file first
            try
            {
                this.Logger.Info("Removing existing file if any [{0}]", remoteFileName);
                this.DeleteFile(remoteFileName);
            }
            catch
            {
            }

            this.Logger.Info("Uploading file [{0}] - {1}", remoteFileName, StringUtil.FormatSize(new FileInfo(fileName).Length));

            // Get a reference to the upload stream
            using (var uploadStream = ftpWebRequest.GetRequestStream())
            {
                var bytesRead = 0;
                var bytesTotalWritten = 0;

                // Create a filestream for the local file to upload
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[1024];

                    // Start uploading
                    while (true)
                    {
                        bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        uploadStream.Write(buffer, 0, bytesRead);
                        bytesTotalWritten += bytesRead;
                    }

                    // Close the upload stream
                    uploadStream.Close();
                    uploadStream.Dispose();
                }

                // Get the respone from the server
                var ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                this.OnMessageReceived(string.Format("Status: {0} {1}", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));

                // Report the number of bytes transferred
                this.OnMessageReceived(
                    string.Format("Transferred {0} bytes in {1} seconds", bytesTotalWritten, DateTime.Now.Subtract(startTime).Seconds));
            }
        }

        /// <summary>
        /// The list directory.
        /// </summary>
        /// <param name="filter">
        /// The filter to apply when listing files (ex *.txt)
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> ListDirectory(string filter)
        {
            var files = new List<string>();
            var line = string.Empty;
            var remotePath = this.Host + "/" + filter;

            try
            {
                // Create a request for directory listing
                var ftpWebRequest = this.CreateWebRequest(WebRequestMethods.Ftp.ListDirectory, remotePath);

                // Get a reference to the response stream
                using (var ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse())
                {
                    using (var responseStream = new StreamReader(ftpWebResponse.GetResponseStream()))
                    {
                        while ((line = responseStream.ReadLine()) != null)
                        {
                            files.Add(line);
                        }

                        this.OnMessageReceived(
                            string.Format("Status: {0} {1} (FTP NLIST)", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
                    }
                }
            }
            catch (WebException ex)
            {
                var response = (FtpWebResponse)ex.Response;
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    this.HandleException(ex);
                    throw;
                }

                return null;
            }

            // Return the list of files
            return files;
        }

        /// <summary>
        /// Deletes a file from the FTP server
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to delete
        /// </param>
        public void DeleteFile(string fileName)
        {
            var remotePath = this.Host + "/" + fileName;
            try
            {
                this.Logger.Info("Removing file [{0}]", remotePath);
                var ftpWebRequest = this.CreateWebRequest(WebRequestMethods.Ftp.DeleteFile, remotePath);

                // get the response from the server
                var ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                this.OnMessageReceived(string.Format("Status: {0} {1}", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
                this.OnMessageReceived(string.Format("Deleted file {0}", remotePath));
            }
            catch (WebException ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a timestamp indicating what time the remote file was modified
        /// </summary>
        /// <param name="fileName">
        /// The name of the remote file
        /// </param>
        /// <returns>
        /// The date and time for when the file was last modified
        /// </returns>
        public DateTime GetFileTimeStamp(string fileName)
        {
            var remotePath = this.Host + "/" + fileName;
            var lastModified = DateTime.MinValue;
            try
            {
                var ftpWebRequest = this.CreateWebRequest(WebRequestMethods.Ftp.GetDateTimestamp, remotePath);

                // get the response from the server
                var ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                lastModified = ftpWebResponse.LastModified;
                this.OnMessageReceived(string.Format("Status: {0} {1}", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
            }
            catch (WebException ex)
            {
                this.HandleException(ex);
                throw;
            }

            return lastModified;
        }

        /// <summary>
        /// Creates a new directory on the FTP Server
        /// </summary>
        /// <param name="directoryName">
        /// The name of the directory to create
        /// </param>
        public void MakeDirectory(string directoryName)
        {
            var remotePath = this.Host + "/" + directoryName.Trim('/');
            try
            {
                this.Logger.Info("Creating dir [{0}]", remotePath);
                var ftpWebRequest = this.CreateWebRequest(WebRequestMethods.Ftp.MakeDirectory, remotePath);

                // get the response from the server
                var ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                this.OnMessageReceived(string.Format("Status: {0} {1}", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
            }
            catch (WebException ex)
            {
                var response = (FtpWebResponse)ex.Response;
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    this.HandleException(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete a new directory on the FTP Server
        /// </summary>
        /// <param name="directoryName">
        /// The name of the directory to create
        /// </param>
        public void RemoveDirectory(string directoryName)
        {
            try
            {
                var ftpItem = new FtpItem();
                var dirList = new List<string>();

                // Create a collection to hold list of directories within the root and including the root directory.
                dirList.Add(directoryName); // Add the root folder to the collection

                for (var i = 0; i < dirList.Count; i++)
                {
                    var currentDir = dirList[i] + "/";
                    var dirContents = this.GetDirectoryList(currentDir);
                    for (var c = 0; c < dirContents.Length; c++)
                    {
                        var curContent = dirContents[c];
                        if (curContent.Length < 1)
                        {
                            break;
                        }

                        curContent = curContent.Replace("\r", string.Empty);
                        ftpItem.GetFtpFileInfo(curContent, currentDir);
                        if (ftpItem.FileType == FtpItem.DirectoryEntryType.Directory)
                        {
                            dirList.Add(ftpItem.FullName); // If Directory add to the collection.
                        }
                        else if (ftpItem.FileType == FtpItem.DirectoryEntryType.File)
                        {
                            this.DeleteFile(ftpItem.FullName); // If file,then delete.
                        }
                    }
                }

                // Remove the directories in the collection from bottom toward top.
                // This would ensure that all the sub directories were deleted first before deleting the root folder.
                for (var count = dirList.Count; count > 0; count--)
                {
                    var dirToDelete = dirList[count - 1];
                    var remotePath = this.Host + "/" + dirToDelete;
                    this.Logger.Info("Removing dir [{0}]", remotePath);
                    var ftpWebRequest = this.CreateWebRequest(WebRequestMethods.Ftp.RemoveDirectory, remotePath);

                    // get the response from the server
                    var ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                    this.OnMessageReceived(string.Format("Status: {0} {1}", ftpWebResponse.StatusCode, ftpWebResponse.StatusDescription));
                    this.OnMessageReceived(string.Format("Deleted directory {0}", dirToDelete));
                }
            }
            catch (WebException ex)
            {
                var response = (FtpWebResponse)ex.Response;
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    this.HandleException(ex);
                }
            }
        }

        // Gets the directory listing given the path
        /// <summary>
        /// The get directory list.
        /// </summary>
        /// <param name="directoryName">
        /// The directory name.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        public string[] GetDirectoryList(string directoryName)
        {
            string[] result = null;
            var remotePath = this.Host + "/" + directoryName.Trim('/');
            var ftpReq = this.CreateWebRequest(WebRequestMethods.Ftp.ListDirectoryDetails, remotePath);

            // Create the request
            var ftpResp = (FtpWebResponse)ftpReq.GetResponse(); // Fire the command
            var ftpResponseStream = ftpResp.GetResponseStream(); // Get the output 
            var reader = new StreamReader(ftpResponseStream, System.Text.Encoding.UTF8);

            // Encode the output to UTF8 format
            result = reader.ReadToEnd().Split('\n'); // Split the output for newline characters. 
            ftpResp.Close(); // Close the response object.
            return result; // return the output 
        }

        /// <summary>
        /// The create web request.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <returns>
        /// The <see cref="FtpWebRequest"/>.
        /// </returns>
        private FtpWebRequest CreateWebRequest(string method, string uri)
        {
            var uriBuilder = new UriBuilder(uri);
            uriBuilder.Port = this.Port;
            uriBuilder.Scheme = "ftp";
            var ftpWebRequest = (FtpWebRequest)WebRequest.Create(uriBuilder.Uri);
            ftpWebRequest.Method = method;
            ftpWebRequest.UseBinary = this.UseBinaryMode;
            ftpWebRequest.KeepAlive = false;
            ftpWebRequest.Timeout = this.TimeOut;
            ftpWebRequest.Credentials = new NetworkCredential(this.UserName, this.Password);

            if (!string.IsNullOrEmpty(this.ProxyHost))
            {
                ftpWebRequest.UsePassive = true;
                ftpWebRequest.Proxy = new WebProxy(this.ProxyHost, this.ProxyPort) { Credentials = CredentialCache.DefaultCredentials };
            }

            return ftpWebRequest;
        }

        /// <summary>
        /// The on message received.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void OnMessageReceived(string message)
        {
            if (this.MessageReceived != null)
            {
                this.MessageReceived(this, new FtpConnectionEventArgs(message.Replace(Environment.NewLine, string.Empty)));
            }
        }

        /// <summary>
        /// The handle exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        private void HandleException(WebException ex)
        {
            this.OnMessageReceived(
                string.Format("Status: {0} {1}", (ex.Response as FtpWebResponse).StatusCode, (ex.Response as FtpWebResponse).StatusDescription));
        }
    }
}