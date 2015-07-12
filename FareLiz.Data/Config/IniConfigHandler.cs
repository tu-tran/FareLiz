using System;
using SkyDean.FareLiz.Core.Data;

namespace SkyDean.FareLiz.Data.Config
{
    public static class IniConfigHandler
    {
        public static bool FromString(string data, Type targetType, out object output)
        {
            bool supported = true;
            if (targetType == typeof(Airport))
                output = AirportDataProvider.FromIATA(data);
            else
            {
                supported = false;
                output = null;
            }

            return supported;
        }

        public static bool ToStorageString(object data, out string output)
        {
            bool supported = true;
            if (data != null)
            {
                var test = data as Airport;
                if (test != null)
                    output = test.IATA;
                else
                {
                    supported = false;
                    output = null;
                }
            }
            else
            {
                supported = false;
                output = null;
            }

            return supported;
        }
    }
}
