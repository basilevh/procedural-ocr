// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProceduralOCR
{
    /// <summary>
    /// Utility class to convert between floating point arrays and actual bitmaps to be displayed on-screen.
    /// </summary>
    public static class MyBitmapTools
    {
        public static ImageSource GetImage(float[,] input)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            var output = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Gray8, null);
            byte[] pixels = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Perform uniform conversion from [0.0, 1.0) to [0, 255].
                    // Values outside this range can happen however, which we simply truncate.
                    pixels[x + y * width] = (byte)Math.Min(Math.Max(input[x, y] * 256.0f, 0.0f), 255.0f);
                }
            }
            output.WritePixels(new Int32Rect(0, 0, width, height), pixels, width, 0);
            return output;
        }
    }
}
