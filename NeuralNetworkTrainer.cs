// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Object that trains a single given neural network.
    /// </summary>
    public class NeuralNetworkTrainer
    {
        private const float learningRate = 0.20f; // Gradient multiplication factor
        // private const bool analytic = false;

        public NeuralNetworkTrainer(NeuralNetwork network)
        {
            Network = network;
        }

        public NeuralNetwork Network { get; }

        /// <summary>
        /// Adjusts the network's weights and biases to better match the given set of labeled characters.
        /// </summary>
        /// <param name="elements">Mini-batch of input-output pairs / training examples that indirectly dictates the gradient.</param>
        public void SingleIteration(List<LabeledCharacter> elements)
        {
            var total = new Gradient(Network.LayerSizes);
            foreach (var element in elements)
            {
                var current = GetGradient(element);
                total.AddWith(current);
            }
            // Update network with the averaged gradient (incorporated in the learning rate argument)
            total.UpdateNetwork(Network, learningRate / elements.Count);
        }

        /// <summary>
        /// Gets the raw gradient that would make the network better match the given labeled character.
        /// </summary>
        public Gradient GetGradient(LabeledCharacter element)
        {
            float[] input = element.NetworkInput;
            float[] target = new float[10]; // desired output
            float[] output = Network.GetOutput(input); // actual output; sets the layer arrays
            for (int i = 0; i < 10; i++)
            {
                target[i] = (i == element.Character - '0' ? 1.0f : 0.0f);
            }

            // Initialize gradient
            var result = new Gradient(Network.LayerSizes);

            // Get delta weights, starting from the last layer
            float[] desiredChange = target.Zip(output, (x, y) => x - y).ToArray(); // the right layer of the current stage
            for (int k = Network.LayerCount - 2; k >= 0; k--)
            {
                float[] nextDesiredChange = new float[Network.LayerSizes[k]];
                Parallel.For(0, Network.LayerSizes[k + 1], j =>
                {
                    // Apply the negative paterial derivative of the total error with respect to this bias
                    result.DeltaBiases[k][j] = desiredChange[j] * Network.ActivationFunctionDeriv(Network.WeightedSums[k + 1][j]);
                    for (int i = 0; i < Network.LayerSizes[k]; i++)
                    {
                        // Apply the negative partial derivative of the total error with respect to this weight
                        result.DeltaWeights[k][i, j] = Network.Layers[k][i] * result.DeltaBiases[k][j];

                        // Update the negative partial derivative of the total error with respect to this neuron value
                        nextDesiredChange[i] += Network.Weights[k][i, j] * result.DeltaBiases[k][j];
                    }
                });

                // Update desired change (one step to the left)
                desiredChange = nextDesiredChange;
            }

            return result;
        }

        // These methods do not work (likely far too much noise):

        /*private float getPartDerivWeightNumeric(float[] input, float[] target, int k, int i, int j)
        {
            float step = 0.001f;
            float orig = Network.Weights[k][i, j];
            float x1 = orig - step / 2.0f;
            float x2 = orig + step / 2.0f;
            Network.Weights[k][i, j] = x1;
            float y1 = Network.GetTotalError(input, target);
            Network.Weights[k][i, j] = x2;
            float y2 = Network.GetTotalError(input, target);
            float result = (y2 - y1) / (x2 - x1);
            return result;
        }

        private float getPartDerivBiasNumeric(float[] input, float[] target, int k, int j)
        {
            float step = 0.001f;
            float orig = Network.Biases[k - 1][j];
            float x1 = orig - step / 2.0f;
            float x2 = orig + step / 2.0f;
            Network.Biases[k - 1][j] = x1;
            float y1 = Network.GetTotalError(input, target);
            Network.Biases[k - 1][j] = x2;
            float y2 = Network.GetTotalError(input, target);
            float result = (y2 - y1) / (x2 - x1);
            return result;
        }*/
    }
}
