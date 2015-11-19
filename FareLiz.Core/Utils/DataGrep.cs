namespace SkyDean.FareLiz.Core.Utils
{
    using System.Text;

    /// <summary>The data grep.</summary>
    public sealed class DataGrep
    {
        /// <summary>The _formatter.</summary>
        private readonly StringFormatter _formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGrep"/> class.
        /// </summary>
        /// <param name="seed">
        /// The seed.
        /// </param>
        public DataGrep(byte[] seed)
        {
            this._formatter = new StringFormatter(seed);
        }

#if DEBUG

        /// <summary>
        /// The generate bytes string.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GenerateBytesString(string input)
        {
            var bytes = this.Convert(input);

            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.AppendFormat("0x{0:x2}, ", b);
            }

            var result = sb.ToString().TrimEnd(", ".ToCharArray());
            return result;
        }

#endif

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Convert(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            var hexStr = Encoding.UTF8.GetString(data);
            string result = this._formatter.Untag(hexStr);
            return result;
        }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] Convert(string data)
        {
            var raw = this._formatter.Tag(data);
            var result = Encoding.UTF8.GetBytes(raw);
            return result;
        }
    }
}