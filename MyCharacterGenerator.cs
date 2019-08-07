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
    /// <summary>
    /// Character generator implementation that adds random noise, rotations, and skews.
    /// </summary>
    public class MyCharacterGenerator : ICharacterGenerator
    {
        private readonly string[] MyTypefaces = {
            // Common
            /*"Arial", "Comic Sans MS", "Courier New",
            "Garamond", "Georgia", "Impact",
            "Times New Roman", "Trebuchet MS", "Verdana",*/
            // Handwriting
            "Bradley Hand ITC", "Brush Script MT", "Brush Script Std",
            "Edwardian Script ITC", "Freestyle Script", "French Script MT",
            "Kunstler Script",  "Lucida Calligraphy", "Lucida Handwriting",
            "Matura MT Script Capitals", "Mistral", "Palace Script MT",
            "Pristina", "Script MT", "Segoe Script",
            "Viner Hand ITC", "Vladimir Script"
            };


        public MyCharacterGenerator(int imageWidth, int imageHeight)
        {
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
            random = new Random();

            // Hard parameters
            /*NoiseStdDev = 0.07;
            MaxRotationAngle = 15.0;
            MaxScaleFactor = 1.1;
            MaxSkewAngle = 15.0;
            TranslationFraction = 0.7;*/

            // Medium parameters
            NoiseStdDev = 0.04;
            MaxRotationAngle = 10.0;
            MaxScaleFactor = 1.05;
            MaxSkewAngle = 10.0;
            TranslationFraction = 0.4;

            // Easy parameters (for debugging)
            /*NoiseStdDev = 0.01;
            MaxRotationAngle = 1.0;
            MaxScaleFactor = 1.01;
            MaxSkewAngle = 1.0;
            TranslationFraction = 0.1;*/
        }

        private readonly Random random;

        public int ImageWidth { get; }

        public int ImageHeight { get; }

        /// <summary>
        /// The standard deviation of the Gaussian noise applied on all pixels, as a fraction of the maximum pixel value.
        /// </summary>
        public double NoiseStdDev
        {
            get; set;
        }

        /// <summary>
        /// The maximum random rotation angle in degrees.
        /// </summary>
        public double MaxRotationAngle
        {
            get; set;
        }

        /// <summary>
        /// An indication of the maximum random scaling factor.
        /// A value of 1.2 means that both dimensions will get independently scaled by a factor of 0.8 to 1.2.
        /// </summary>
        public double MaxScaleFactor
        {
            get; set;
        }

        /// <summary>
        /// The maximum random skew angle for both dimensions.
        /// </summary>
        public double MaxSkewAngle
        {
            get; set;
        }

        public double TranslationFraction
        {
            get; set;
        }

        public LabeledCharacter Generate()
        {
            // Initialize character and output array
            char toDraw = random.Next(10).ToString()[0];
            float[,] output = new float[ImageWidth, ImageHeight];

            // Create visual and draw text
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                string text = toDraw.ToString();
                // Character should fill (most of) the image
                double fontSize = ImageHeight;
                // These offsets are approximations made for 16 x 16 Times New Roman
                double offsetX = ImageWidth / 4.0;
                double offsetY = -ImageHeight / 12.0;
                string tfName = MyTypefaces[random.Next(MyTypefaces.Length)];
                context.DrawText(new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                    new Typeface(tfName), fontSize, Brushes.White), new Point(offsetX, offsetY));
            }

            // Apply random rotation
            var transforms = new TransformGroup();
            double angle = random.NextDouble() * MaxRotationAngle * 2.0 - MaxRotationAngle;
            double centerX = ImageWidth / 2.0;
            double centerY = ImageHeight / 2.0;
            transforms.Children.Add(new RotateTransform(angle, centerX, centerY));

            // Apply random scaling
            double scaleX = random.NextDouble() * (MaxScaleFactor - 1.0) * 2.0 + 2.0 - MaxScaleFactor;
            double scaleY = random.NextDouble() * (MaxScaleFactor - 1.0) * 2.0 + 2.0 - MaxScaleFactor;
            transforms.Children.Add(new ScaleTransform(scaleX, scaleY));

            // Apply random skew
            double angleX = random.NextDouble() * MaxSkewAngle * 2.0 - MaxSkewAngle;
            double angleY = random.NextDouble() * MaxSkewAngle * 2.0 - MaxSkewAngle;
            transforms.Children.Add(new SkewTransform(angleX, angleY, centerX, centerY));

            // Apply random translation
            double transX = (random.NextDouble() * ImageWidth / 5.0 - ImageWidth / 8.0) * TranslationFraction;
            double transY = (random.NextDouble() * ImageHeight / 10.0 - ImageHeight / 7.0) * TranslationFraction;
            transforms.Children.Add(new TranslateTransform(transX, transY));
            visual.Transform = transforms;

            // Render and copy to output
            var bmp = new RenderTargetBitmap(ImageWidth, ImageHeight, 96.0, 96.0, PixelFormats.Pbgra32);
            bmp.Render(visual);
            byte[] bytes = new byte[ImageWidth * ImageHeight * 4];
            bmp.CopyPixels(bytes, ImageWidth * 4, 0);
            double[] allNoise = MyRandom.NextStdGaussian(ImageWidth * ImageHeight);
            for (int y = 0; y < ImageHeight; y++)
            {
                for (int x = 0; x < ImageWidth; x++)
                {
                    // Perform uniform conversion from [0, 255] to [0.0, 1.0) and apply gaussian noise
                    int index = x + y * ImageWidth;
                    float input = bytes[index * 4] / 256.0f;
                    float noise = (float)(allNoise[index] * NoiseStdDev);
                    output[x, y] = Math.Min(Math.Max(input + noise, 0.0f), 1.0f);
                }
            }
            return new LabeledCharacter(toDraw, output);
        }

        public List<LabeledCharacter> GenerateMulti(int count)
        {
            return Enumerable.Range(0, count).Select(i => Generate()).ToList();
        }
    }
}
