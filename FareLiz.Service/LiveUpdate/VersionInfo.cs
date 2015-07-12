using System;
using System.Diagnostics;

namespace SkyDean.FareLiz.Service.LiveUpdate
{
    [DebuggerDisplay("{VersionNumber} - {CreatedDate} - {Location}")]
    public class VersionInfo
    {
        public readonly Version VersionNumber;
        public readonly DateTime CreatedDate;
        public readonly string X64Package;
        public readonly string X86Package;

        public VersionInfo(string version, DateTime createdDate, string x86package, string x64package)
            : this(new Version(version), createdDate, x86package, x64package) { }

        public VersionInfo(Version version, DateTime createdDate, string x86package, string x64package)
        {
            VersionNumber = version;
            CreatedDate = createdDate;
            X86Package = x86package;
            X64Package = x64package;
        }

        public VersionInfo(VersionInfo other) : this(other.VersionNumber, other.CreatedDate, other.X86Package, other.X64Package) { }
    }
}
