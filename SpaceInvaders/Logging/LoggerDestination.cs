/*
 * Author: Shon Verch
 * File Name: LoggerDestination.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/07/2019
 * Modified Date: 02/07/2019
 * Description: The different types of destinations the Logger can log to.
 */

using System;

namespace SpaceInvaders.Logging
{
    /// <summary>
    /// The different types of destinations the <see cref="Logger"/> can log to.
    /// </summary>
    [Flags]
    public enum LoggerDestination
    {
        /// <summary>
        /// Do not log to anywhere.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only log to the standard output (<see cref="Console"/> or <see cref="Trace"/>).
        /// </summary>
        Output = 1,

        /// <summary>
        /// Only log to file.
        /// </summary>
        File = 2,

        /// <summary>
        /// Log to both file and standard output.
        /// </summary>
        All = File | Output
    }
}
