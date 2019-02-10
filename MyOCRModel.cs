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
            var layerSizes = new List<int>() { imageWidth * imageHeight, 32, 16, 10 };
            neuralNetwork = new NeuralNetwork(layerSizes);
            neuralNetwork.InitializeWeights(0.2);
            neuralNetwork.InitializeBiases(0.2);
        }

        private readonly int imageWidth, imageHeight;
        private readonly ICharacterGenerator characterGenerator;
        private readonly NeuralNetwork neuralNetwork;

        public TrainResult TrainModel(int samples)
        {
            var sample = characterGenerator.Generate();
            NeuralNetworkTrainer.TrainSingle(neuralNetwork, sample);

            /*const int batchSize = 64;
            for (int i = 0; i < samples; i += batchSize)
            {
                var batch = characterGenerator.GenerateMulti(batchSize);
                foreach (var sample in batch)
                {
                    NeuralNetworkTrainer.TrainSingle(neuralNetwork, sample);
                }
            }*/

            TrainResult result;
            return result;
        }

        public TestResult TestModel(int samples)
        {
            throw new NotImplementedException();
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
