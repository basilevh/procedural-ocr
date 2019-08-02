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
        private const float learningRate = 0.35f; // Gradient multiplication factor
        // private const bool analytic = false;

        public NeuralNetworkTrainer(NeuralNetwork network)
        {
            Network = network;
        }

        public NeuralNetwork Network { get; }

        /// <summary>
        /// Adjusts the network's weights and biases using backpropagation to better match the given set of training examples.
        /// </summary>
        /// <param name="examples">Mini-batch of input-output pairs / training examples that indirectly dictates the gradient.</param>
        /// <returns>The training loss.</returns>
        public float SingleIteration(List<InputOutputPair> examples)
        {
            var total = new Gradient(Network.LayerSizes);
            float errorSum = 0.0f;
            foreach (var example in examples)
            {
                var current = GetGradient(example);
                total.AddWith(current.Item1);
                errorSum += current.Item2;
            }

            // Update network with the averaged gradient (incorporated in the learning rate argument)
            Network.ApplyGradient(total, learningRate / examples.Count);

            // Get mean error before network update
            float trainingLoss = errorSum / examples.Count;
            return trainingLoss;
        }

        /// <summary>
        /// Gets the raw gradient using backpropagation that would make the network better match the given training example.
        /// </summary>
        /// <param name="example">The input layer and desired output layer.</param>
        /// <returns>A tuple containing the gradient and the error before the parameter update.</returns>
        public Tuple<Gradient, float> GetGradient(InputOutputPair example)
        {
            float[] target = example.Output;
            float[] output = Network.FeedForward(example.Input);
            float error = Network.GetOutputError(output, target);

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

            return new Tuple<Gradient, float>(result, error);
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
