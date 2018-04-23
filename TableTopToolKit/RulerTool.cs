using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    public class RulerTool : DrawingTool
    {
        private CanvasDrawings source;
        private Grid grid;
        private Point lastKnownMouseDown;
        private bool drawing;
        private bool mouseDown;
        public static Theme theme = Theme.standard;

        private Line MeasuringLine { get; set; }
        private TextBlock DistanceDisplay { get; set; }

        public RulerTool(CanvasDrawings cd, Grid grid)
        {
            source = cd;
            this.grid = grid;
            drawing = false;
            mouseDown = false;
            DistanceDisplay = new TextBlock();
            DistanceDisplay.FontSize = 22;
            DistanceDisplay.FontFamily = new FontFamily("Consolas");
        }

        private void UpdateDisplay(double x, double y, string text)
        {
            DistanceDisplay.Text = text;
            DistanceDisplay.Foreground = theme.RulerColor;
            Canvas.SetLeft(DistanceDisplay, x);
            Canvas.SetTop(DistanceDisplay, y);
        }

        private void RenderLine()
        {
            source.AddNonDrawing(new List<UIElement>() { MeasuringLine, DistanceDisplay });
        }

        private void UnrenderLine()
        {
            source.RemoveNonDrawing(new List<UIElement>() { MeasuringLine, DistanceDisplay });
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            UnrenderLine();

            if (mouseEvent.LeftButton == MouseButtonState.Pressed && mouseDown)
            {
                Point start = grid.SnapToGridCornersOrCenter(lastKnownMouseDown.X, lastKnownMouseDown.Y);
                Point end = grid.SnapToGridCornersOrCenter(mousePosition.X, mousePosition.Y);

                if (!drawing)
                {
                    MeasuringLine = new Line()
                    {
                        X1 = start.X,
                        Y1 = start.Y,
                        X2 = end.X,
                        Y2 = end.Y,
                        Stroke = theme.RulerColor,
                        StrokeDashArray = new DoubleCollection(new double[] { 6, 2 }),
                        StrokeThickness = 3
                    };
                    drawing = true;
                }
                else
                {
                    MeasuringLine.X1 = start.X;
                    MeasuringLine.Y1 = start.Y;
                    MeasuringLine.X2 = end.X;
                    MeasuringLine.Y2 = end.Y;
                }

                Vector mouseDragDistance = mousePosition - lastKnownMouseDown;
                double dy = mouseDragDistance.Y / (mouseDragDistance.Y == 0 ? 1 : Math.Abs(mouseDragDistance.Y));
                double displayOffset = dy > 0 ? -40 : 20;
                UpdateDisplay(start.X, start.Y + displayOffset, grid.DistanceInSquares(start, end).ToString("0.##"));
                RenderLine();
            }
            else if (mouseEvent.LeftButton == MouseButtonState.Released && mouseDown)
            {
                mouseDown = false;
            }
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            UnrenderLine();
            drawing = false;
            mouseDown = false;
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
            lastKnownMouseDown = mousePosition;
            UnrenderLine();
            mouseDown = true;
            drawing = false;
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
            drawing = false;
            UnrenderLine();
        }

        public void Close()
        {
            UnrenderLine();
        }
    }
}