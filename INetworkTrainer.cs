// 05-08-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Interface for an object that trains a neural network via training examples.
    /// </summary>
    public interface INetworkTrainer
    {
        /// <summary>
        /// Adjusts the network's weights and biases to better match the given set of training examples.
        /// </summary>
        /// <param name="examples">Mini-batch of input-output pairs / training examples that indirectly dictate the network updates.</param>
        /// <returns>The training loss.</returns>
        double SingleIteration(List<InputOutputPair> examples);
    }
}
