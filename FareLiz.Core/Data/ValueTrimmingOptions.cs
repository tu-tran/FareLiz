namespace SkyDean.FareLiz.Core.Data
{
    using System;

    /// <summary>The value trimming options.</summary>
    [Flags]
    public enum ValueTrimmingOptions
    {
        /// <summary>The none.</summary>
        None = 0, 

        /// <summary>The unquoted only.</summary>
        UnquotedOnly = 1, 

        /// <summary>The quoted only.</summary>
        QuotedOnly = 2, 

        /// <summary>The all.</summary>
        All = UnquotedOnly | QuotedOnly
    }
}