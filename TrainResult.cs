// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    public struct TrainResult
    {
        public TrainResult(float[] batchErrors)
        {
            BatchErrors = batchErrors;
        }

        public float[] BatchErrors { get; }
    }
}
