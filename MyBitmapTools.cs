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
        public static ImageSource GetImageGray8(float[,] array)
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

        public static ImageSource GetImageRgb24(float[,,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            var output = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Rgb24, null);
            byte[] bytes = new byte[width * height * 3];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        // Perform uniform conversion from [0.0, 1.0) to [0, 255].
                        // Values outside this range can happen however, which we simply truncate.
                        bytes[c + (x + y * width) * 3] = (byte)Math.Min(Math.Max(array[x, y, c] * 256.0f, 0.0f), 255.0f);
                    }
                }
            }
            output.WritePixels(new Int32Rect(0, 0, width, height), bytes, width * 3, 0);
            return output;
        }

        /// <summary>
        /// Converts the given bitmap image into a float array, with values between 0.0 and 1.0.
        /// The pixel format is assumed to be Bgra24, and an average of the three color channels is taken.
        /// </summary>
        public static float[,] GetArray(BitmapImage bitmap)
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
                    byte b = bytes[(x + y * width) * 4];
                    byte g = bytes[(x + y * width) * 4 + 1];
                    byte r = bytes[(x + y * width) * 4 + 2];
                    output[x, y] = (b + g + r) / (256.0f * 3.0f);
                }
            }
            return output;
        }

        public static float[] ReshapeArray(float[,] array)
        {
            float[] dest = new float[array.Length];
            Buffer.BlockCopy(array, 0, dest, 0, array.Length * sizeof(float));
            return dest;
        }

        public static float[,] ReshapeArray(float[] array, int width, int height)
        {
            float[,] dest = new float[width, height];
            Buffer.BlockCopy(array, 0, dest, 0, array.Length * sizeof(float));
            return dest;
        }
    }
}
