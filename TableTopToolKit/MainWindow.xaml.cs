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

namespace TableTopToolKit
{
    public partial class MainWindow : Window
    {
        private App main;

        private void ZoomIn()
        {
            CanvasScaleTransform.ScaleX *= 1.1;
            CanvasScaleTransform.ScaleY *= 1.1;
        }

        private void ZoomOut()
        {
            CanvasScaleTransform.ScaleX /= 1.1;
            CanvasScaleTransform.ScaleY /= 1.1;
        }

        private void ClearCanvas()
        {
            Canvas.Children.Clear();
        }

        public MainWindow()
        {
            InitializeComponent();
            main = Application.Current as App;
            main.InitializeCanvasDrawing(Canvas);
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(Canvas);
            main.MouseMove(currentPoint, e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Back:
                    ClearCanvas();
                    break;

                case Key.Add:
                    ZoomIn();
                    break;

                case Key.Subtract:
                    ZoomOut();
                    break;

                case Key.Tab:
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        main.Command(App.Controls.SelectPrevious);
                    }
                    else
                    {
                        main.Command(App.Controls.SelectNext);
                    }
                    break;
            }
        }
    }
}