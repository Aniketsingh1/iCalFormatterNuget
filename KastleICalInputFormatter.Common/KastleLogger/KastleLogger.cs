using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KastleICalInputFormatter.Common.KastleLogger
{
    /// <summary>
    /// KastleLogger
    /// </summary>
    /// <seealso cref="KastleIntegration.Common.Infra.IKastleLogger" />
    public class KastleLogger : IKastleLogger
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<KastleLogger> _logger;


        /// <summary>
        /// Initializes a new instance of the <see cref="KastleLogger" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public KastleLogger(ILogger<KastleLogger> logger)
        {
            _logger = logger;

        }
        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        public void LogDebug(string message = "Logging Start", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)} {Environment.NewLine}{"\t"}{"\t"}Message:: {message}";
            _logger.LogDebug(message);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        public void LogDebug(Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            string message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)} {Environment.NewLine}{"\t"}{"\t"}Message:: {exception.Message}{Environment.NewLine}{"\t"}{"\t"}Stack Trace:: {exception.StackTrace}";
            _logger.LogDebug(message);
        }

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        public void LogInformation(string message = "Logging Start", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)} {Environment.NewLine}{"\t"}{"\t"}Message:: {message}";
            _logger.LogInformation(message);
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        public void LogWarning(string message = "Logging Start", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)} {Environment.NewLine}{"\t"}{"\t"}Message:: {message}";
            _logger.LogWarning(message);
        }

        /// <summary>
        /// Logs the start.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        public void LogInformationStart(string message = "Method Start", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)}{"\t"}Message:: {message}";
            _logger.LogInformation(message);

        }
        /// <summary>
        /// Logs the end.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        public void LogInformationEnd(string message = "Method End", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)}{"\t"}Message:: {message}";
            _logger.LogInformation(message);

        }

        /// <summary>
        /// Logs the debug start.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        public void LogDebugStart(string message = "Method Start", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)}{"\t"}Message:: {message}";
            _logger.LogDebug(message);

        }
        /// <summary>
        /// Logs the end.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        public void LogDebugEnd(string message = "Method End", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)}{"\t"}Message:: {message}";
            _logger.LogDebug(message);
        }
        /// <summary>
        /// Logs the API error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        public void LogError(Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            var errormessage = exception.InnerException == null ? exception.Message : exception.InnerException.Message;
            string message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)} {Environment.NewLine}{"\t"}{"\t"}Error Message:: {exception.Message}{Environment.NewLine}{"\t"}{"\t"}Stack Trace:: {exception.StackTrace}";

            _logger.LogCritical(message);
        }

        /// <summary>
        /// Logs the API error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        public void LogError(string message = "Some error occured in method", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {

            message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)} {Environment.NewLine}{"\t"}{"\t"}Error Message:: {message}";
            _logger.LogCritical(message);


        }
        /// <summary>
        /// Logs the API information.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        public void LogApiInformation(string message = "InSufficientData", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)} {Environment.NewLine}{"\t"}{"\t"}Message:: {message}";
            _logger.LogInformation(message);
        }

        public void LogApiCriticalError(string message = "Critical error occured in method", [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            message = $"Caller:: {callerMemberName}{"()"}{"\t"}FileName:: {Path.GetFileName(callerFilePath)} {Environment.NewLine}{"\t"}{"\t"}Error Message:: {message}";
            _logger.LogCritical(message);
        }
    }
}
