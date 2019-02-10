// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralOCR
{
    /// <summary>
    /// Associates a character with its image in a 2D array form.
    /// </summary>
    public class LabeledCharacter
    {
        public LabeledCharacter(char character, float[,] image)
        {
            Character = character;
            Image = image;
        }

        public char Character { get; }
        public float[,] Image { get; }
    }
}
