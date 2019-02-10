// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    public interface IOCRModel
    {
        TrainResult TrainModel(int samples);

        TestResult TestModel(int samples);

        SingleResult ExecuteSingle(float[,] input);
    }
}
