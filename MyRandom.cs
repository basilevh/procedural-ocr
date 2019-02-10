// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Utility class to sample random values from non-trivial distributions.
    /// </summary>
    public static class MyRandom
    {
        private static readonly double twoPi = 2.0 * Math.PI;
        private static readonly Random random = new Random();

        /// <summary>
        /// Generates several values from the standard normal distribution using the Box-Muller transform.
        /// </summary>
        /// <param name="count">The amount of random values to generate. For optimal performance, this value should be a multiple of two.</param>
        /// <returns>An array of length 'count' that consists of randomly sampled values.</returns>
        public static double[] NextStdGaussian(int count)
        {
            double[] result = new double[count];
            for (int i = 0; i < count; i += 2)
            {
                double u1 = random.NextDouble();
                double u2 = random.NextDouble();
                double magn = Math.Sqrt(-2.0 * Math.Log(u1));
                double z1 = magn * Math.Cos(twoPi * u2);
                double z2 = magn * Math.Sin(twoPi * u2);
                result[i] = z1;
                if (i < count - 1)
                {
                    result[i + 1] = z2;
                }
            }
            return result;
        }
    }
}
