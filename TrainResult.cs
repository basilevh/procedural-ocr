// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Summary of an OCR model training event, capturing how the mini-batch errors evolve over time.
    /// </summary>
    public struct TrainResult
    {
        public TrainResult(int totalSamples, double[] batchErrors)
        {
            TotalSamples = totalSamples;
            BatchErrors = batchErrors;
        }

        public int TotalSamples { get; }

        public double[] BatchErrors { get; }
    }
}
