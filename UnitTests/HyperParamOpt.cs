// 07-08-2019, BVH

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProceduralOCR.Tests
{
    /// <summary>
    /// Class for optimizing hyperparameters of the neural network OCR model (not really a test)
    /// </summary>
    [TestClass]
    public class HyperParamOpt
    {
        [TestMethod]
        public void OutputTest()
        {
            Console.WriteLine("7331");
        }

        [TestMethod]
        public void MainPass1()
        {
            // Iterations per batch
            ICharacterGenerator characterGenerator = new MyCharacterGenerator(24, 24);
            Console.WriteLine("{0,18} {1,18} {2,18} {3,18}", "Iters per batch", "Total samples", "Avg train loss", "Accuracy [%]");
            for (int itersPerBatch = 1; itersPerBatch <= 256; itersPerBatch *= 2)
            {
                var hiddenLayerSizes = new List<int>() { 32, 16 };
                IOCRModel ocrModel = new MyOCRModel(characterGenerator, hiddenLayerSizes);
                int batchSize = 4;
                int batches = (65536 / (itersPerBatch * batchSize));
                float learningRate = 0.20f;
                var trainRes = ocrModel.TrainModel(batches, batchSize, itersPerBatch, learningRate);
                var testRes = ocrModel.TestModel(10000);
                Console.WriteLine("{0,18} {1,18} {2,18:N4} {3,18:N2}",
                    itersPerBatch, trainRes.TotalSamples, trainRes.BatchErrors.Average(), testRes.Accuracy * 100.0);
            }
        }

        [TestMethod]
        public void MainPass2()
        {
            // Learning rate
            ICharacterGenerator characterGenerator = new MyCharacterGenerator(24, 24);
            Console.WriteLine("{0,18} {1,18} {2,18} {3,18}", "Learning rate", "Total samples", "Avg train loss", "Accuracy [%]");
            for (int i = 1; i <= 40; i++)
            {
                var hiddenLayerSizes = new List<int>() { 32, 16 };
                IOCRModel ocrModel = new MyOCRModel(characterGenerator, hiddenLayerSizes);
                int batches = 16384;
                int batchSize = 4;
                int itersPerBatch = 1;
                float learningRate = i * 0.02f;
                var trainRes = ocrModel.TrainModel(batches, batchSize, itersPerBatch, learningRate);
                var testRes = ocrModel.TestModel(10000);
                Console.WriteLine("{0,18:N2} {1,18} {2,18:N4} {3,18:N2}",
                    learningRate, trainRes.TotalSamples, trainRes.BatchErrors.Average(), testRes.Accuracy * 100.0);
            }
        }

        [TestMethod]
        public void MainPass3()
        {
            // Batch size
            ICharacterGenerator characterGenerator = new MyCharacterGenerator(24, 24);
            Console.WriteLine("{0,18} {1,18} {2,18} {3,18}", "Batch size", "Total samples", "Avg train loss", "Accuracy [%]");
            for (int batchSize = 1; batchSize <= 20; batchSize++)
            {
                var hiddenLayerSizes = new List<int>() { 32, 16 };
                IOCRModel ocrModel = new MyOCRModel(characterGenerator, hiddenLayerSizes);
                int batches = 65536 / batchSize;
                int itersPerBatch = 1;
                float learningRate = 0.40f;
                var trainRes = ocrModel.TrainModel(batches, batchSize, itersPerBatch, learningRate);
                var testRes = ocrModel.TestModel(10000);
                Console.WriteLine("{0,18:N2} {1,18} {2,18:N4} {3,18:N2}",
                    batchSize, trainRes.TotalSamples, trainRes.BatchErrors.Average(), testRes.Accuracy * 100.0);
            }
        }

        [TestMethod]
        public void MainPass4()
        {
            // First hidden layer size (1 HL)
            ICharacterGenerator characterGenerator = new MyCharacterGenerator(24, 24);
            Console.WriteLine("{0,18} {1,18} {2,18} {3,18}", "HL1 size", "Total samples", "Avg train loss", "Accuracy [%]");
            for (int i = 10; i <= 60; i += 2)
            {
                var hiddenLayerSizes = new List<int>() { i };
                IOCRModel ocrModel = new MyOCRModel(characterGenerator, hiddenLayerSizes);
                int batches = 16384;
                int batchSize = 4;
                int itersPerBatch = 1;
                float learningRate = 0.40f;
                var trainRes = ocrModel.TrainModel(batches, batchSize, itersPerBatch, learningRate);
                var testRes = ocrModel.TestModel(10000);
                Console.WriteLine("{0,18} {1,18} {2,18:N4} {3,18:N2}",
                    i, trainRes.TotalSamples, trainRes.BatchErrors.Average(), testRes.Accuracy * 100.0);
            }
        }

        [TestMethod]
        public void MainPass5()
        {
            // First hidden layer size (2 HL)
            ICharacterGenerator characterGenerator = new MyCharacterGenerator(24, 24);
            Console.WriteLine("{0,18} {1,18} {2,18} {3,18}", "HL1 size", "Total samples", "Avg train loss", "Accuracy [%]");
            for (int i = 10; i <= 60; i += 2)
            {
                var hiddenLayerSizes = new List<int>() { i, 16 };
                IOCRModel ocrModel = new MyOCRModel(characterGenerator, hiddenLayerSizes);
                int batches = 16384;
                int batchSize = 4;
                int itersPerBatch = 1;
                float learningRate = 0.40f;
                var trainRes = ocrModel.TrainModel(batches, batchSize, itersPerBatch, learningRate);
                var testRes = ocrModel.TestModel(10000);
                Console.WriteLine("{0,18} {1,18} {2,18:N4} {3,18:N2}",
                    i, trainRes.TotalSamples, trainRes.BatchErrors.Average(), testRes.Accuracy * 100.0);
            }
        }

        [TestMethod]
        public void MainPass6()
        {
            // Second hidden layer size (2 HL)
            ICharacterGenerator characterGenerator = new MyCharacterGenerator(24, 24);
            Console.WriteLine("{0,18} {1,18} {2,18} {3,18}", "HL2 size", "Total samples", "Avg train loss", "Accuracy [%]");
            for (int i = 4; i <= 40; i += 2)
            {
                var hiddenLayerSizes = new List<int>() { 32, i };
                IOCRModel ocrModel = new MyOCRModel(characterGenerator, hiddenLayerSizes);
                int batches = 16384;
                int batchSize = 4;
                int itersPerBatch = 1;
                float learningRate = 0.40f;
                var trainRes = ocrModel.TrainModel(batches, batchSize, itersPerBatch, learningRate);
                var testRes = ocrModel.TestModel(10000);
                Console.WriteLine("{0,18} {1,18} {2,18:N4} {3,18:N2}",
                    i, trainRes.TotalSamples, trainRes.BatchErrors.Average(), testRes.Accuracy * 100.0);
            }
        }
    }
}
