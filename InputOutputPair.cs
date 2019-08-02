// 02-08-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    public class InputOutputPair
    {
        public InputOutputPair(float[] input, float[] output)
        {
            Input = input;
            Output = output;
        }

        public float[] Input { get; set; }

        public float[] Output { get; set; }
    }
}
