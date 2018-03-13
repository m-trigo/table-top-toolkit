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
using System.Windows.Shapes;

namespace TableTopToolKit
{
    public partial class PrintPreviewWindow : Window
    {
        public PrintPreviewWindow(ImageSource previewImage)
        {
            InitializeComponent();
            ImagePreview.Source = previewImage;
            SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}