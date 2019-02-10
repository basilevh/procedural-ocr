// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    public class MyOCRModel : IOCRModel
    {
        public MyOCRModel(int imageWidth, int imageHeight)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
        }

        private readonly int imageWidth, imageHeight;

        public TrainResult TrainModel(int samples)
        {
            throw new NotImplementedException();
        }

        public TestResult TestModel(int samples)
        {
            throw new NotImplementedException();
        }

        public SingleResult ExecuteSingle(float[,] input)
        {
            throw new NotImplementedException();
        }
    }
}
