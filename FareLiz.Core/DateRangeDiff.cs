using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Storage object for DateTime range
    /// </summary>
    public class DateRangeDiff
    {
        int _minus = 15, _plus = 15;
        private const string ParseRegex = @"((?<Op>[\+\-])?(?<TypeConfiguration>\d+))";

        public DateRangeDiff()
        {
        }

        public DateRangeDiff(int offset)
        {
            Plus = Minus = offset;
        }

        public DateRangeDiff(int plus, int minus)
        {
            Plus = plus;
            Minus = minus;
        }

        /// <summary>
        /// Plus offset
        /// </summary>
        public int Plus
        {
            get { return _plus; }
            set { _plus = value; }
        }

        /// <summary>
        /// Minus offset
        /// </summary>
        public int Minus
        {
            get { return _minus; }
            set { _minus = value; }
        }

        /// <summary>
        /// Returns a clone of current object
        /// </summary>
        public DateRangeDiff Clone()
        {
            return MemberwiseClone() as DateRangeDiff;
        }

        /// <summary>
        /// Create a new object based on the string representation
        /// </summary>
        public static DateRangeDiff Parse(string input)
        {
            input = input ?? String.Empty;
            var matches = Regex.Matches(input, ParseRegex);
            if (matches == null || matches.Count < 1)
                throw new ArgumentException("Invalid string input for parsing date offset: " + input);

            var result = new DateRangeDiff(0);
            foreach (Match m in matches)
            {
                string op = m.Groups["Op"].Value;
                int value = Int32.Parse(m.Groups["TypeConfiguration"].Value, CultureInfo.InvariantCulture);
                if (String.IsNullOrEmpty(op))
                    result.Plus = result.Minus = value;
                else
                {
                    if (op == "+")
                        result.Plus = value;
                    else
                        result.Minus = value;
                }
            }

            return result;
        }

        /// <summary>
        /// Empty offset (0 for both plus and minus offset)
        /// </summary>
        public static readonly DateRangeDiff Empty = new DateRangeDiff(0, 0);
    }
}