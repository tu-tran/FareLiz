namespace SkyDean.FareLiz.Data.Web
{
    using SkyDean.FareLiz.Data.Properties;
    using System;
    using System.IO;

    /// <summary>
    /// The user agent data.
    /// </summary>
    public static class UserAgent
    {
        /// <summary>
        /// Gets the user agent..
        /// </summary>
        /// <returns>The user agent.</returns>
        public static string Get()
        {
            var rand = new Random(DateTime.Now.Millisecond).Next(0, 98);
            var current = 0;
            string agent = null;
            using (var reader = new StringReader(Resources.UserAgents))
            {
                do
                {
                    agent = reader.ReadLine();
                }
                while (current++ < rand);
            }

            return agent;
        }
    }
}
