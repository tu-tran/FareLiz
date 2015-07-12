using System;
using System.Text.RegularExpressions;

namespace SkyDean.FareLiz.Service.Utils
{
    public class FtpItem
    {
        public enum DirectoryEntryType { File, Directory }

        # region Private Members
        private string _fileName = string.Empty; // Represents the filename without extension
        private string _fileExtension = string.Empty; // Represents the file extension
        private string _path = string.Empty; // Represents the complete path
        private DirectoryEntryType _fileType; // Represents if the current listing represents a file/directory.
        private long _size; // Represents the size. 
        private DateTime _fileDateTime; // DateTime of file/Directory 
        private string _permissions; // Permissions on the directory 
        IFormatProvider culture = System.Globalization.CultureInfo.InvariantCulture; //Eliminate DateTime parsing issues.
        # endregion

        # region Public Properties
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                // Set the FileExtension.
                if (_fileName.LastIndexOf(".") > -1)
                {
                    FileExtension = _fileName.Substring(_fileName.LastIndexOf(".") + 1);
                }
            }
        }
        public string FileExtension { get; set; }
        public string FullName { get; set; }
        public string Path { get; set; }
        internal DirectoryEntryType FileType;
        public long Size { get; set; }
        public DateTime FileDateTime { get; set; }
        public string Permissions { get; set; }
        public string NameOnly
        {
            get
            {
                int i = this.FileName.LastIndexOf(".");
                if (i > 0)
                    return this.FileName.Substring(0, i);
                else
                    return this.FileName;
            }
        }

        # endregion
        # region Regular expressions for parsing List results
        /// <summary>
        /// List of REGEX formats for different FTP server listing formats
        /// The first three are various UNIX/LINUX formats, 
        /// fourth is for MS FTP in detailed mode and the last for MS FTP in 'DOS' mode.
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
        # endregion
        # region Private Functions
        /// <summary>
        /// Depending on the various directory listing formats, 
        /// the current listing will be matched against the set of available matches.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        // This method evaluates the directory/file listing by applying 
        // each of the regular expression defined by the string array, _ParseFormats and returns success on a successful match.
        private Match GetMatchingRegex(string line)
        {
            Match match;
            int counter;
            for (counter = 0; counter < _ParseFormats.Length - 1; counter++)
            {
                match = Regex.Match(line, _ParseFormats[counter], RegexOptions.IgnoreCase);
                if (match.Success)
                    return match;
            }
            return null;
        }
        # endregion
        # region Public Functions
        /// <summary>
        /// The method accepts a directory listing and initialises all the attributes of a file.
        /// </summary>
        /// <param name="line">Directory Listing line returned by the DetailedDirectoryList method</param>
        /// <param name="path">The path of the Directory</param>
        // This method populates the needful properties such as filename,path etc.
        public void GetFtpFileInfo(string line, string path)
        {
            string directory;
            Match match = GetMatchingRegex(line); //Get the match of the current listing.
            if (match == null)
                throw new Exception("Unable to parse the line " + line);
            else
            {
                _fileName = match.Groups["name"].Value; //Set the name of the file/directory.
                _path = path; // Set the path from which the listing needs to be obtained.
                _permissions = match.Groups["permission"].Value; // Set the permissions available for the listing
                directory = match.Groups["dir"].Value;
                //Set the filetype to either Directory or File basing on the listing.
                if (!string.IsNullOrEmpty(directory) && directory != "-")
                    _fileType = DirectoryEntryType.Directory;
                else
                {
                    _fileType = DirectoryEntryType.File;
                    _size = long.Parse(match.Groups["size"].Value, culture);
                }
                try
                {
                    _fileDateTime = DateTime.Parse(match.Groups["timestamp"].Value, culture); // Set the datetime of the listing.
                }
                catch
                {
                    _fileDateTime = DateTime.Now;
                }
            }
            // Initialize the readonly properties.
            FileName = _fileName;
            Path = _path;
            FileType = _fileType;
            FullName = Path + FileName;
            Size = _size;
            FileDateTime = _fileDateTime;
            Permissions = _permissions;
        }
        # endregion
    }
}
