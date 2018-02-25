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
        private int selectedDrawing;

        private class Drawing
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }

            public Drawing(int start)
            {
                StartIndex = start;
                EndIndex = -1;
            }
        }

        private List<Drawing> drawings;

        private void StartDrawing(int index)
        {
            drawings.Add(new Drawing(index));
        }

        private void EndDrawing(int index)
        {
            if (drawings.Count > 0 && drawings[drawings.Count - 1].EndIndex == -1)
            {
                drawings[drawings.Count - 1].EndIndex = index;
            }
        }

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
            drawings = new List<Drawing>();
            selectedDrawing = -1;
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
                    d.Stroke = Brushes.Black;
                    d.X1 = lastKnownMouseDown.X;
                    d.Y1 = lastKnownMouseDown.Y;
                    d.X2 = currentPoint.X;
                    d.Y2 = currentPoint.Y;
                    Canvas.Children.Add(d);
                }

                if (!mouseDown)
                {
                    mouseDown = true;
                    StartDrawing(Canvas.Children.Count);
                }
            }
            else
            {
                mouseDown = false;
                EndDrawing(Canvas.Children.Count);
            }

            lastKnownMouseDown = currentPoint;
            Console.Text = $"drawings: {drawings.Count}";
        }

        private void SelectDrawing(bool reverse = false)
        {
            if (drawings.Count > 0)
            {
                if (selectedDrawing >= 0)
                {
                    UnhightLightDrawing(drawings[selectedDrawing]);
                }

                if (reverse)
                {
                    selectedDrawing--;
                    if (selectedDrawing < 0)
                    {
                        selectedDrawing = drawings.Count - 1;
                    }
                }
                else
                {
                    selectedDrawing++;
                    if (selectedDrawing >= drawings.Count)
                    {
                        selectedDrawing = 0;
                    }
                }

                HightlightDrawing(drawings[selectedDrawing]);
            }
        }

        private void UnhightLightDrawing(Drawing d)
        {
            for (int i = d.StartIndex; i < d.EndIndex; i++)
            {
                Line l = Canvas.Children[i] as Line;
                l.Stroke = Brushes.Black;
            }
        }

        private void HightlightDrawing(Drawing d)
        {
            for (int i = d.StartIndex; i < d.EndIndex; i++)
            {
                Line l = Canvas.Children[i] as Line;
                l.Stroke = Brushes.Blue;
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

                case Key.Tab:
                    SelectDrawing(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
                    break;
            }
        }
    }
}