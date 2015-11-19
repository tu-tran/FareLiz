namespace SkyDean.FareLiz.Data.Config
{
    using System;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>
    /// The ini config handler.
    /// </summary>
    public static class IniConfigHandler
    {
        /// <summary>
        /// The from string.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool FromString(string data, Type targetType, out object output)
        {
            bool supported = true;
            if (targetType == typeof(Airport))
            {
                output = AirportDataProvider.FromIATA(data);
            }
            else
            {
                supported = false;
                output = null;
            }

            return supported;
        }

        /// <summary>
        /// The to storage string.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ToStorageString(object data, out string output)
        {
            bool supported = true;
            if (data != null)
            {
                var test = data as Airport;
                if (test != null)
                {
                    output = test.IATA;
                }
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