// (c) Vasian Cepa 2005
// Version 2

using System;
using System.Collections; // required for NumericComparer : IComparer only

namespace SkyDean.FareLiz.Core.Utils
{
    public class NumericComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return StringLogicalComparer.Compare(x.ToString(), y.ToString());
        }
    }

}