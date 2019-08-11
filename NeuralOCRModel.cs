// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Digit recognizer based on a deep neural network.
    /// </summary>
    public class NeuralOCRModel : IOCRModel
    {
        public NeuralOCRModel(ICharacterSource characterGenerator, List<int> hiddenLayerSizes)
        {
            ImageWidth = characterGenerator.ImageWidth;
            ImageHeight = characterGenerator.ImageHeight;
            this.characterGenerator = characterGenerator;
            var layerSizes = new List<int>();
            layerSizes.Add(ImageWidth * ImageHeight); // input layer
            layerSizes.AddRange(hiddenLayerSizes);
            layerSizes.Add(10); // output layer
            NeuralNetwork = new NeuralNetwork(layerSizes);
            NeuralNetwork.InitializeWeights(0.2);
            NeuralNetwork.InitializeBiases(0.2);
        }

        public NeuralOCRModel(ICharacterSource characterGenerator)
            : this(characterGenerator, new List<int>() { 32, 16 })
        { }

        private readonly ICharacterSource characterGenerator;

        public int ImageWidth { get; }

        public int ImageHeight { get; }

        public NeuralNetwork NeuralNetwork { get; }

        /// <summary>
        /// Trains the model using backpropagation on the neural network.
        /// </summary>
        /// <param name="batches">The amount of mini-batches to process in this method call.</param>
        /// <param name="batchSize">The amount of samples (images) one mini-batch contains.</param>
        /// <param name="itersPerBatch">The amount of gradient descent steps performed per mini-batch.</param>
        /// <param name="learningRate">The factor the gradient is multiplied with before updating the neural network.</param>
        /// <returns>A training summary of how the mini-batch errors evolve over time.</returns>
        public TrainResult TrainModel(int batches, int batchSize, int itersPerBatch, float learningRate)
        {
            INetworkTrainer networkTrainer = new BackpropNetworkTrainer(NeuralNetwork, learningRate);
            int totalSamples = 0;
            double[] batchErrors = new double[batches];
            for (int b = 0; b < batches; b++)
            {
                // Convert labeled characters to more general input-output pairs
                var examples = characterGenerator.GenerateMulti(batchSize).Select(
                    lc => new InputOutputPair(lc.NetworkInput, Enumerable.Range(0, 10).Select(
                        i => (i + '0') == lc.Character ? 1.0f : 0.0f).ToArray())).ToList();

                // Perform multiple gradient descent steps per batch
                double[] iterErrors = new double[itersPerBatch];
                for (int i = 0; i < itersPerBatch; i++)
                {
                    totalSamples += batchSize;
                    iterErrors[i] = networkTrainer.SingleIteration(examples);
                    if (i > 0 && Math.Abs(iterErrors[i] - iterErrors[i - 1]) / iterErrors[i] < 1e-6)
                        // Barely any improvement
                        break;
                }
                batchErrors[b] = iterErrors.Where(e => e != 0.0f).Average();
            }
            var result = new TrainResult(totalSamples, batchErrors);
            return result;
        }

        public TestResult TestModel(int samples)
        {
            var elements = characterGenerator.GenerateMulti(samples);
            int[] tested = new int[10];
            int[] correct = new int[10];
            foreach (var element in elements)
            {
                var current = ExecuteSingle(element.Image);
                int index = element.Character - '0';
                if (current.MostConfident == element.Character)
                    correct[index]++;
                tested[index]++;
            }
            var result = new TestResult(tested, correct);
            return result;
        }

        public SingleResult ExecuteSingle(float[,] input)
        {
            float[] output = NeuralNetwork.FeedForward(MyBitmapTools.ReshapeArray(input));
            return new SingleResult(CreateDictFromProbs(output));
        }

        private static IDictionary<char, float> CreateDictFromProbs(float[] probabilities)
        {
            var result = new Dictionary<char, float>();
            float sum = probabilities.Sum();
            for (int i = 0; i <= 9; i++)
            {
                char c = (char)(i + '0');
                result[c] = probabilities[i] / sum;
            }
            return result;
        }
    }
}
