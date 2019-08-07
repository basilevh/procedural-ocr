// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Summary of an OCR model test, capturing how well each digit performs with respect to recongition.
    /// The indices refer to the actual characters shown by an image, so false negatives are counted as
    /// incorrect for that index under consideration (and not for the falsely detected character).
    /// </summary>
    public struct TestResult
    {
        public TestResult(int[] tested, int[] correct)
        {
            Tested = tested;
            Correct = correct;
            Accuracy = correct.Zip(tested, (a, b) => (double)a / b).ToArray();
            TotalTested = tested.Sum();
            TotalCorrect = correct.Sum();
            TotalAccuracy = (double)TotalCorrect / TotalTested;
        }

        public int TotalTested { get; }

        public int TotalCorrect { get; }

        public double TotalAccuracy { get; }

        public int[] Tested { get; }

        public int[] Correct { get; }

        public double[] Accuracy { get; }
    }
}
