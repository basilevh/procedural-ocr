// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Encapsulates the result of a single execution through an OCR model.
    /// More specifically, the output is given as a list of probabilities,
    /// where the most confident character is associated with the highest probability.
    /// </summary>
    public struct SingleResult
    {
        public SingleResult(IDictionary<char, float> probabilities)
        {
            this.probabilities = probabilities;
            MostConfident = probabilities.Aggregate((a, b) => a.Value > b.Value ? a : b).Key;
        }

        private readonly IDictionary<char, float> probabilities;

        public float GetProbability(char character) => probabilities[character];

        public char MostConfident { get; }
    }
}
