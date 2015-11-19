namespace SkyDean.FareLiz.Core
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>Storage object for DateTime range</summary>
    public class DateRangeDiff
    {
        /// <summary>The parse regex.</summary>
        private const string ParseRegex = @"((?<Op>[\+\-])?(?<TypeConfiguration>\d+))";

        /// <summary>Empty offset (0 for both plus and minus offset)</summary>
        public static readonly DateRangeDiff Empty = new DateRangeDiff(0, 0);

        /// <summary>The _minus.</summary>
        private int _minus = 15;

        /// <summary>The _plus.</summary>
        private int _plus = 15;

        /// <summary>Initializes a new instance of the <see cref="DateRangeDiff" /> class.</summary>
        public DateRangeDiff()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRangeDiff"/> class.
        /// </summary>
        /// <param name="offset">
        /// The offset.
        /// </param>
        public DateRangeDiff(int offset)
        {
            this.Plus = this.Minus = offset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateRangeDiff"/> class.
        /// </summary>
        /// <param name="plus">
        /// The plus.
        /// </param>
        /// <param name="minus">
        /// The minus.
        /// </param>
        public DateRangeDiff(int plus, int minus)
        {
            this.Plus = plus;
            this.Minus = minus;
        }

        /// <summary>Plus offset</summary>
        public int Plus
        {
            get
            {
                return this._plus;
            }

            set
            {
                this._plus = value;
            }
        }

        /// <summary>Minus offset</summary>
        public int Minus
        {
            get
            {
                return this._minus;
            }

            set
            {
                this._minus = value;
            }
        }

        /// <summary>Returns a clone of current object</summary>
        /// <returns>The <see cref="DateRangeDiff" />.</returns>
        public DateRangeDiff Clone()
        {
            return this.MemberwiseClone() as DateRangeDiff;
        }

        /// <summary>
        /// Create a new object based on the string representation
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="DateRangeDiff"/>.
        /// </returns>
        public static DateRangeDiff Parse(string input)
        {
            input = input ?? string.Empty;
            var matches = Regex.Matches(input, ParseRegex);
            if (matches == null || matches.Count < 1)
            {
                throw new ArgumentException("Invalid string input for parsing date offset: " + input);
            }

            var result = new DateRangeDiff(0);
            foreach (Match m in matches)
            {
                string op = m.Groups["Op"].Value;
                int value = int.Parse(m.Groups["TypeConfiguration"].Value, CultureInfo.InvariantCulture);
                if (string.IsNullOrEmpty(op))
                {
                    result.Plus = result.Minus = value;
                }
                else
                {
                    if (op == "+")
                    {
                        result.Plus = value;
                    }
                    else
                    {
                        result.Minus = value;
                    }
                }
            }

            return result;
        }
    }
}