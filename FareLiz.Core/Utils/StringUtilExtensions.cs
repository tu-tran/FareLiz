namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>The string util extensions.</summary>
    public static class StringUtilExtensions
    {
        /// <summary>
        /// The split.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<string> Split(this string str, Func<char, bool> controller)
        {
            var nextPiece = 0;

            for (var c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }

        /// <summary>
        /// The take.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="charCount">
        /// The char count.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Take(this string input, int charCount)
        {
            if (string.IsNullOrEmpty(input) || charCount >= input.Length)
            {
                return input;
            }

            return input.Substring(0, charCount - 1);
        }

        /// <summary>
        /// The trim matching quotes.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="quote">
        /// The quote.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) && (input[0] == quote) && (input[input.Length - 1] == quote))
            {
                return input.Substring(1, input.Length - 2);
            }

            return input;
        }

        /// <summary>
        /// The split command line.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        public static string[] SplitCommandLine(this string input)
        {
            var inQuotes = false;

            return input.Split(
                c =>
                    {
                        if (c == '\"')
                        {
                            inQuotes = !inQuotes;
                        }

                        return !inQuotes && c == ' ';
                    }).Select(arg => TrimMatchingQuotes(arg.Trim(), '\"')).Where(arg => !string.IsNullOrEmpty(arg)).ToArray();
        }

        /// <summary>
        /// The is match.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsMatch(this string input, string pattern)
        {
            return IsMatch(input, pattern, '?', '*');
        }

        /// <summary>
        /// Tests whether specified string can be matched agains provided pattern string. Pattern may contain single- and multiple-replacing wildcard
        /// characters.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="singleWildcard">
        /// The single Wildcard.
        /// </param>
        /// <param name="multipleWildcard">
        /// The multiple Wildcard.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsMatch(this string input, string pattern, char singleWildcard, char multipleWildcard)
        {
            var inputPosStack = new int[(input.Length + 1) * (pattern.Length + 1)];

            // Stack containing input positions that should be tested for further matching
            var patternPosStack = new int[inputPosStack.Length];

            // Stack containing pattern positions that should be tested for further matching
            var stackPos = -1; // Points to last occupied entry in stack; -1 indicates that stack is empty
            var pointTested = new bool[input.Length + 1][];

            // Each true value indicates that input position vs. pattern position has been tested
            for (var i = 0; i < pointTested.Length; i++)
            {
                pointTested[i] = new bool[pattern.Length + 1];
            }

            var inputPos = 0; // Position in input matched up to the first multiple wildcard in pattern
            var patternPos = 0; // Position in pattern matched up to the first multiple wildcard in pattern

            // Match beginning of the string until first multiple wildcard in pattern
            while (inputPos < input.Length && patternPos < pattern.Length && pattern[patternPos] != multipleWildcard
                   && (input[inputPos] == pattern[patternPos] || pattern[patternPos] == singleWildcard))
            {
                inputPos++;
                patternPos++;
            }

            // Push this position to stack if it points to end of pattern or to a general wildcard
            if (patternPos == pattern.Length || pattern[patternPos] == multipleWildcard)
            {
                pointTested[inputPos][patternPos] = true;
                inputPosStack[++stackPos] = inputPos;
                patternPosStack[stackPos] = patternPos;
            }

            var matched = false;

            // Repeat matching until either string is matched against the pattern or no more parts remain on stack to test
            while (stackPos >= 0 && !matched)
            {
                inputPos = inputPosStack[stackPos]; // Pop input and pattern positions from stack
                patternPos = patternPosStack[stackPos--];

                // Matching will succeed if rest of the input string matches rest of the pattern
                if (inputPos == input.Length && patternPos == pattern.Length)
                {
                    matched = true; // Reached end of both pattern and input string, hence matching is successful
                }
                else
                {
                    // First character in next pattern block is guaranteed to be multiple wildcard
                    // So skip it and search for all matches in value string until next multiple wildcard character is reached in pattern
                    for (var curInputStart = inputPos; curInputStart < input.Length; curInputStart++)
                    {
                        var curInputPos = curInputStart;
                        var curPatternPos = patternPos + 1;
                        if (curPatternPos == pattern.Length)
                        {
                            // Pattern ends with multiple wildcard, hence rest of the input string is matched with that character
                            curInputPos = input.Length;
                        }
                        else
                        {
                            while (curInputPos < input.Length && curPatternPos < pattern.Length && pattern[curPatternPos] != multipleWildcard
                                   && (input[curInputPos] == pattern[curPatternPos] || pattern[curPatternPos] == singleWildcard))
                            {
                                curInputPos++;
                                curPatternPos++;
                            }
                        }

                        // If we have reached next multiple wildcard character in pattern without breaking the matching sequence, then we have another candidate for full match
                        // This candidate should be pushed to stack for further processing
                        // At the same time, pair (input position, pattern position) will be marked as tested, so that it will not be pushed to stack later again
                        if (((curPatternPos == pattern.Length && curInputPos == input.Length)
                             || (curPatternPos < pattern.Length && pattern[curPatternPos] == multipleWildcard))
                            && !pointTested[curInputPos][curPatternPos])
                        {
                            pointTested[curInputPos][curPatternPos] = true;
                            inputPosStack[++stackPos] = curInputPos;
                            patternPosStack[stackPos] = curPatternPos;
                        }
                    }
                }
            }

            return matched;
        }
    }
}