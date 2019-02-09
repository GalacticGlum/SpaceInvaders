/*
 * Author: Shon Verch
 * File Name: Random.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/09/2019
 * Modified Date: 02/09/2019
 * Description: Facility for generating random data.
 */

using System;
using RandomEngine = System.Random;

namespace SpaceInvaders.Helpers
{
    public static class Random
    {
        /// <summary>
        /// The current seed used by the random generator.
        /// </summary>
        public static int Seed { get; private set; }
        private static RandomEngine randomEngine;

        /// <summary>
        /// Initialize the random generator.
        /// </summary>
        static Random()
        {
            Reseed();
        }

        /// <summary>
        /// Reseed the random generator using system time.
        /// </summary>
        public static void Reseed() => Reseed(DateTime.Now.Millisecond);

        /// <summary>
        /// Reseed the random generating using a specified integer seed.
        /// </summary>
        /// <param name="seed">The seed to use.</param>
        public static void Reseed(int seed)
        {
            Seed = seed;
            randomEngine = new RandomEngine(Seed);
        }

        /// <summary>
        /// Generates a double-precision floating point value that is greater than or equal to 0.0 and less than 1.0; [0.0, 1.0).
        /// </summary>
        public static float Value() => (float) randomEngine.NextDouble();

        /// <summary>
        /// Generates a non-negative integer value that is less than the specified maximum; [0, <paramref name="max"/>).
        /// </summary>
        /// <param name="max">The maximum value (exclusive).</param>
        public static int Range(int max) => randomEngine.Next(max);

        /// <summary>
        /// Generates an integer value that is within the specified range; [<paramref name="min"/>, <paramref name="max"/>).
        /// </summary>
        /// <param name="min">The minimum value (inclusive).</param>
        /// <param name="max">The maximum value (exclusive).</param>
        public static int Range(int min, int max) => randomEngine.Next(min, max);

        /// <summary>
        /// Generates a non-negative floating point value that is less than the specified maximum; [0.0, <paramref name="max"/>).
        /// </summary>
        /// <param name="max">The maximum value (exclusive).</param>
        public static float Range(float max) => Range(0, max);

        /// <summary>
        /// Generates a floating point value that is within the specified range; [<paramref name="min"/>, <paramref name="max"/>).
        /// </summary>
        /// <param name="min">The minimum value (inclusive).</param>
        /// <param name="max">The maximum value (exclusive).</param>
        public static float Range(float min, float max) => (float) randomEngine.NextDouble() * (max - min) + min;

        /// <summary>
        /// Generates a non-negative double-precision floating point value that is less than the specified maximum; [0.0, <paramref name="max"/>).
        /// </summary>
        /// <param name="max">The maximum value (exclusive).</param>
        public static double Range(double max) => Range(0, max);

        /// <summary>
        /// Generates a double-precision floating point value that is within the specified range; [<paramref name="min"/>, <paramref name="max"/>).
        /// </summary>
        /// <param name="min">The minimum value (inclusive).</param>
        /// <param name="max">The maximum value (exclusive).</param>
        public static double Range(double min, double max) => randomEngine.NextDouble() * (max - min) + min;
    }
}
