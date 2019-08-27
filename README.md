# procedural-ocr
A deep neural network for the purpose of recognizing digits, that can be trained "by itself" without explicitly requiring any input data.

* **Why is no data required?** The trick is to use Windows' system-installed fonts that look like handwriting, before applying image augmentation in order to generate many diverse variants. The resulting images are *somewhat* realistically looking replications of human handwriting, but of course not as good as for example MNIST.

* **Why did I create this?**
  1. To demonstrate a small proof of concept that AI-related applications might not always need huge sets of data; obtaining the relevant data is often a practical issue in real-world scenarios.
  2. While using Python libraries would of course be a more appropriate alternative in most use cases, a second reason is to familiarize myself with deep learning by writing several algorithms from scratch in C# .NET.

* **What are the training and test sets?** A character generator class constantly creates new images on the fly, so in principle both sets are (almost) infinitely large. I intend to add support for MNIST for more reliable testing.

* **What is the network architecture?** For an image size of 24x24, the list of layer sizes is as follows: [576, 32, 16, 10]. The input layer is of size 24x24 because that is the image size, and the output layer is of size 10 because that is the amount of different characters we aim to detect. The hidden layer sizes were determined by hyperparameter optimization. The activation function is the standard logistic sigmoid function.

* **What is the accuracy / performance?** TBD; still optimizing, but for now an accuracy above 80-85% is definitely feasible given enough training time.

![alt text](https://i.imgur.com/4Hw6qrL.png "Screenshot of Procedural OCR (beta)")

### To-do

* Support input from the MNIST dataset to allow for testing on actual real-world data.

* Further optimize parameters to achieve higher accuracies.

* Visualize the neural network in more detail (show all connections and weights?).

* Bonus: implement convolutional architectures, support (and auto-segment) multiple digits in one image, support letters in addition to digits, ...


