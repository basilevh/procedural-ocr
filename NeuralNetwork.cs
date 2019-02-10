// 08-01-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    public class NeuralNetwork
    {
        public NeuralNetwork(List<int> layerSizes)
        {
            int layerCount = layerSizes.Count;
            layers = new List<float[]>(layerCount);
            weights = new List<float[,]>(layerCount - 1);
            biases = new List<float[]>(layerCount - 1);
            for (int i = 0; i < layerCount; i++)
            {
                layers[i] = new float[layerSizes[i]];
                if (i < layerCount - 1)
                    weights[i] = new float[layerSizes[i], layerSizes[i + 1]];
            }
        }

        private readonly List<float[]> layers;
        private readonly List<float[,]> weights;
        private readonly List<float[]> biases;
    }
}
