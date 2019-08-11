# procedural-ocr
A deep neural network for the purpose of recognizing digits, that can be trained "by itself" without explicitly requiring any input data.

* **Why is no data required?** The trick is to use Windows' system-installed fonts that look like handwriting, before applying image augmentation in order to generate many variants. The resulting images are *somewhat* realistically looking replications of human handwriting, but of course not as good as MNIST.

* **Why did I create this?**
  1. To demonstrate a small proof of concept that AI-related applications might not always need huge sets of data; obtaining the relevant data is often a practical issue in real-world scenarios.
  2. To familiarize myself with deep learning by writing several algorithms from scratch in C# .NET.

* **What are the training and test sets?** A character generator class constantly creates new images on the fly, so in principle both sets are (almost) infinitely large.

* **What is the network architecture?** For an image size of 24x24, the whole layer stack is as follows: [576, 32, 16, 10]. The input layer is of size 24x24 because that is the image size, and the output layer is of size 10 because that is the amount of different characters we aim to detect. The hidden layer sizes were determined by hyperparameter optimization.

* **What is the accuracy / performance?** (TBD, still optimizing)
