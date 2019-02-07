/*
 * Author: Shon Verch
 * File Name: Logger.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/07/2019
 * Modified Date: 02/07/2019
 * Description: Console logger with extra functionality. 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

using SpaceInvaders.Helpers;

namespace SpaceInvaders.Logging
{
    /// <summary>
    /// Event for when a message is logged using the logger.
    /// </summary>
    /// <param name="args"></param>
    public delegate void MessageLoggedEventHandler(MessagedLoggerEventArgs args);

    /// <summary>
    /// Event args for the MessageLogged event.
    /// </summary>
    public class MessagedLoggerEventArgs : EventArgs
    {
        /// <summary>
        /// The message which was logged.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Creates a new event args with a specified message.
        /// </summary>
        /// <param name="message"></param>
        public MessagedLoggerEventArgs(string message)
        {
            Message = message;
        }
    }


    /// <summary>
    /// Console logger with extra functionality.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Constant identifier filter for allowing any category verbosity.
        /// </summary>
        public const string AllowAnyCategoryVerbosities = "__ALLOW_ANY_CATEGORY_VERBOSITIES__";

        /// <summary>
        /// The verbosity of the logger. Only a message with this verbosity (or higher) will be logged.
        /// </summary>
        public static LoggerVerbosity Verbosity { get; set; }

        /// <summary>
        /// The destination of log messages.
        /// </summary>
        public static LoggerDestination Destination { get; set; } = LoggerDestination.File;

        /// <summary>
        /// Line seperator between messages.
        /// </summary>
        public static string LineSeperator { get; set; } = string.Empty;

        /// <summary>
        /// A global suffix for every message logged. 
        /// This is appended to the end of every message.
        /// </summary>
        public static string MessageSuffix { get; set; } = string.Empty;

        /// <summary>
        /// The frequency at which lines should be separated. 
        /// </summary>
        public static int LineSeperatorMessageInterval { get; set; } = -1;

        /// <summary>
        /// When a message with this verbosity (or higher) is logged, the whole message buffer is flushed.
        /// </summary>
        public static LoggerVerbosity FlushVerbosity { get; set; } = LoggerVerbosity.Error;

        /// <summary>
        /// This is the time (in seconds) after which we flush our log to the file.
        /// </summary>
        public static int FlushFrequency { get; set; } = 5;

        /// <summary>
        /// This is the amount of messages the message buffer can store until it must flush itself.
        /// </summary>
        public static int FlushBufferMessageCapacity { get; set; } = 100;

        /// <summary>
        /// The category verbosity filter. If set to null, then the filter will allow all categories.
        /// </summary>
        public static Dictionary<string, LoggerVerbosity> CategoryVerbosities { get; set; }

        /// <summary>
        /// The amount of messages logged since startup.
        /// </summary>
        public static int MessageCount { get; private set; }

        /// <summary>
        /// MessageLogged event. 
        /// This event is raised whenever a message is logged to any destination.
        /// </summary>
        public static event MessageLoggedEventHandler MessageLogged;

        /// <summary>
        /// Raises the MessageLogged event.
        /// </summary>
        /// <param name="args"></param>
        private static void OnMessageLogged(MessagedLoggerEventArgs args) => MessageLogged?.Invoke(args);

        private static readonly StringBuilder messageBuffer = new StringBuilder();
        private static readonly Timer messageBufferFlushTimer;

        private static string logFilePath;
        private static bool isFirstLog = true;
        private static int messageCountSinceLastFlush;

        /// <summary>
        /// Logger constructor. 
        /// This will be called when the first reference to the Logger is made.
        /// </summary>
        static Logger()
        {
            CategoryVerbosities = new Dictionary<string, LoggerVerbosity>
            {
                // Second parameter doesn't matter, if the key AllowAnyCategoryVerbosities is in the dictionary then it just logs any category.
                {
                    AllowAnyCategoryVerbosities, LoggerVerbosity.None
                }
            };

            logFilePath = GetLogFileName();

            // Initialize message buffer flush timer
            messageBufferFlushTimer = new Timer(FlushFrequency * 1000)
            {
                AutoReset = true
            };

            messageBufferFlushTimer.Elapsed += (sender, args) => FlushMessageBuffer();
            messageBufferFlushTimer.Start();

            // Clear message buffer if exists.
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            // Print format
            const string format = "[yyyy-MM-dd HH:mm:ss.fff][verbosity] <category>: <message>";
            if ((Destination & LoggerDestination.File) != 0)
            {
                messageBuffer.AppendLine(format);
                messageBuffer.AppendLine('-'.Multiply(format.Length));
            }

            if ((Destination & LoggerDestination.Output) == 0) return;

            Console.WriteLine(format);
            Console.WriteLine('-'.Multiply(format.Length));
        }

        /// <summary>
        /// Log a message in a category with a specified verbosity.
        /// </summary>
        /// <param name="category">The category to log in.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="messageVerbosity">The verbosity to log with.</param>
        /// <param name="separateLineHere">Should this message be separated by a line?</param>
        public static void Log(string category, object message, LoggerVerbosity messageVerbosity = LoggerVerbosity.Info, bool separateLineHere = false)
        {
            // The none verbosity turns off logging.
            if (messageVerbosity == LoggerVerbosity.None) return;

            // Lock the output stream of the console for operation.
            lock (Console.Out)
            {
                if (Verbosity > messageVerbosity) return;

                // Check if the category we are trying to log into allows the specified verbosity.
                if (CategoryVerbosities != null)
                {
                    bool allowAnyCategoryVerbosities = CategoryVerbosities.ContainsKey(AllowAnyCategoryVerbosities);
                    if (!CategoryVerbosities.ContainsKey(category) && !allowAnyCategoryVerbosities) return;

                    if (CategoryVerbosities.ContainsKey(category))
                    {
                        if (CategoryVerbosities[category] > messageVerbosity) return;
                    }
                }

                MessageCount++;

                string output = string.Concat(GetMessageHeader(messageVerbosity, category), message, MessageSuffix);
                string messageSeperator = string.Empty;

                bool shouldSeparateLine = separateLineHere || LineSeperatorMessageInterval > 0 &&
                        MessageCount % LineSeperatorMessageInterval == 0;

                if (!string.IsNullOrEmpty(LineSeperator) && shouldSeparateLine)
                {
                    messageSeperator = LineSeperator.Multiply(output.Length);
                }

                // Log to the file.
                if ((Destination & LoggerDestination.File) != 0)
                {
                    messageBuffer.AppendLine(output);
                    if (!string.IsNullOrEmpty(messageSeperator))
                    {
                        messageBuffer.AppendLine(messageSeperator);
                    }
                }

                // Log to the console.
                if ((Destination & LoggerDestination.Output) != 0)
                {
                    ConsoleColor oldConsoleColor = Console.ForegroundColor;
                    Console.ForegroundColor = GetVerbosityConsoleColour(messageVerbosity);

                    if (messageVerbosity == LoggerVerbosity.Plain)
                    {
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Console.WriteLine(output);
                    }

                    if (!string.IsNullOrEmpty(messageSeperator))
                    {
                        Console.WriteLine(messageSeperator);
                    }

                    Console.ForegroundColor = oldConsoleColor;
                }

                if (Destination != LoggerDestination.None)
                {
                    OnMessageLogged(new MessagedLoggerEventArgs(message.ToString()));
                }

                // Flush the message buffer if it is time.
                messageCountSinceLastFlush++;
                if (messageVerbosity >= FlushVerbosity || messageCountSinceLastFlush == FlushBufferMessageCapacity)
                {
                    FlushMessageBuffer();
                }
            }

            if (!isFirstLog) return;

            // It is our first log so we need to startup the flush timer.
            // The flush timer flushes the message buffer every X seconds.
            messageBufferFlushTimer.Start();
            isFirstLog = false;
        }

        /// <summary>
        /// Log a message with a specified verbosity in no category.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="messageVerbosity">The verbosity of the message.</param>
        public static void Log(object message, LoggerVerbosity messageVerbosity = LoggerVerbosity.Info)
        {
            Log(string.Empty, message, messageVerbosity);
        }

        /// <summary>
        /// Log the function this method is called from. 
        /// This is useful for debugging purposes.
        /// </summary>
        /// <param name="category">The category to log into.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="messageVerbosity">The verbosity of the message.</param>
        /// <param name="separateLineHere">Should this message be separated by a line?</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LogFunctionEntry(string category = "", string message = "", 
            LoggerVerbosity messageVerbosity = LoggerVerbosity.Info, bool separateLineHere = false)
        {
            // Get the last stack frame and find the calling method name.
            MethodBase methodFrame = new StackTrace().GetFrame(1).GetMethod();
            string functionName = methodFrame.Name;
            string className = "NotAvailable";

            // Attempt to retrieve the class name of the calling method.
            // If we cannot retrieve the class name, it will show up as NotAvailable
            if (methodFrame.DeclaringType != null)
            {
                className = methodFrame.DeclaringType.FullName;
            }

            string messageContents = string.IsNullOrEmpty(message) ? string.Empty : $": {message}";
            string actualMessage = $"{className}::{functionName}{messageContents}";

            Log(category, actualMessage, messageVerbosity, separateLineHere);
        }

        /// <summary>
        /// Log a message with formatting options.
        /// </summary>
        /// <param name="category">The category to log into.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="messageVerbosity">The verbosity of the message.</param>
        /// <param name="separateLineHere">Should this message be separated by a line?</param>
        /// <param name="args">The formatting arguments.</param>
        public static void LogFormat(string category, string message, LoggerVerbosity messageVerbosity = LoggerVerbosity.Info, bool separateLineHere = false, params object[] args)
        {
            Log(category, string.Format(message, args), messageVerbosity, separateLineHere);
        }

        /// <summary>
        /// Log a message with formatting options.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="messageVerbosity">The verbosity of the message.</param>
        /// <param name="separateLineHere">Should this message be separated by a line?</param>
        /// <param name="args">The formatting arguments.</param>
        public static void LogFormat(string message, LoggerVerbosity messageVerbosity = LoggerVerbosity.Info, bool separateLineHere = false, params object[] args)
        {
            Log(string.Empty, string.Format(message, args), messageVerbosity, separateLineHere);
        }

        /// <summary>
        /// Assert a condition.
        /// </summary>
        /// <param name="condition">The condition to assert.</param>
        /// <param name="message">The assert message.</param>
        /// <param name="sourceFilePath">The file path from where the call was made.</param>
        /// <param name="sourceLineNumber">The line number from where the call was made.</param>
        public static void Assert(bool condition, string message = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (condition) return;

            string assertMessage = $"Assert failed! {sourceFilePath} at line {sourceLineNumber}";
            message = !string.IsNullOrEmpty(message) ? $"{assertMessage}{Environment.NewLine}{message}" : assertMessage;

            Log(message, LoggerVerbosity.Error);
            Environment.Exit(-1);
        }

        /// <summary>
        /// Get the header of the message.
        /// </summary>
        /// <param name="verbosity">The verbosity of the message.</param>
        /// <param name="category">The category of the message.</param>
        /// <returns>The resulted header.</returns>
        private static string GetMessageHeader(LoggerVerbosity verbosity, string category)
        {
            string padding = StringHelper.Space.Multiply(GetLongestVerbosityLength() - GetVerbosityName(verbosity).Length);
            string verbosityHeader = $"[{padding}{GetVerbosityName(verbosity)}]";

            string header = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}]{verbosityHeader}";
            string categoryHeader = !string.IsNullOrEmpty(category) ? $" {category}: " : ": ";
            return string.Concat(header, categoryHeader);
        }

        /// <summary>
        /// Get the ConsoleColour of a verbosity.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        private static ConsoleColor GetVerbosityConsoleColour(LoggerVerbosity verbosity)
        {
            switch (verbosity)
            {
                case LoggerVerbosity.Info:
                    return ConsoleColor.White;
                case LoggerVerbosity.Warning:
                    return ConsoleColor.Yellow;
                case LoggerVerbosity.Error:
                    return ConsoleColor.Red;
            }

            return ConsoleColor.Gray;
        }

        /// <summary>
        /// Get the name of a verbosity.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        private static string GetVerbosityName(LoggerVerbosity verbosity)
        {
            return Enum.GetName(typeof(LoggerVerbosity), verbosity);
        }

        /// <summary>
        /// Flush the message buffer.
        /// </summary>
        /// <param name="tries"></param>
        public static void FlushMessageBuffer(int tries = 0)
        {
            if (tries > 2) return;
            if ((Destination & LoggerDestination.File) == 0) return;

            string flushContents = messageBuffer.ToString();
            if (messageCountSinceLastFlush == 0 || string.IsNullOrEmpty(flushContents)) return;

            try
            {
                File.AppendAllText(logFilePath, flushContents);
            }
            catch (IOException)
            {
                // The file is already being used (presumably), let's change the logFilePath and try again
                logFilePath = GetLogFileName("2");
                FlushMessageBuffer(tries + 1);
            }

            messageCountSinceLastFlush = 0;
            messageBuffer.Clear();
        }

        /// <summary>
        /// Get the length of the longest verbosity name.
        /// </summary>
        /// <returns></returns>
        private static int GetLongestVerbosityLength()
        {
            string[] verbosityNames = Enum.GetNames(typeof(LoggerVerbosity));
            return verbosityNames.OrderByDescending(name => name.Length).First().Length;
        }

        /// <summary>
        /// Get the name of a log file.
        /// The name of the log file is the name of the executing assembly.
        /// </summary>
        /// <param name="suffix">An optional suffix.</param>
        /// <returns></returns>
        private static string GetLogFileName(string suffix = null)
        {
            return $"{Assembly.GetEntryAssembly().GetName().Name}{suffix}.log";
        }
    }
}
