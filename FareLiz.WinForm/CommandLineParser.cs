// CommandLineParser.cs
// Parse and analyze command-line parameters

using System;
using System.Collections;
using SkyDean.FareLiz.Core.Utils;

namespace SkyDean.FareLiz.WinForm
{
    public enum SwitchType
    {
        Simple,
        PostMinus,
        LimitedPostString,
        UnLimitedPostString,
        PostChar
    }

    internal class SwitchForm
    {
        public string Description;
        public string IDString;
        public int MaxLen;
        public int MinLen;
        public bool Multi;
        public string PostCharSet;
        public SwitchType Type;

        public SwitchForm(string idString, SwitchType type, bool multi,
                          int minLen, int maxLen, string postCharSet)
        {
            IDString = idString;
            Type = type;
            Multi = multi;
            MinLen = minLen;
            MaxLen = maxLen;
            PostCharSet = postCharSet;
        }

        public SwitchForm(string idString, SwitchType type, bool multi, int minLen, string desc) :
            this(idString, type, multi, minLen, 0, "")
        {
            Description = desc;
        }

        public SwitchForm(string idString, SwitchType type, bool multi, string desc) :
            this(idString, type, multi, 0, desc)
        {
        }

        public override string ToString()
        {
            return IDString;
        }
    }

    internal class SwitchResult
    {
        public int PostCharIndex;
        public ArrayList PostStrings = new ArrayList();
        public bool ThereIs;
        public bool WithMinus;

        public SwitchResult()
        {
            ThereIs = false;
        }
    }

    internal class Parser
    {
        private const char kSwitchID1 = '-';
        private const char kSwitchID2 = '/';

        private const char kSwitchMinus = '-';
        private const string kStopSwitchParsing = "--";
        private readonly SwitchResult[] _switches;
        public ArrayList NonSwitchStrings = new ArrayList();

        public Parser(int numSwitches)
        {
            _switches = new SwitchResult[numSwitches];
            for (int i = 0; i < numSwitches; i++)
                _switches[i] = new SwitchResult();
        }

        public SwitchResult this[int index]
        {
            get { return _switches[index]; }
        }

        private bool ParseString(string srcString, SwitchForm[] switchForms)
        {
            int len = srcString.Length;
            if (len == 0)
                return false;
            int pos = 0;
            if (!IsItSwitchChar(srcString[pos]))
                return false;
            while (pos < len)
            {
                if (IsItSwitchChar(srcString[pos]))
                    pos++;
                const int kNoLen = -1;
                int matchedSwitchIndex = 0;
                int maxLen = kNoLen;
                for (int switchIndex = 0; switchIndex < _switches.Length; switchIndex++)
                {
                    int switchLen = switchForms[switchIndex].IDString.Length;
                    if (switchLen <= maxLen || pos + switchLen > len)
                        continue;
                    if (String.Compare(switchForms[switchIndex].IDString, 0,
                                       srcString, pos, switchLen, true) == 0)
                    {
                        matchedSwitchIndex = switchIndex;
                        maxLen = switchLen;
                    }
                }
                if (maxLen == kNoLen)
                    throw new ApplicationException("maxLen == kNoLen");
                SwitchResult matchedSwitch = _switches[matchedSwitchIndex];
                SwitchForm switchForm = switchForms[matchedSwitchIndex];
                if ((!switchForm.Multi) && matchedSwitch.ThereIs)
                    throw new ApplicationException("switch must be single");
                matchedSwitch.ThereIs = true;
                pos += maxLen;
                int tailSize = len - pos;
                SwitchType type = switchForm.Type;
                switch (type)
                {
                    case SwitchType.PostMinus:
                        {
                            if (tailSize == 0)
                                matchedSwitch.WithMinus = false;
                            else
                            {
                                matchedSwitch.WithMinus = (srcString[pos] == kSwitchMinus);
                                if (matchedSwitch.WithMinus)
                                    pos++;
                            }
                            break;
                        }
                    case SwitchType.PostChar:
                        {
                            if (tailSize < switchForm.MinLen)
                                throw new ApplicationException("switch is not full");
                            string charSet = switchForm.PostCharSet;
                            const int kEmptyCharValue = -1;
                            if (tailSize == 0)
                                matchedSwitch.PostCharIndex = kEmptyCharValue;
                            else
                            {
                                int index = charSet.IndexOf(srcString[pos]);
                                if (index < 0)
                                    matchedSwitch.PostCharIndex = kEmptyCharValue;
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
                                throw new ApplicationException("switch is not full");
                            if (type == SwitchType.UnLimitedPostString)
                            {
                                matchedSwitch.PostStrings.Add(srcString.Substring(pos).TrimMatchingQuotes('\"'));
                                return true;
                            }
                            String stringSwitch = srcString.Substring(pos, minLen);
                            pos += minLen;
                            for (int i = minLen; i < switchForm.MaxLen && pos < len; i++, pos++)
                            {
                                char c = srcString[pos];
                                if (IsItSwitchChar(c))
                                    break;
                                stringSwitch += c;
                            }
                            matchedSwitch.PostStrings.Add(stringSwitch.TrimMatchingQuotes('\"'));
                            break;
                        }
                }
            }
            return true;
        }

        public void ParseStrings(SwitchForm[] switchForms, string[] commandStrings)
        {
            int numCommandStrings = commandStrings.Length;
            bool stopSwitch = false;
            for (int i = 0; i < numCommandStrings; i++)
            {
                string s = commandStrings[i];
                if (stopSwitch)
                    NonSwitchStrings.Add(s);
                else if (s == kStopSwitchParsing)
                    stopSwitch = true;
                else if (!ParseString(s, switchForms))
                    NonSwitchStrings.Add(s);
            }
        }

        public static int ParseCommand(CommandForm[] commandForms, string commandString,
                                       out string postString)
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
                    postString = "";
                    return i;
                }
            }
            postString = "";
            return -1;
        }

        private static bool ParseSubCharsCommand(int numForms, CommandSubCharsSet[] forms,
                                                 string commandString, ArrayList indices)
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
                            return false;
                        if (commandString.IndexOf(c, newIndex + 1) >= 0)
                            return false;
                        currentIndex = j;
                        numUsedChars++;
                    }
                }
                if (currentIndex == -1 && !charsSet.EmptyAllowed)
                    return false;
                indices.Add(currentIndex);
            }
            return (numUsedChars == commandString.Length);
        }

        private static bool IsItSwitchChar(char c)
        {
            return (c == kSwitchID1 || c == kSwitchID2);
        }
    }

    internal class CommandForm
    {
        public string IDString = "";
        public bool PostStringMode = false;

        public CommandForm(string idString, bool postStringMode)
        {
            IDString = idString;
            PostStringMode = postStringMode;
        }
    }

    internal class CommandSubCharsSet
    {
        public string Chars = "";
        public bool EmptyAllowed = false;
    }
}