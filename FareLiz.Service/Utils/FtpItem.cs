namespace SkyDean.FareLiz.Service.Utils
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>The ftp item.</summary>
    public class FtpItem
    {
        /// <summary>The directory entry type.</summary>
        public enum DirectoryEntryType
        {
            /// <summary>The file.</summary>
            File, 

            /// <summary>The directory.</summary>
            Directory
        }

        #region Regular expressions for parsing List results

        /// <summary>
        /// List of REGEX formats for different FTP server listing formats The first three are various UNIX/LINUX formats, fourth is for MS FTP in
        /// detailed mode and the last for MS FTP in 'DOS' mode.
        /// </summary>
        // These regular expressions will be used to match a directory/file 
        // listing as explained at the top of this class.
        internal string[] _ParseFormats =
            {
                @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)", 
                @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\d+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)", 
                @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\d+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{1,2}:\d{2})\s+(?<name>.+)", 
                @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{1,2}:\d{2})\s+(?<name>.+)", 
                @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})(\s+)(?<size>(\d+))(\s+)(?<ctbit>(\w+\s\w+))(\s+)(?<size2>(\d+))\s+(?<timestamp>\w+\s+\d+\s+\d{2}:\d{2})\s+(?<name>.+)", 
                @"(?<timestamp>\d{2}\-\d{2}\-\d{2}\s+\d{2}:\d{2}[Aa|Pp][mM])\s+(?<dir>\<\w+\>){0,1}(?<size>\d+){0,1}\s+(?<name>.+)", 
                @"([<timestamp>]*\d{2}\-\d{2}\-\d{2}\s+\d{2}:\d{2}[Aa|Pp][mM])\s+([<dir>]*\<\w+\>){0,1}([<size>]*\d+){0,1}\s+([<name>]*.+)"
            };

        #endregion

        #region Private Functions

        /// <summary>
        /// Depending on the various directory listing formats, the current listing will be matched against the set of available matches.
        /// </summary>
        /// <param name="line">
        /// </param>
        /// <returns>
        /// The <see cref="Match"/>.
        /// </returns>
        private Match GetMatchingRegex(string line)
        {
            Match match;
            int counter;
            for (counter = 0; counter < this._ParseFormats.Length - 1; counter++)
            {
                match = Regex.Match(line, this._ParseFormats[counter], RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match;
                }
            }

            return null;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// The method accepts a directory listing and initialises all the attributes of a file.
        /// </summary>
        /// <param name="line">
        /// Directory Listing line returned by the DetailedDirectoryList method
        /// </param>
        /// <param name="path">
        /// The path of the Directory
        /// </param>
        public void GetFtpFileInfo(string line, string path)
        {
            string directory;
            var match = this.GetMatchingRegex(line); // Get the match of the current listing.
            if (match == null)
            {
                throw new Exception("Unable to parse the line " + line);
            }

            this._fileName = match.Groups["name"].Value; // Set the name of the file/directory.
            this._path = path; // Set the path from which the listing needs to be obtained.
            this._permissions = match.Groups["permission"].Value; // Set the permissions available for the listing
            directory = match.Groups["dir"].Value;

            // Set the filetype to either Directory or File basing on the listing.
            if (!string.IsNullOrEmpty(directory) && directory != "-")
            {
                this._fileType = DirectoryEntryType.Directory;
            }
            else
            {
                this._fileType = DirectoryEntryType.File;
                this._size = long.Parse(match.Groups["size"].Value, this.culture);
            }

            try
            {
                this._fileDateTime = DateTime.Parse(match.Groups["timestamp"].Value, this.culture);

                // Set the datetime of the listing.
            }
            catch
            {
                this._fileDateTime = DateTime.Now;
            }

            // Initialize the readonly properties.
            this.FileName = this._fileName;
            this.Path = this._path;
            this.FileType = this._fileType;
            this.FullName = this.Path + this.FileName;
            this.Size = this._size;
            this.FileDateTime = this._fileDateTime;
            this.Permissions = this._permissions;
        }

        #endregion

        #region Private Members

        /// <summary>The _file name.</summary>
        private string _fileName = string.Empty; // Represents the filename without extension

        /// <summary>The _file extension.</summary>
        private string _fileExtension = string.Empty; // Represents the file extension

        /// <summary>The _path.</summary>
        private string _path = string.Empty; // Represents the complete path

        /// <summary>The _file type.</summary>
        private DirectoryEntryType _fileType; // Represents if the current listing represents a file/directory.

        /// <summary>The _size.</summary>
        private long _size; // Represents the size. 

        /// <summary>The _file date time.</summary>
        private DateTime _fileDateTime; // DateTime of file/Directory 

        /// <summary>The _permissions.</summary>
        private string _permissions; // Permissions on the directory 

        /// <summary>The culture.</summary>
        private readonly IFormatProvider culture = System.Globalization.CultureInfo.InvariantCulture;

        // Eliminate DateTime parsing issues.
        #endregion

        #region Public Properties

        /// <summary>Gets or sets the file name.</summary>
        public string FileName
        {
            get
            {
                return this._fileName;
            }

            set
            {
                this._fileName = value;

                // Set the FileExtension.
                if (this._fileName.LastIndexOf(".") > -1)
                {
                    this.FileExtension = this._fileName.Substring(this._fileName.LastIndexOf(".") + 1);
                }
            }
        }

        /// <summary>Gets or sets the file extension.</summary>
        public string FileExtension { get; set; }

        /// <summary>Gets or sets the full name.</summary>
        public string FullName { get; set; }

        /// <summary>Gets or sets the path.</summary>
        public string Path { get; set; }

        /// <summary>The file type.</summary>
        internal DirectoryEntryType FileType;

        /// <summary>Gets or sets the size.</summary>
        public long Size { get; set; }

        /// <summary>Gets or sets the file date time.</summary>
        public DateTime FileDateTime { get; set; }

        /// <summary>Gets or sets the permissions.</summary>
        public string Permissions { get; set; }

        /// <summary>Gets the name only.</summary>
        public string NameOnly
        {
            get
            {
                var i = this.FileName.LastIndexOf(".");
                if (i > 0)
                {
                    return this.FileName.Substring(0, i);
                }

                return this.FileName;
            }
        }

        #endregion
    }
}