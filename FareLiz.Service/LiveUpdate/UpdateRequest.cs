using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyDean.FareLiz.Service.LiveUpdate
{
    public class UpdateRequest
    {
        public readonly string ProductName;
        public readonly VersionInfo FromVersion;
        public readonly VersionInfo ToVersion;
        public UpdateParameter Parameters;

        public UpdateRequest(string productName, VersionInfo fromVersion, VersionInfo toVersion)
        {
            ProductName = productName;
            FromVersion = fromVersion;
            ToVersion = toVersion;
        }
    }
}
