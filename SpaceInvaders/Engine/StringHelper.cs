/*
 * Author: Shon Verch
 * File Name: StringHelper.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/07/2019
 * Modified Date: 02/07/2019
 * Description: Collection of useful string related functionality.
 */

using System.Linq;

namespace SpaceInvaders.Engine
{
    /// <summary>
    /// Collection of useful string related functionality.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Space character constant.
        /// </summary>
        public const string Space = " ";

        /// <summary>
        /// Multiplies a string by a given amount.
        /// </summary>
        /// <param name="a">The string to multiply.</param>
        /// <param name="length">The amount to multiply the string by.</param>
        /// <returns>The result of the string multiplication.</returns>
        public static string Multiply(this string a, int length) => string.Concat(Enumerable.Repeat(a, length));

        /// <summary>
        /// Multiplies a character by a given amount.
        /// </summary>
        /// <param name="a">The character to multiply.</param>
        /// <param name="length">The amount to multiply the character by.</param>
        /// <returns>The result of the string multiplication.</returns>
        public static string Multiply(this char a, int length) => new string(a, length);
    }
}
