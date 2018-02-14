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
        private Point lastKnownMouseDown;
        private bool mouseDown;

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
            mouseDown = false;
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.Text = e.GetPosition(Canvas).ToString();
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(Canvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (mouseDown && !currentPoint.Equals(lastKnownMouseDown))
                {
                    Line d = new Line();
                    d.Stroke = SystemColors.WindowFrameBrush;
                    d.X1 = lastKnownMouseDown.X;
                    d.Y1 = lastKnownMouseDown.Y;
                    d.X2 = currentPoint.X;
                    d.Y2 = currentPoint.Y;
                    Canvas.Children.Add(d);
                }

                if (!mouseDown)
                {
                    mouseDown = true;
                }
            }
            else
            {
                mouseDown = false;
            }

            lastKnownMouseDown = currentPoint;
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
            }
        }
    }
}