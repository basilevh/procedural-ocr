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
            LayerSizes = new List<int>(layerSizes);
            LayerCount = layerSizes.Count;
            Layers = new List<float[]>(LayerCount);
            Weights = new List<float[,]>(LayerCount - 1);
            Biases = new List<float[]>(LayerCount - 1);
            for (int layer = 0; layer < LayerCount; layer++)
            {
                Layers.Add(new float[layerSizes[layer]]);
                if (layer < LayerCount - 1)
                {
                    Weights.Add(new float[layerSizes[layer], layerSizes[layer + 1]]);
                    Biases.Add(new float[layerSizes[layer + 1]]);
                }
            }
        }

        public List<int> LayerSizes { get; }

        public int LayerCount { get; }

        public List<float[]> Layers;

        public List<float[,]> Weights { get; }

        public List<float[]> Biases { get; }

        public void InitializeWeights(double stdDev)
        {
            // Initialize weights to random values
            for (int layer = 0; layer < LayerCount - 1; layer++)
            {
                for (int j = 0; j < LayerSizes[layer + 1]; j++)
                {
                    double[] allNoise = MyRandom.NextStdGaussian(LayerSizes[layer]);
                    for (int i = 0; i < LayerSizes[layer]; i++)
                    {
                        Weights[layer][i, j] = (float)(allNoise[i] * stdDev);
                    }
                }
            }
        }

        public void InitializeBiases(double stdDev)
        {
            // Initialize biases to random values
            for (int layer = 1; layer < LayerCount; layer++)
            {
                double[] allNoise = MyRandom.NextStdGaussian(LayerSizes[layer]);
                for (int j = 0; j < LayerSizes[layer]; j++)
                {
                    Biases[layer - 1][j] = (float)(allNoise[j] * stdDev);
                }
            }
        }

        public void CalculateLayer(int layer)
        {
            // Loop over all destination neurons in the current layer
            for (int j = 0; j < LayerSizes[layer]; j++)
            {
                // Apply bias
                float argument = Biases[layer - 1][j];
                // Sum all contributions of the previous layer
                for (int i = 0; i < LayerSizes[layer - 1]; i++)
                {
                    float source = Layers[layer - 1][i];
                    float weight = Weights[layer - 1][i, j];
                    argument += source * weight;
                }
                // Apply activation function (ReLU)
                argument = Math.Max(argument, 0.0f);
                // Assign current neuron
                Layers[layer][j] = argument;
            }
        }

        public float[] GetOutput(float[] input)
        {
            Layers[0] = input;
            for (int layer = 1; layer < LayerCount; layer++)
            {
                CalculateLayer(layer);
            }
            return Layers[LayerCount - 1];
        }

        public float GetTotalError(float[] input, float[] target)
        {
            float[] output = GetOutput(input);
            return output.Zip(target, (a, b) => (a - b) * (a - b)).Sum();
        }

        public void UpdateWeights(List<float[,]> delta)
        {
            for (int layer = 0; layer < LayerCount - 1; layer++)
            {
                for (int j = 0; j < LayerSizes[layer + 1]; j++)
                {
                    for (int i = 0; i < LayerSizes[layer]; i++)
                    {
                        Weights[layer][i, j] += delta[layer][i, j];
                    }
                }
            }
        }

        public void UpdateBiases(List<float[]> delta)
        {
            for (int layer = 1; layer < LayerCount; layer++)
            {
                for (int j = 0; j < LayerSizes[layer]; j++)
                {
                    Biases[layer - 1][j] += delta[layer - 1][j];
                }
            }
        }
    }
}
