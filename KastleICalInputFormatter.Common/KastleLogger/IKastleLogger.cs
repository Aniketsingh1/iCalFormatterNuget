using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KastleICalInputFormatter.Common.KastleLogger
{
    public interface IKastleLogger
    {

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        void LogDebug(string message = "Logging Start", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        void LogDebug(Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");
        /// <summary>
        /// Logs the start.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        void LogInformation(string message = "Logging Start", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");
        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void LogWarning(string message = "Logging Start", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");

        /// <summary>
        /// Logs the start.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        void LogInformationStart(string message = "Method Start", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");

        /// <summary>
        /// Logs the end.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        void LogInformationEnd(string message = "Method End", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");

        void LogDebugStart(string message = "Method Start", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");

        void LogDebugEnd(string message = "Method End", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");
        /// <summary>
        /// Logs the API error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        void LogError(Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");
        /// <summary>
        /// Logs the API error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        void LogError(string message = "Some error occured in method", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");

        /// <summary>
        /// Logs the API information.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        void LogApiInformation(string message = "InSufficientData", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");

        void LogApiCriticalError(string message = "Critical error occured in method", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "");
    }
}
