// 10-02-2019, BVH

#pragma warning disable CS0162 // Unreachable code detected

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    public static class NeuralNetworkTrainer
    {
        private const float delta = 0.01f; // Gradient multiplication factor
        private const bool analytic = false;

        public static void TrainSingle(NeuralNetwork network, LabeledCharacter element)
        {
            float[] input = MyBitmapTools.ReshapeArray(element.Image);
            float[] target = new float[10];
            for (int i = 0; i < 10; i++)
            {
                target[i] = (i == element.Character - '0' ? 1.0f : 0.0f);
            }

            // Initialize deltas
            List<float[,]> deltaWeights = new List<float[,]>(network.LayerCount - 1);
            List<float[]> deltaBiases = new List<float[]>(network.LayerCount - 1);
            for (int layer = 0; layer < network.LayerCount - 1; layer++)
            {
                deltaWeights.Add(new float[network.LayerSizes[layer], network.LayerSizes[layer + 1]]);
                deltaBiases.Add(new float[network.LayerSizes[layer + 1]]);
            }

            // Get gradient for all weights, starting from the last layer
            for (int layer = network.LayerCount - 2; layer >= 0; layer--)
            {
                for (int j = 0; j < network.LayerSizes[layer + 1]; j++)
                {
                    for (int i = 0; i < network.LayerSizes[layer]; i++)
                    {
                        // Get the partial derivative of the total error with respect to this weight
                        float partDeriv;
                        if (analytic)
                        {
                            partDeriv = getPartDerivWeightAnalytic(network, input, target, layer, i, j);
                        }
                        else
                        {
                            partDeriv = getPartDerivWeightNumeric(network, input, target, layer, i, j);
                        }
                        deltaWeights[layer][i, j] = -partDeriv * delta;
                    }
                }
            }

            // Get gradient for all biases
            for (int layer = network.LayerCount - 1; layer >= 1; layer--)
            {
                for (int j = 0; j < network.LayerSizes[layer]; j++)
                {
                    // Get the partial derivative of the total error with respect to this bias
                    float partDeriv;
                    if (analytic)
                    {
                        partDeriv = getPartDerivBiasAnalytic(network, input, target, layer, j);
                    }
                    else
                    {
                        partDeriv = getPartDerivBiasNumeric(network, input, target, layer, j);
                    }
                    deltaBiases[layer - 1][j] = -partDeriv * delta;

                }
            }

            // Update network
            network.UpdateWeights(deltaWeights);
            network.UpdateBiases(deltaBiases);
        }

        private static float getPartDerivWeightAnalytic(NeuralNetwork network,
            float[] input, float[] target, int layer, int i, int j)
        {
            // TODO
            return 0.0f;
        }

        private static float getPartDerivWeightNumeric(NeuralNetwork network,
            float[] input, float[] target, int layer, int i, int j)
        {
            float step = 0.001f;
            float orig = network.Weights[layer][i, j];
            float x1 = orig - step / 2.0f;
            float x2 = orig + step / 2.0f;
            network.Weights[layer][i, j] = x1;
            float y1 = network.GetTotalError(input, target);
            network.Weights[layer][i, j] = x2;
            float y2 = network.GetTotalError(input, target);
            float result = (y2 - y1) / (x2 - x1);
            return result;
        }

        private static float getPartDerivBiasAnalytic(NeuralNetwork network,
            float[] input, float[] target, int layer, int j)
        {
            // TODO
            return 0.0f;
        }

        private static float getPartDerivBiasNumeric(NeuralNetwork network,
            float[] input, float[] target, int layer, int j)
        {
            float step = 0.001f;
            float orig = network.Biases[layer - 1][j];
            float x1 = orig - step / 2.0f;
            float x2 = orig + step / 2.0f;
            network.Biases[layer - 1][j] = x1;
            float y1 = network.GetTotalError(input, target);
            network.Biases[layer - 1][j] = x2;
            float y2 = network.GetTotalError(input, target);
            float result = (y2 - y1) / (x2 - x1);
            return result;
        }
    }
}
