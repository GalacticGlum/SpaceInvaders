/*
 * Author: Shon Verch
 * File Name: Program.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/05/2019
 * Modified Date: 02/05/2019
 * Description: DESCRIPTION
 */

using System;

namespace SpaceInvaders
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (var game = new MainGame())
            {
                game.Run();
            }
        }
    }
}
