// 02-08-2019, BVH

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProceduralOCR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR.Tests
{
    [TestClass()]
    public class NeuralNetworkTests
    {
        [TestMethod()]
        public void NeuralNetworkTest()
        {
            var layerSizes = new List<int>() { 3, 5, 2 };
            var network = new NeuralNetwork(layerSizes);

            // Verify network architecture
            Assert.AreEqual(3, network.Layers.Count);
            Assert.AreEqual(3, network.Layers[0].Length);
            Assert.AreEqual(5, network.Layers[1].Length);
            Assert.AreEqual(2, network.Layers[2].Length);

            // Verify network parameters
            foreach (float[,] layerPair in network.Weights)
                foreach (float value in layerPair)
                    Assert.AreEqual(0.0f, value);
            foreach (float[] layer in network.Biases)
                foreach (float value in layer)
                    Assert.AreEqual(0.0f, value);

            // Call random initialization
            network.InitializeWeights(0.5);
            network.InitializeBiases(0.5);

            // Verify network parameters
            bool allZero = true;
            foreach (float[,] layerPair in network.Weights)
                foreach (float value in layerPair)
                    if (value != 0.0f)
                        allZero = false;
            Assert.IsFalse(allZero);
        }

        [TestMethod()]
        public void FeedForwardTest()
        {
            var layerSizes = new List<int>() { 3, 5, 2 };
            var network = new NeuralNetwork(layerSizes);
            fillNetwork(network);

            // Input layer
            float in0 = 0.1f;
            float in1 = 0.2f;
            float in2 = 0.3f;
            float[] input = { in0, in1, in2 };

            // Expected output layer
            float tmp0 = (float)Math.Tanh(0.1f + (in0 - in1 + in2) * 1.0f);
            float tmp1 = (float)Math.Tanh(0.2f + (in0 - in1 + in2) * 2.0f);
            float tmp2 = (float)Math.Tanh(0.3f + (in0 - in1 + in2) * 3.0f);
            float tmp3 = (float)Math.Tanh(0.5f + (in0 - in1 + in2) * 5.0f);
            float tmp4 = (float)Math.Tanh(0.7f + (in0 - in1 + in2) * 7.0f);
            float out0 = (float)Math.Tanh(0.1f + tmp0 + tmp1 + tmp2 + tmp3 + tmp4);
            float out1 = (float)Math.Tanh(0.2f + tmp0 + tmp1 + tmp2 + tmp3 + tmp4);

            // Verify
            float[] actualOutput = network.FeedForward(input);
            Assert.AreEqual(out0, actualOutput[0], 1e-6);
            Assert.AreEqual(out1, actualOutput[1], 1e-6);
        }

        [TestMethod()]
        public void GetTotalErrorTest()
        {
            var layerSizes = new List<int>() { 3, 5, 2 };
            var network = new NeuralNetwork(layerSizes);
            fillNetwork(network);

            // Input layer
            float in0 = 0.1f;
            float in1 = 0.2f;
            float in2 = 0.3f;
            float[] input = { in0, in1, in2 };

            // Output layer
            float tmp0 = (float)Math.Tanh(0.1f + (in0 - in1 + in2) * 1.0f);
            float tmp1 = (float)Math.Tanh(0.2f + (in0 - in1 + in2) * 2.0f);
            float tmp2 = (float)Math.Tanh(0.3f + (in0 - in1 + in2) * 3.0f);
            float tmp3 = (float)Math.Tanh(0.5f + (in0 - in1 + in2) * 5.0f);
            float tmp4 = (float)Math.Tanh(0.7f + (in0 - in1 + in2) * 7.0f);
            float out0 = (float)Math.Tanh(0.1f + tmp0 + tmp1 + tmp2 + tmp3 + tmp4);
            float out1 = (float)Math.Tanh(0.2f + tmp0 + tmp1 + tmp2 + tmp3 + tmp4);

            float totalError = network.GetTotalError(input, new float[] { 2.0f, -2.0f });
            Assert.AreEqual((out0 - 2.0) * (out0 - 2.0) + (out1 + 2.0) * (out1 + 2.0), totalError, 1e-6);
        }

        private void fillNetwork(NeuralNetwork network)
        {
            // Input layer to hidden layer
            network.Weights[0][0, 0] = 1.0f;
            network.Weights[0][0, 1] = 2.0f;
            network.Weights[0][0, 2] = 3.0f;
            network.Weights[0][0, 3] = 5.0f;
            network.Weights[0][0, 4] = 7.0f;
            network.Weights[0][1, 0] = -1.0f;
            network.Weights[0][1, 1] = -2.0f;
            network.Weights[0][1, 2] = -3.0f;
            network.Weights[0][1, 3] = -5.0f;
            network.Weights[0][1, 4] = -7.0f;
            network.Weights[0][2, 0] = 1.0f;
            network.Weights[0][2, 1] = 2.0f;
            network.Weights[0][2, 2] = 3.0f;
            network.Weights[0][2, 3] = 5.0f;
            network.Weights[0][2, 4] = 7.0f;
            network.Biases[0][0] = 0.1f;
            network.Biases[0][1] = 0.2f;
            network.Biases[0][2] = 0.3f;
            network.Biases[0][3] = 0.5f;
            network.Biases[0][4] = 0.7f;

            // Hidden layer to output layer
            network.Weights[1][0, 0] = 1.0f;
            network.Weights[1][0, 1] = 1.0f;
            network.Weights[1][1, 0] = 1.0f;
            network.Weights[1][1, 1] = 1.0f;
            network.Weights[1][2, 0] = 1.0f;
            network.Weights[1][2, 1] = 1.0f;
            network.Weights[1][3, 0] = 1.0f;
            network.Weights[1][3, 1] = 1.0f;
            network.Weights[1][4, 0] = 1.0f;
            network.Weights[1][4, 1] = 1.0f;
            network.Biases[1][0] = 0.1f;
            network.Biases[1][1] = 0.2f;
        }
    }
}