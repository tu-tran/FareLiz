using System.Text;

namespace SkyDean.FareLiz.Core.Utils
{
    public sealed class DataGrep
    {
        private readonly StringFormatter _formatter;

        public DataGrep(byte[] seed)
        {
            _formatter = new StringFormatter(seed);
        }

#if DEBUG
        public string GenerateBytesString(string input)
        {
            var bytes = Convert(input);

            var sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.AppendFormat("0x{0:x2}, ", b);
            var result = sb.ToString().TrimEnd(", ".ToCharArray());
            return result;
        }
#endif

        public string Convert(byte[] data)
        {
            if (data == null)
                return null;

            var hexStr = Encoding.UTF8.GetString(data);
            string result = _formatter.Untag(hexStr);
            return result;
        }

        public byte[] Convert(string data)
        {
            var raw = _formatter.Tag(data);
            var result = Encoding.UTF8.GetBytes(raw);
            return result;
        }
    }
}
