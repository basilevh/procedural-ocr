// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Interface for any generator of random optical characters in 2D array form.
    /// </summary>
    public interface ICharacterGenerator
    {
        LabeledCharacter Generate();

        List<LabeledCharacter> GenerateMulti(int count);
    }
}
