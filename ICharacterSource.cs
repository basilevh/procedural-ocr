// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Interface for a generator of (possibly random) optical characters in 2D array form.
    /// </summary>
    public interface ICharacterSource
    {
        LabeledCharacter Generate();

        List<LabeledCharacter> GenerateMulti(int count);

        int ImageWidth { get; }

        int ImageHeight { get; }
    }
}
