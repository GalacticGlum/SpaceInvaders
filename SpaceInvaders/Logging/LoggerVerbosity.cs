/*
 * Author: Shon Verch
 * File Name: LoggerVerbosity.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/07/2019
 * Modified Date: 02/07/2019
 * Description: The different types of verbosities the Logger can log in.
 */

namespace SpaceInvaders.Logging
{
    /// <summary>
    /// The different types of verbosities the <see cref="Logger"/> can log in.
    /// </summary>
    public enum LoggerVerbosity
    {
        /// <summary>
        /// None verbosity will not log anything if used.
        /// Not really a verbosity rather a way of internally defining: "print any verbosity."
        /// </summary>
        None,

        /// <summary>
        /// Plain verbosity, this will only log the message without any extra decorations (timestamp, category).
        /// </summary>
        Plain,

        /// <summary>
        /// Regular message.
        /// </summary>
        Info,

        /// <summary>
        /// Warning message. 
        /// </summary>
        Warning,

        /// <summary>
        /// Error message. 
        /// </summary>
        Error
    }
}
