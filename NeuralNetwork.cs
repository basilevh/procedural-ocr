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
            WeightedSums = new List<float[]>(LayerCount);
            Weights = new List<float[,]>(LayerCount - 1);
            Biases = new List<float[]>(LayerCount - 1);
            for (int k = 0; k < LayerCount; k++)
            {
                Layers.Add(new float[layerSizes[k]]);
                WeightedSums.Add(new float[layerSizes[k]]);
                if (k < LayerCount - 1)
                {
                    Weights.Add(new float[layerSizes[k], layerSizes[k + 1]]);
                    Biases.Add(new float[layerSizes[k + 1]]);
                }
            }
        }

        public List<int> LayerSizes { get; }

        public int LayerCount { get; }

        /// <summary>
        /// The most recent neuron values for every layer.
        /// The first index k accesses different layers (columns),
        /// the second index i accesses different neurons (rows) within that layer.
        /// </summary>
        public List<float[]> Layers;

        /// <summary>
        /// The most recent neuron values for every layer, before applying the activation function.
        /// </summary>
        public List<float[]> WeightedSums;

        /// <summary>
        /// The network weights from layer k to layer k+1.
        /// The first index k accesses different layer pairs (columns),
        /// the second index i accesses source neurons (left layer),
        /// the third index j accesses destination neurons (right layer).
        /// Note: the index order is opposite to mathematical convention.
        /// </summary>
        public List<float[,]> Weights { get; }

        /// <summary>
        /// The network biases for every layer.
        /// The first index k accesses different layers (columns) excluding the input layer,
        /// the second index j accesses different neurons (rows) within that layer.
        /// </summary>
        public List<float[]> Biases { get; }

        public void InitializeWeights(double stdDev)
        {
            // Initialize weights to random values
            for (int k = 0; k < LayerCount - 1; k++)
            {
                for (int j = 0; j < LayerSizes[k + 1]; j++)
                {
                    double[] allNoise = MyRandom.NextStdGaussian(LayerSizes[k]);
                    for (int i = 0; i < LayerSizes[k]; i++)
                    {
                        Weights[k][i, j] = (float)(allNoise[i] * stdDev);
                    }
                }
            }
        }

        public void InitializeBiases(double stdDev)
        {
            // Initialize biases to random values
            for (int k = 1; k < LayerCount; k++)
            {
                double[] allNoise = MyRandom.NextStdGaussian(LayerSizes[k]);
                for (int j = 0; j < LayerSizes[k]; j++)
                {
                    Biases[k - 1][j] = (float)(allNoise[j] * stdDev);
                }
            }
        }

        public void CalculateLayer(int k)
        {
            // Loop over all destination neurons in the current layer
            Parallel.For(0, LayerSizes[k], j =>
            {
                // Apply bias
                float argument = Biases[k - 1][j];

                // Sum all contributions of the previous layer
                for (int i = 0; i < LayerSizes[k - 1]; i++)
                {
                    float source = Layers[k - 1][i];
                    float weight = Weights[k - 1][i, j];
                    argument += source * weight;
                }

                // Assign pre-activation function neuron
                WeightedSums[k][j] = argument;

                // Apply activation function and assign final neuron
                Layers[k][j] = ActivationFunction(argument);
            });
        }

        public float[] FeedForward(float[] input)
        {
            Layers[0] = WeightedSums[0] = input;
            for (int k = 1; k < LayerCount; k++)
            {
                CalculateLayer(k);
            }
            return Layers[LayerCount - 1];
        }

        public float GetTotalError(float[] input, float[] target)
        {
            float[] output = FeedForward(input);
            return GetOutputError(output, target);
        }

        public float GetOutputError(float[] output, float[] target)
        {
            return output.Zip(target, (a, b) => (a - b) * (a - b)).Sum();
        }

        /// <summary>
        /// Updates this network to incorporate the given gradient, adding all incoming weights and biases to the existing values
        /// (after first multiplying them with multFact).
        /// </summary>
        /// <param name="multFact">The learning rate (if the gradient is scaled correctly).</param>
        public void ApplyGradient(Gradient gradient, float multFact)
        {
            gradient.UpdateNetwork(this, multFact);
        }

        /// <summary>
        /// Applies the rectified linear unit (ReLU) activation function to a single input number.
        /// </summary>
        public float ActivationFunction(float argument)
        {
            // return Math.Max(argument, 0.0f);
            return (float)(1.0 / (1.0 + Math.Exp(-argument)));
            // return (float)Math.Tanh(argument);
        }

        /// <summary>
        /// Applies the derivative of the rectified linear unit (ReLU) activation function to a single input number.
        /// The result is a Heaviside step function.
        /// </summary>
        public float ActivationFunctionDeriv(float argument)
        {
            // return (argument > 0.0f ? 1.0f : 0.0f);
            return ActivationFunction(argument) * (1.0f - ActivationFunction(argument));
            // return (float)(1.0 - Math.Pow(Math.Tanh(argument), 2.0));
        }
    }
}
