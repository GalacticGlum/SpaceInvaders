/*
 * Author: Shon Verch
 * File Name: MathHelper.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/06/2019
 * Modified Date: 02/06/2019
 * Description: A collection of useful utilities that perform mathematical computations.
 */

namespace SpaceInvaders.Engine
{
    public static class MathHelper
    {
        /// <summary>
        /// The initial guess for the inverse square root function
        /// using Newton's method.
        /// </summary>
        private const int InverseSqrtApproximation = 0x5f3759df;

        /// <summary>
        /// <para>Perform an inverse square root using Chris Lomont's fast method.</para>
        /// The implementation is from Lomont's wonderful paper
        /// "Fast Inverse Square Root" (<see cref="http://www.lomont.org/Math/Papers/2003/InvSqrt.pdf"/>).
        /// </summary>
        /// <param name="x">The value of the radicand.</param>
        /// <returns>The value of the inverse sqrt of <paramref name="x"/></returns>
        public static unsafe float InverseSqrt(float x)
        {
            float xhalf = 0.5f * x;
            int i = InverseSqrtApproximation - (*(int*)&x >> 1);
            x = *(float*) &i;
            x = x * (1.5f - xhalf * x * x);

            return x;
        }
    }
}
