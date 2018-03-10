using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            main.CurrentTool.MouseMove(currentPoint, e);
        }

        private void OnCanvasMouseUp(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(Canvas);
            main.CurrentTool.MouseUp(currentPoint, e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    main.Command(App.Controls.SavePNG);
                    break;

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

                case Key.Z:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        main.Command(App.Controls.Undo);
                    }
                    break;

                case Key.Y:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        main.Command(App.Controls.Redo);
                    }
                    break;

                case Key.Space:
                    main.Command(App.Controls.ToggleGrid);
                    break;
            }
        }

        private void GridToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            main.Command(App.Controls.ToggleGrid);
        }
    }
}