using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    public partial class MainWindow : Window
    {
        private Point lastKnownMouseDown;
        private bool mouseDown;

        private bool gridVisible;
        private Grid grid;

        private Line currentLine;
        private bool drawingStraightLine;

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

            InitializeGrid();
        }

        public MainWindow()
        {
            InitializeComponent();
            mouseDown = false;
            drawingStraightLine = false;

            InitializeGrid();
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(Canvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (mouseDown && !currentPoint.Equals(lastKnownMouseDown))
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        if (!drawingStraightLine)
                        {
                            drawingStraightLine = true;

                            currentLine = new Line();
                            Point beginning = grid.SnapToGrid(currentPoint.X, currentPoint.Y);
                            currentLine.X1 = beginning.X;
                            currentLine.Y1 = beginning.Y;

                            currentLine.StrokeThickness = 2;
                            currentLine.Stroke = Brushes.Black;

                            Canvas.Children.Add(currentLine);
                        }

                        Point snapped = grid.SnapToGrid(currentPoint.X, currentPoint.Y);
                        currentLine.X2 = snapped.X;
                        currentLine.Y2 = snapped.Y;
                    }

                    else
                    {
                        Line d = new Line();
                        d.Stroke = SystemColors.WindowFrameBrush;

                        d.X1 = lastKnownMouseDown.X;
                        d.Y1 = lastKnownMouseDown.Y;
                        d.X2 = currentPoint.X;
                        d.Y2 = currentPoint.Y;

                        Canvas.Children.Add(d);
                    }
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

        private void OnCanvasMouseUp(object sender, MouseEventArgs e)
        {
            if (drawingStraightLine)
            {
                drawingStraightLine = false;
            }
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

                case Key.Space:
                    ToggleGrid();
                    break;
            }
        }

        private void InitializeGrid()
        {
            grid = new Grid((int)Canvas.Width, (int)Canvas.Height, 30);
            grid.InitializeGridLines();

            foreach(Line line in grid.gridLines)
            {
                Canvas.Children.Add(line);
            }

            gridVisible = true;
        }

        private void ToggleGrid()
        {
            foreach(Line line in grid.gridLines)
            {
                line.Stroke = gridVisible ? Brushes.Transparent : Brushes.Gray;
            }
            
            gridVisible = !gridVisible;
        }
    }
}