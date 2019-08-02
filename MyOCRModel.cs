// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    public class MyOCRModel : IOCRModel
    {
        public MyOCRModel(int imageWidth, int imageHeight, ICharacterGenerator characterGenerator)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            this.characterGenerator = characterGenerator;
            // var layerSizes = new List<int>() { imageWidth * imageHeight, 20, 10, 10 };
            var layerSizes = new List<int>() { imageWidth * imageHeight, 20, 10 };
            // var layerSizes = new List<int>() { imageWidth * imageHeight, 10 };
            neuralNetwork = new NeuralNetwork(layerSizes);
            neuralNetwork.InitializeWeights(0.2);
            neuralNetwork.InitializeBiases(0.2);
            networkTrainer = new NeuralNetworkTrainer(neuralNetwork);
        }

        private readonly int imageWidth, imageHeight;
        private readonly ICharacterGenerator characterGenerator;
        private readonly NeuralNetwork neuralNetwork;
        private readonly NeuralNetworkTrainer networkTrainer;

        public TrainResult TrainModel(int batches, int batchSize, int itersPerBatch)
        {
            float[] batchErrors = new float[batches];
            for (int b = 0; b < batches; b++)
            {
                // Convert labeled characters to more general input-output pairs
                var examples = characterGenerator.GenerateMulti(batchSize).Select(
                    lc => new InputOutputPair(lc.NetworkInput, Enumerable.Range(0, 10).Select(
                        i => (i + '0') == lc.Character ? 1.0f : 0.0f).ToArray())).ToList();

                // Perform multiple gradient descent steps per batch
                float[] iterErrors = new float[itersPerBatch];
                for (int i = 0; i < itersPerBatch; i++)
                { 
                    iterErrors[i] = networkTrainer.SingleIteration(examples);
                    if (i > 0 && Math.Abs(iterErrors[i] - iterErrors[i - 1]) / iterErrors[i] < 1e-6)
                        // Barely any improvement
                        break;
                }
                batchErrors[b] = iterErrors.Where(e => e != 0.0f).Average();
            }

            var result = new TrainResult(batchErrors);
            return result;
        }

        public TestResult TestModel(int samples)
        {
            TestResult result = new TestResult();
            var elements = characterGenerator.GenerateMulti(samples);
            foreach (var element in elements)
            {
                var current = ExecuteSingle(element.Image);
                if (current.MostConfident == element.Character)
                    result.Correct++;
                result.Tested++;
            }
            return result;
        }

        public SingleResult ExecuteSingle(float[,] input)
        {
            float[] output = neuralNetwork.FeedForward(MyBitmapTools.ReshapeArray(input));
            return new SingleResult(CreateDictFromProbs(output));
        }

        private static IDictionary<char, float> CreateDictFromProbs(float[] probabilities)
        {
            var result = new Dictionary<char, float>();
            for (int i = 0; i <= 9; i++)
            {
                char c = (char)(i + '0');
                result[c] = probabilities[i];
            }
            return result;
        }
    }
}
