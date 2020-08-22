using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ColorControl
{

    /// <summary>
    ///  Extension methods for Exception class.
    /// </summary>
    static class ExceptionExtensions
    {
        /// <summary>
        ///  Provides full stack trace for the exception that occurred.
        /// </summary>
        /// <param name="exception">Exception object.</param>
        /// <param name="environmentStackTrace">Environment stack trace, for pulling additional stack frames.</param>
        public static string ToLogString(this Exception exception, string environmentStackTrace)
        {
            List<string> environmentStackTraceLines = ExceptionExtensions.GetUserStackTraceLines(environmentStackTrace);
            environmentStackTraceLines.RemoveAt(0);

            List<string> stackTraceLines = ExceptionExtensions.GetStackTraceLines(exception.StackTrace);
            stackTraceLines.AddRange(environmentStackTraceLines);

            string fullStackTrace = String.Join(Environment.NewLine, stackTraceLines);

            string logMessage = exception.Message + Environment.NewLine + fullStackTrace;
            return logMessage;
        }

        /// <summary>
        ///  Gets a list of stack frame lines, as strings.
        /// </summary>
        /// <param name="stackTrace">Stack trace string.</param>
        private static List<string> GetStackTraceLines(string stackTrace)
        {
            return stackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }

        /// <summary>
        ///  Gets a list of stack frame lines, as strings, only including those for which line number is known.
        /// </summary>
        /// <param name="fullStackTrace">Full stack trace, including external code.</param>
        private static List<string> GetUserStackTraceLines(string fullStackTrace)
        {
            List<string> outputList = new List<string>();
            Regex regex = new Regex(@"([^\)]*\)) in (.*):line (\d)*$");

            List<string> stackTraceLines = ExceptionExtensions.GetStackTraceLines(fullStackTrace);
            foreach (string stackTraceLine in stackTraceLines)
            {
                if (!regex.IsMatch(stackTraceLine))
                {
                    continue;
                }

                outputList.Add(stackTraceLine);
            }

            return outputList;
        }
    }
}
