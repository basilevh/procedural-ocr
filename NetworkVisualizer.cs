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
    /// <summary>
    /// Utility class to visualize the execution and testing of neural networks.
    /// </summary>
    public static class NetworkVisualizer
    {
        private const int neuronSize = 4;
        private const int neuronPadding = 1;
        private const int neuronFullSize = neuronSize + neuronPadding;
        private const int sectionPadding = 8;

        /// <summary>
        /// Visualizes the full neural network for the most recently executed input.
        /// </summary>
        /// <param name="network">The neural network to draw.</param>
        /// <param name="input">Input image that was fed into the network.</param>
        /// <returns>An object that can be assigned directly to an ImageControl's Source property.</returns>
        public static ImageSource DrawNetworkSingle(NeuralNetwork network, float[,] input)
        {
            int inputWidth = input.GetLength(0);
            int inputHeight = input.GetLength(1);
            const int inputScaleFact = 2;
            int networkMaxNeurons = network.LayerSizes.Skip(1).Max();
            int lastWeightsCount = network.LayerSizes[network.LayerCount - 2];
            int lastWeightsWidth = (network.LayerCount == 2 ? inputWidth : lastWeightsCount);
            int lastWeightsHeight = lastWeightsCount / lastWeightsWidth;
            int width = /*inputWidth * inputScaleFact +
                sectionPadding +*/ (network.LayerCount - 1) * neuronFullSize +
                sectionPadding + lastWeightsWidth;
            int height = Math.Max(Math.Max(inputHeight * inputScaleFact,
                networkMaxNeurons * neuronFullSize),
                network.LayerSizes.Last() * (lastWeightsHeight + neuronPadding));
            var output = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Rgb24, null);
            byte[] bytes = new byte[width * height * 3];
            int baseX = 0;

            // Draw input image, scaled by a factor 2
            /*drawInput(input, bytes, inputScaleFact, baseX, 0, width);
            baseX += inputWidth * inputScaleFact + sectionPadding;*/

            // Draw neuron values (except input)
            for (int k = 1; k < network.LayerCount; k++)
            {
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
                baseX += neuronFullSize;
            }
            baseX += sectionPadding;

            // Draw weights of last two layers
            var lastWeights = network.Weights.Last();
            for (int j = 0; j < network.LayerSizes[network.LayerCount - 1]; j++)
            {
                int baseY = j * (lastWeightsHeight + neuronPadding);
                for (int i = 0; i < network.LayerSizes[network.LayerCount - 2]; i++)
                {
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

        /// <summary>
        /// Visualizes a few examples and their classification outcome to get an overall impression of the network's accuracy.
        /// </summary>
        /// <param name="network">The neural network to test.</param>
        /// <param name="characterSource">The character source to generate samples from.</param>
        /// <param name="rows">Amount of examples to show vertically.</param>
        /// <param name="columns">Amount of examples to show horizontally.</param>
        /// <returns>An object that can be assigned directly to an ImageControl's Source property.</returns>
        public static ImageSource DrawTestGallery(IOCRModel ocrModel, ICharacterSource characterSource, int rows, int columns)
        {
            const int inputScaleFact = 2;
            const int labelHeight = 16;
            int width = (characterSource.ImageWidth * inputScaleFact + sectionPadding) * columns;
            int height = (characterSource.ImageHeight * inputScaleFact + labelHeight + sectionPadding) * rows;
            var output = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Rgb24, null);
            byte[] bytes = new byte[width * height * 3];
            var samples = characterSource.GenerateMulti(rows * columns);
            int baseY = 0;
            for (int i = 0; i < rows; i++)
            {
                int baseX = 0;
                for (int j = 0; j < columns; j++)
                {
                    var sample = samples[i * columns + j];
                    drawInput(sample.Image, bytes, inputScaleFact, baseX, baseY, width);
                    var result = ocrModel.ExecuteSingle(sample.Image);
                    bool correct = (result.MostConfident == sample.Character);
                    for (int y = baseY + characterSource.ImageHeight * inputScaleFact + labelHeight / 4;
                        y < baseY + characterSource.ImageHeight * inputScaleFact + labelHeight / 2; y++)
                    {
                        for (int x = baseX; x < baseX + characterSource.ImageWidth * inputScaleFact; x++)
                        {
                            bytes[(x + y * width) * 3] = (byte)(correct ? 0 : 255);
                            bytes[(x + y * width) * 3 + 1] = (byte)(correct ? 255 : 0);
                            bytes[(x + y * width) * 3 + 2] = (byte)0;
                        }
                    }
                    baseX += characterSource.ImageWidth * inputScaleFact + sectionPadding;
                }
                baseY += characterSource.ImageHeight * inputScaleFact + labelHeight + sectionPadding;
            }

            output.WritePixels(new Int32Rect(0, 0, width, height), bytes, width * 3, 0);
            return output;
        }

        private static void drawInput(float[,] source, byte[] dest, int inputScaleFact, int destX, int destY, int destFullWidth)
        {
            int inputWidth = source.GetLength(0);
            int inputHeight = source.GetLength(1);
            for (int srcY = 0; srcY < inputHeight; srcY++)
            {
                for (int srcX = 0; srcX < inputWidth; srcX++)
                {
                    byte value = (byte)Truncate255(source[srcX, srcY] * 256.0f);
                    for (int y = srcY * inputScaleFact + destY; y < srcY * inputScaleFact + inputScaleFact + destY; y++)
                    {
                        for (int x = srcX * inputScaleFact + destX; x < srcX * inputScaleFact + inputScaleFact + destX; x++)
                        {
                            dest[(x + y * destFullWidth) * 3] = value;
                            dest[(x + y * destFullWidth) * 3 + 1] = value;
                            dest[(x + y * destFullWidth) * 3 + 2] = value;
                        }
                    }
                }
            }
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
