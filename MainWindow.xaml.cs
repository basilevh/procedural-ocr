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
        private const int imageWidth = 16;
        private const int imageHeight = 16;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnExample_Click(object sender, RoutedEventArgs e)
        {
            ICharacterGenerator charGen = new MyCharacterGenerator(imageWidth, imageHeight);
            var output = charGen.Generate();
            var image = MyBitmapTools.GetImage(output);
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            imageControl.Source = image;
        }

        private void btnTrain_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDetect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
