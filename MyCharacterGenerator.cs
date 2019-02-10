// 10-02-2019, BVH

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProceduralOCR
{
    public class MyCharacterGenerator : ICharacterGenerator
    {
        public MyCharacterGenerator(int imageWidth, int imageHeight)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            random = new Random();
        }

        private readonly int imageWidth, imageHeight;
        private readonly Random random;

        public float[,] Generate()
        {
            // Create visual and draw text
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                string text = random.Next(10).ToString();
                context.DrawText(new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                    new Typeface("Times New Roman"), 16.0, Brushes.White), new Point(0.0, 0.0));
            }

            // Initialize arrays
            var bmp = new RenderTargetBitmap(imageWidth, imageHeight, 96.0, 96.0, PixelFormats.Pbgra32);
            byte[] bytes = new byte[imageWidth * imageHeight * 4];
            float[,] output = new float[imageWidth, imageHeight];

            // Render and copy
            bmp.Render(visual);
            bmp.CopyPixels(bytes, imageWidth * 4, 0);
            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    // Perform uniform conversion from [0, 255] to [0.0, 1.0).
                    output[x, y] = bytes[(x + y * imageWidth) * 4] / 256.0f;
                }
            }
            return output;
        }

        public List<float[,]> GenerateMulti(int count)
        {
            return Enumerable.Range(0, count).Select(i => Generate()).ToList();
        }
    }
}
