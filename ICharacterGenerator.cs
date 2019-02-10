// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    public interface ICharacterGenerator
    {
        float[,] Generate();

        List<float[,]> GenerateMulti(int count);
    }
}
