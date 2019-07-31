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
            var layerSizes = new List<int>() { imageWidth * imageHeight, 10 };
            neuralNetwork = new NeuralNetwork(layerSizes);
            neuralNetwork.InitializeWeights(0.2);
            neuralNetwork.InitializeBiases(0.2);
            networkTrainer = new NeuralNetworkTrainer(neuralNetwork);
        }

        private readonly int imageWidth, imageHeight;
        private readonly ICharacterGenerator characterGenerator;
        private readonly NeuralNetwork neuralNetwork;
        private readonly NeuralNetworkTrainer networkTrainer;

        public TrainResult TrainModel(int batches, int batchSize)
        {
            for (int b = 0; b < batches; b++)
            {
                var elements = characterGenerator.GenerateMulti(batchSize);
                networkTrainer.SingleIteration(elements);
            }

            TrainResult result;
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
            float[] output = neuralNetwork.GetOutput(MyBitmapTools.ReshapeArray(input));
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
