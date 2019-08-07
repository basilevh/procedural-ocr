// 05-08-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProceduralOCR
{
    public static class NetworkVisualizer
    {
        private const int neuronSize = 4;
        private const int neuronPadding = 1;
        private const int neuronFullSize = neuronSize + neuronPadding;
        private const int sectionPadding = 8;

        public static ImageSource DrawNeurons(NeuralNetwork network, float[,] input)
        {
            int inputWidth = input.GetLength(0);
            int inputHeight = input.GetLength(1);
            const int inputScaleFact = 2;
            int networkMaxNeurons = network.LayerSizes.Skip(1).Max();
            int lastWeightsCount = network.LayerSizes[network.LayerCount - 2];
            int lastWeightsWidth = (network.LayerCount == 2 ? inputWidth : lastWeightsCount);
            int lastWeightsHeight = lastWeightsCount / lastWeightsWidth;
            int width = inputWidth * inputScaleFact +
                sectionPadding + (network.LayerCount - 1) * neuronFullSize +
                sectionPadding + lastWeightsWidth;
            int height = Math.Max(Math.Max(inputHeight * inputScaleFact,
                networkMaxNeurons * neuronFullSize),
                network.LayerSizes.Last() * (lastWeightsHeight + neuronPadding));
            var output = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Rgb24, null);
            byte[] bytes = new byte[width * height * 3];

            // Draw input image, scaled by a factor 2
            for (int inY = 0; inY < inputHeight; inY++)
            {
                for (int inX = 0; inX < inputWidth; inX++)
                {
                    byte value = (byte)Truncate255(input[inX, inY] * 256.0f);
                    for (int y = inY * inputScaleFact; y < inY * inputScaleFact + inputScaleFact; y++)
                    {
                        for (int x = inX * inputScaleFact; x < inX * inputScaleFact + inputScaleFact; x++)
                        {
                            bytes[(x + y * width) * 3] = value;
                            bytes[(x + y * width) * 3 + 1] = value;
                            bytes[(x + y * width) * 3 + 2] = value;
                        }
                    }
                }
            }

            // Draw neuron values (except input)
            for (int k = 1; k < network.LayerCount; k++)
            {
                int baseX = inputWidth * inputScaleFact + sectionPadding + (k - 1) * neuronFullSize;
                for (int i = 0; i < network.LayerSizes[k]; i++)
                {
                    int baseY = i * neuronFullSize;
                    float value = Truncate1(network.Layers[k][i]); // force in interval [0, 1]
                    byte r = (byte)Truncate255(128.0f + value * 128.0f);
                    byte g = (byte)Truncate255(value * 256.0f);
                    byte b = (byte)Truncate255(value * 64.0f);
                    for (int y = baseY; y < baseY + neuronSize; y++)
                    {
                        for (int x = baseX; x < baseX + neuronSize; x++)
                        {
                            bytes[(x + y * width) * 3] = r;
                            bytes[(x + y * width) * 3 + 1] = g;
                            bytes[(x + y * width) * 3 + 2] = b;
                        }
                    }
                }
            }

            // Draw weights of last two layers
            var lastWeights = network.Weights.Last();
            for (int j = 0; j < network.LayerSizes[network.LayerCount - 1]; j++)
            {
                int baseY = j * (lastWeightsHeight + neuronPadding);
                for (int i = 0; i < network.LayerSizes[network.LayerCount - 2]; i++)
                {
                    int baseX = inputWidth * inputScaleFact +
                        sectionPadding + (network.LayerCount - 1) * neuronFullSize +
                        sectionPadding;
                    int y = baseY + (i / lastWeightsWidth);
                    int x = baseX + (i % lastWeightsWidth);
                    float value = Truncate1((lastWeights[i, j] + 1.0f) / 2.0f); // force in interval [0, 1]
                    byte r = (byte)Truncate255(128.0f + value * 128.0f);
                    byte g = (byte)Truncate255(value * 256.0f);
                    byte b = (byte)Truncate255(value * 64.0f);
                    bytes[(x + y * width) * 3] = r;
                    bytes[(x + y * width) * 3 + 1] = g;
                    bytes[(x + y * width) * 3 + 2] = b;
                }
            }

            output.WritePixels(new Int32Rect(0, 0, width, height), bytes, width * 3, 0);
            return output;
        }

        private static float Truncate1(float value)
        {
            return Math.Min(Math.Max(value, 0.0f), 1.0f);
        }

        private static float Truncate255(float value)
        {
            return Math.Min(Math.Max(value, 0.0f), 255.0f);
        }
    }
}
