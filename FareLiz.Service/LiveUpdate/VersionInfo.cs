namespace SkyDean.FareLiz.Service.LiveUpdate
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The version info.
    /// </summary>
    [DebuggerDisplay("{VersionNumber} - {CreatedDate} - {Location}")]
    public class VersionInfo
    {
        /// <summary>
        /// The created date.
        /// </summary>
        public readonly DateTime CreatedDate;

        /// <summary>
        /// The version number.
        /// </summary>
        public readonly Version VersionNumber;

        /// <summary>
        /// The x 64 package.
        /// </summary>
        public readonly string X64Package;

        /// <summary>
        /// The x 86 package.
        /// </summary>
        public readonly string X86Package;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfo"/> class.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="createdDate">
        /// The created date.
        /// </param>
        /// <param name="x86package">
        /// The x 86 package.
        /// </param>
        /// <param name="x64package">
        /// The x 64 package.
        /// </param>
        public VersionInfo(string version, DateTime createdDate, string x86package, string x64package)
            : this(new Version(version), createdDate, x86package, x64package)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfo"/> class.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="createdDate">
        /// The created date.
        /// </param>
        /// <param name="x86package">
        /// The x 86 package.
        /// </param>
        /// <param name="x64package">
        /// The x 64 package.
        /// </param>
        public VersionInfo(Version version, DateTime createdDate, string x86package, string x64package)
        {
            this.VersionNumber = version;
            this.CreatedDate = createdDate;
            this.X86Package = x86package;
            this.X64Package = x64package;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfo"/> class.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        public VersionInfo(VersionInfo other)
            : this(other.VersionNumber, other.CreatedDate, other.X86Package, other.X64Package)
        {
        }
    }
}