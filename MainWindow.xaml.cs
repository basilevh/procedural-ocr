// 08-01-2019, BVH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProceduralOCR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int imageWidth = 20;
        private const int imageHeight = 20;

        public MainWindow()
        {
            InitializeComponent();
            characterGenerator = new MyCharacterGenerator(imageWidth, imageHeight);
            ocrModel = new MyOCRModel(imageWidth, imageHeight, characterGenerator);
        }

        private ICharacterGenerator characterGenerator;
        private IOCRModel ocrModel;

        private void btnExample_Click(object sender, RoutedEventArgs e)
        {
            var output = characterGenerator.Generate();
            var image = MyBitmapTools.GetImage(output.Image);
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            imageControl.Source = image;
            txtResult.Text = output.Character.ToString();

            // Automatically attempt to recognize
            btnRecognize_Click(sender, e);
        }

        private void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            var result = ocrModel.TrainModel(64, 64);
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            var result = ocrModel.TestModel(1024);
            txtResult.Text = "Accuracy:" + Environment.NewLine +
                ((double)result.Correct / result.Tested * 100.0).ToString("0.00") + "%";
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRecognize_Click(object sender, RoutedEventArgs e)
        {
            float[,] input = MyBitmapTools.GetArray(imageControl.Source as WriteableBitmap);
            var result = ocrModel.ExecuteSingle(input);
            txtResult.Text = result.MostConfident + string.Join("",
                Enumerable.Range('0', 10).Select(c => Environment.NewLine +
                (char)c + ": " + result.GetProbability((char)c).ToString("0.0000")));
        }
    }
}
