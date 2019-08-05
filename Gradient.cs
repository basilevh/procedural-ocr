// 31-07-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Captures all adjustable parameters of a neural network.
    /// </summary>
    public class Gradient
    {
        public Gradient(List<int> layerSizes)
        {
            LayerSizes = new List<int>(layerSizes);
            LayerCount = layerSizes.Count;
            DeltaWeights = new List<float[,]>(LayerCount - 1);
            DeltaBiases = new List<float[]>(LayerCount - 1);
            for (int k = 0; k < LayerCount - 1; k++)
            {
                DeltaWeights.Add(new float[layerSizes[k], layerSizes[k + 1]]);
                DeltaBiases.Add(new float[layerSizes[k + 1]]);
            }
        }

        public List<int> LayerSizes { get; }

        public int LayerCount { get; }

        public List<float[,]> DeltaWeights { get; }

        public List<float[]> DeltaBiases { get; }

        /// <summary>
        /// Updates this gradient by adding all weights and biases of the incoming object to the existing values element-wise.
        /// </summary>
        public void AddWith(Gradient other)
        {
            for (int k = 0; k < LayerCount - 1; k++)
            {
                AddWith(DeltaWeights[k], other.DeltaWeights[k], 1.0f);
                AddWith(DeltaBiases[k], other.DeltaBiases[k], 1.0f);
            }
        }

        /// <summary>
        /// Applies this gradient to the given network, updating all incoming weights and biases by adding the gradient values to them
        /// (all gradient values are first multiplied with multFact).
        /// </summary>
        /// <param name="multFact">The learning rate (if the gradient is scaled correctly).</param>
        public void UpdateNetwork(NeuralNetwork network, float multFact)
        {
            for (int k = 0; k < LayerCount - 1; k++)
            {
                AddWith(network.Weights[k], DeltaWeights[k], multFact);
                AddWith(network.Biases[k], DeltaBiases[k], multFact);
            }
        }

        private static void AddWith(float[,] recipient, float[,] other, float multFact)
        {
            Parallel.For(0, recipient.GetLength(0), i =>
            // for (int i = 0; i < recipient.GetLength(0); i++)
            {
                for (int j = 0; j < recipient.GetLength(1); j++)
                {
                    recipient[i, j] += other[i, j] * multFact;
                }
            });
        }

        private static void AddWith(float[] recipient, float[] other, float multFact)
        {
            Parallel.For(0, recipient.Length, j =>
            // for (int j = 0; j < recipient.Length; j++)
            {
                recipient[j] += other[j] * multFact;
            });
        }
    }
}
