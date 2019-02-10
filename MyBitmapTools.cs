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
        public static ImageSource GetImage(float[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            var output = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Gray8, null);
            byte[] bytes = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Perform uniform conversion from [0.0, 1.0) to [0, 255].
                    // Values outside this range can happen however, which we simply truncate.
                    bytes[x + y * width] = (byte)Math.Min(Math.Max(array[x, y] * 256.0f, 0.0f), 255.0f);
                }
            }
            output.WritePixels(new Int32Rect(0, 0, width, height), bytes, width, 0);
            return output;
        }

        public static float[,] GetArray(WriteableBitmap bitmap)
        {
            int width = (int)bitmap.Width;
            int height = (int)bitmap.Height;
            byte[] bytes = new byte[width * height * 4];
            bitmap.CopyPixels(bytes, width * 4, 0);
            float[,] output = new float[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Perform uniform conversion from [0, 255] to [0.0, 1.0)
                    output[x, y] = bytes[(x + y * width) * 4] / 256.0f;
                }
            }
            return output;
        }

        public static float[] ReshapeArray(float[,] array)
        {
            float[] dest = new float[array.Length];
            Buffer.BlockCopy(array, 0, dest, 0, array.Length);
            return dest;
        }

        public static float[,] ReshapeArray(float[] array, int width, int height)
        {
            float[,] dest = new float[width, height];
            Buffer.BlockCopy(array, 0, dest, 0, array.Length);
            return dest;
        }
    }
}
