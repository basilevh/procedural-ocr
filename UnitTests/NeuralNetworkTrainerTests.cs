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
    public class NeuralNetworkTrainerTests
    {
        [TestMethod()]
        public void SingleIterationTest()
        {
            // Initialize network
            var layerSizes = new List<int>() { 1, 1 };
            var network = new NeuralNetwork(layerSizes);
            network.InitializeWeights(0.5);
            network.InitializeBiases(0.5);

            // Initialize trainer
            var trainer = new BackpropNetworkTrainer(network);
            var example0 = new InputOutputPair(new float[] { 0.0f }, new float[] { -0.3f });
            var example1 = new InputOutputPair(new float[] { 1.0f }, new float[] { 0.6f });
            var examples = new List<InputOutputPair>() { example0, example1 };
            const float trueWeight = 1.002666784763057f;
            const float trueBias = -0.30951960420311175f;

            // Track progress of weight and bias
            float weight0 = network.Weights[0][0, 0];
            float bias0 = network.Biases[0][0];
            const int iterations = 50;
            float[] weights = new float[iterations];
            float[] biases = new float[iterations];
            for (int i = 0; i < iterations; i++)
            {
                trainer.SingleIteration(examples);
                weights[i] = network.Weights[0][0, 0];
                biases[i] = network.Biases[0][0];
                Console.WriteLine("weight: " + weights[i] + " bias: " + biases[i]);

                // Verify eventual convergence of network parameters
                if (i > iterations / 2)
                { 
                    Assert.IsTrue(Math.Abs(weights[i] - trueWeight) < Math.Abs(weights[i - 1] - trueWeight));
                    Assert.IsTrue(Math.Abs(biases[i] - trueBias) < Math.Abs(biases[i - 1] - trueBias));
                }
            }
        }
    }
}