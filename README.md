# procedural-ocr
A deep neural network for the purpose of recognizing digits, that can be trained "by itself" and without any input data.

* **Why did I make this?**
  1. To demonstrate a small proof of concept that AI-related applications might not always need huge sets of data; obtaining them is often a practical issue in real-world scenarios.
  2. To familiarize myself with deep learning by writing several algorithms from scratch in C# .NET.

* **Why does it not require data?** The trick is to use Windows' system-installed fonts that look like handwriting, before applying image augmentation in order to generate many variants. The resulting images are *somewhat* realistically looking replications of human handwriting, but of course not as good as MNIST.

* **What is the accuracy / performance?** (TBD, still optimizing parameters)
