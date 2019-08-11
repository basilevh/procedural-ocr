// 11-08-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Character source implementation that retrieves random samples from the MNIST dataset.
    /// The generation itself is not random, so duplicates may eventually occur.
    /// Recommended for use in testing.
    /// </summary>
    public class MnistCharacterSource : ICharacterSource
    {
        public int ImageWidth => throw new NotImplementedException();

        public int ImageHeight => throw new NotImplementedException();

        public LabeledCharacter Generate()
        {
            throw new NotImplementedException();
        }

        public List<LabeledCharacter> GenerateMulti(int count)
        {
            throw new NotImplementedException();
        }
    }
}
