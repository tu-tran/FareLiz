// CommandLineParser.cs
// Parse and analyze command-line parameters
namespace SkyDean.FareLiz.WinForm
{
    using System;
    using System.Collections;

    using SkyDean.FareLiz.Core.Utils;

    /// <summary>The switch type.</summary>
    public enum SwitchType
    {
        /// <summary>The simple.</summary>
        Simple, 

        /// <summary>The post minus.</summary>
        PostMinus, 

        /// <summary>The limited post string.</summary>
        LimitedPostString, 

        /// <summary>The un limited post string.</summary>
        UnLimitedPostString, 

        /// <summary>The post char.</summary>
        PostChar
    }

    /// <summary>The switch form.</summary>
    internal class SwitchForm
    {
        /// <summary>The description.</summary>
        public string Description;

        /// <summary>The id string.</summary>
        public string IDString;

        /// <summary>The max len.</summary>
        public int MaxLen;

        /// <summary>The min len.</summary>
        public int MinLen;

        /// <summary>The multi.</summary>
        public bool Multi;

        /// <summary>The post char set.</summary>
        public string PostCharSet;

        /// <summary>The type.</summary>
        public SwitchType Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchForm"/> class.
        /// </summary>
        /// <param name="idString">
        /// The id string.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="multi">
        /// The multi.
        /// </param>
        /// <param name="minLen">
        /// The min len.
        /// </param>
        /// <param name="maxLen">
        /// The max len.
        /// </param>
        /// <param name="postCharSet">
        /// The post char set.
        /// </param>
        public SwitchForm(string idString, SwitchType type, bool multi, int minLen, int maxLen, string postCharSet)
        {
            this.IDString = idString;
            this.Type = type;
            this.Multi = multi;
            this.MinLen = minLen;
            this.MaxLen = maxLen;
            this.PostCharSet = postCharSet;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchForm"/> class.
        /// </summary>
        /// <param name="idString">
        /// The id string.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="multi">
        /// The multi.
        /// </param>
        /// <param name="minLen">
        /// The min len.
        /// </param>
        /// <param name="desc">
        /// The desc.
        /// </param>
        public SwitchForm(string idString, SwitchType type, bool multi, int minLen, string desc)
            : this(idString, type, multi, minLen, 0, string.Empty)
        {
            this.Description = desc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchForm"/> class.
        /// </summary>
        /// <param name="idString">
        /// The id string.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="multi">
        /// The multi.
        /// </param>
        /// <param name="desc">
        /// The desc.
        /// </param>
        public SwitchForm(string idString, SwitchType type, bool multi, string desc)
            : this(idString, type, multi, 0, desc)
        {
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return this.IDString;
        }
    }

    /// <summary>The switch result.</summary>
    internal class SwitchResult
    {
        /// <summary>The post char index.</summary>
        public int PostCharIndex;

        /// <summary>The post strings.</summary>
        public ArrayList PostStrings = new ArrayList();

        /// <summary>The there is.</summary>
        public bool ThereIs;

        /// <summary>The with minus.</summary>
        public bool WithMinus;

        /// <summary>Initializes a new instance of the <see cref="SwitchResult" /> class.</summary>
        public SwitchResult()
        {
            this.ThereIs = false;
        }
    }

    /// <summary>The parser.</summary>
    internal class Parser
    {
        /// <summary>The k switch i d 1.</summary>
        private const char kSwitchID1 = '-';

        /// <summary>The k switch i d 2.</summary>
        private const char kSwitchID2 = '/';

        /// <summary>The k switch minus.</summary>
        private const char kSwitchMinus = '-';

        /// <summary>The k stop switch parsing.</summary>
        private const string kStopSwitchParsing = "--";

        /// <summary>The _switches.</summary>
        private readonly SwitchResult[] _switches;

        /// <summary>The non switch strings.</summary>
        public ArrayList NonSwitchStrings = new ArrayList();

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="numSwitches">
        /// The num switches.
        /// </param>
        public Parser(int numSwitches)
        {
            this._switches = new SwitchResult[numSwitches];
            for (int i = 0; i < numSwitches; i++)
            {
                this._switches[i] = new SwitchResult();
            }
        }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="SwitchResult"/>.
        /// </returns>
        public SwitchResult this[int index]
        {
            get
            {
                return this._switches[index];
            }
        }

        /// <summary>
        /// The parse string.
        /// </summary>
        /// <param name="srcString">
        /// The src string.
        /// </param>
        /// <param name="switchForms">
        /// The switch forms.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        private bool ParseString(string srcString, SwitchForm[] switchForms)
        {
            int len = srcString.Length;
            if (len == 0)
            {
                return false;
            }

            int pos = 0;
            if (!IsItSwitchChar(srcString[pos]))
            {
                return false;
            }

            while (pos < len)
            {
                if (IsItSwitchChar(srcString[pos]))
                {
                    pos++;
                }

                const int kNoLen = -1;
                int matchedSwitchIndex = 0;
                int maxLen = kNoLen;
                for (int switchIndex = 0; switchIndex < this._switches.Length; switchIndex++)
                {
                    int switchLen = switchForms[switchIndex].IDString.Length;
                    if (switchLen <= maxLen || pos + switchLen > len)
                    {
                        continue;
                    }

                    if (string.Compare(switchForms[switchIndex].IDString, 0, srcString, pos, switchLen, true) == 0)
                    {
                        matchedSwitchIndex = switchIndex;
                        maxLen = switchLen;
                    }
                }

                if (maxLen == kNoLen)
                {
                    throw new ApplicationException("maxLen == kNoLen");
                }

                SwitchResult matchedSwitch = this._switches[matchedSwitchIndex];
                SwitchForm switchForm = switchForms[matchedSwitchIndex];
                if ((!switchForm.Multi) && matchedSwitch.ThereIs)
                {
                    throw new ApplicationException("switch must be single");
                }

                matchedSwitch.ThereIs = true;
                pos += maxLen;
                int tailSize = len - pos;
                SwitchType type = switchForm.Type;
                switch (type)
                {
                    case SwitchType.PostMinus:
                        {
                            if (tailSize == 0)
                            {
                                matchedSwitch.WithMinus = false;
                            }
                            else
                            {
                                matchedSwitch.WithMinus = srcString[pos] == kSwitchMinus;
                                if (matchedSwitch.WithMinus)
                                {
                                    pos++;
                                }
                            }

                            break;
                        }

                    case SwitchType.PostChar:
                        {
                            if (tailSize < switchForm.MinLen)
                            {
                                throw new ApplicationException("switch is not full");
                            }

                            string charSet = switchForm.PostCharSet;
                            const int kEmptyCharValue = -1;
                            if (tailSize == 0)
                            {
                                matchedSwitch.PostCharIndex = kEmptyCharValue;
                            }
                            else
                            {
                                int index = charSet.IndexOf(srcString[pos]);
                                if (index < 0)
                                {
                                    matchedSwitch.PostCharIndex = kEmptyCharValue;
                                }
                                else
                                {
                                    matchedSwitch.PostCharIndex = index;
                                    pos++;
                                }
                            }

                            break;
                        }

                    case SwitchType.LimitedPostString:
                    case SwitchType.UnLimitedPostString:
                        {
                            int minLen = switchForm.MinLen;
                            if (tailSize < minLen)
                            {
                                throw new ApplicationException("switch is not full");
                            }

                            if (type == SwitchType.UnLimitedPostString)
                            {
                                matchedSwitch.PostStrings.Add(srcString.Substring(pos).TrimMatchingQuotes('\"'));
                                return true;
                            }

                            string stringSwitch = srcString.Substring(pos, minLen);
                            pos += minLen;
                            for (int i = minLen; i < switchForm.MaxLen && pos < len; i++, pos++)
                            {
                                char c = srcString[pos];
                                if (IsItSwitchChar(c))
                                {
                                    break;
                                }

                                stringSwitch += c;
                            }

                            matchedSwitch.PostStrings.Add(stringSwitch.TrimMatchingQuotes('\"'));
                            break;
                        }
                }
            }

            return true;
        }

        /// <summary>
        /// The parse strings.
        /// </summary>
        /// <param name="switchForms">
        /// The switch forms.
        /// </param>
        /// <param name="commandStrings">
        /// The command strings.
        /// </param>
        public void ParseStrings(SwitchForm[] switchForms, string[] commandStrings)
        {
            int numCommandStrings = commandStrings.Length;
            bool stopSwitch = false;
            for (int i = 0; i < numCommandStrings; i++)
            {
                string s = commandStrings[i];
                if (stopSwitch)
                {
                    this.NonSwitchStrings.Add(s);
                }
                else if (s == kStopSwitchParsing)
                {
                    stopSwitch = true;
                }
                else if (!this.ParseString(s, switchForms))
                {
                    this.NonSwitchStrings.Add(s);
                }
            }
        }

        /// <summary>
        /// The parse command.
        /// </summary>
        /// <param name="commandForms">
        /// The command forms.
        /// </param>
        /// <param name="commandString">
        /// The command string.
        /// </param>
        /// <param name="postString">
        /// The post string.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ParseCommand(CommandForm[] commandForms, string commandString, out string postString)
        {
            for (int i = 0; i < commandForms.Length; i++)
            {
                string id = commandForms[i].IDString;
                if (commandForms[i].PostStringMode)
                {
                    if (commandString.IndexOf(id, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        postString = commandString.Substring(id.Length);
                        return i;
                    }
                }
                else if (commandString == id)
                {
                    postString = string.Empty;
                    return i;
                }
            }

            postString = string.Empty;
            return -1;
        }

        /// <summary>
        /// The parse sub chars command.
        /// </summary>
        /// <param name="numForms">
        /// The num forms.
        /// </param>
        /// <param name="forms">
        /// The forms.
        /// </param>
        /// <param name="commandString">
        /// The command string.
        /// </param>
        /// <param name="indices">
        /// The indices.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool ParseSubCharsCommand(int numForms, CommandSubCharsSet[] forms, string commandString, ArrayList indices)
        {
            indices.Clear();
            int numUsedChars = 0;
            for (int i = 0; i < numForms; i++)
            {
                CommandSubCharsSet charsSet = forms[i];
                int currentIndex = -1;
                int len = charsSet.Chars.Length;
                for (int j = 0; j < len; j++)
                {
                    char c = charsSet.Chars[j];
                    int newIndex = commandString.IndexOf(c);
                    if (newIndex >= 0)
                    {
                        if (currentIndex >= 0)
                        {
                            return false;
                        }

                        if (commandString.IndexOf(c, newIndex + 1) >= 0)
                        {
                            return false;
                        }

                        currentIndex = j;
                        numUsedChars++;
                    }
                }

                if (currentIndex == -1 && !charsSet.EmptyAllowed)
                {
                    return false;
                }

                indices.Add(currentIndex);
            }

            return numUsedChars == commandString.Length;
        }

        /// <summary>
        /// The is it switch char.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsItSwitchChar(char c)
        {
            return c == kSwitchID1 || c == kSwitchID2;
        }
    }

    /// <summary>The command form.</summary>
    internal class CommandForm
    {
        /// <summary>The id string.</summary>
        public string IDString = string.Empty;

        /// <summary>The post string mode.</summary>
        public bool PostStringMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandForm"/> class.
        /// </summary>
        /// <param name="idString">
        /// The id string.
        /// </param>
        /// <param name="postStringMode">
        /// The post string mode.
        /// </param>
        public CommandForm(string idString, bool postStringMode)
        {
            this.IDString = idString;
            this.PostStringMode = postStringMode;
        }
    }

    /// <summary>The command sub chars set.</summary>
    internal class CommandSubCharsSet
    {
        /// <summary>The chars.</summary>
        public string Chars = string.Empty;

        /// <summary>The empty allowed.</summary>
        public bool EmptyAllowed = false;
    }
}