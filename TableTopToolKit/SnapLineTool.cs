﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    public class SnapLineTool : DrawingTool
    {
        private CanvasDrawings source;

        private Point lastKnownMouseDown;
        private bool mouseDown;

        private Line currentLine;

        private bool drawingCornerSnappinLine;
        private bool drawingGridSnappingLine;

        private Grid grid;

        public SnapLineTool(CanvasDrawings canvasDrawings, Grid grid)
        {
            source = canvasDrawings;
            this.grid = grid;
            mouseDown = false;
            drawingCornerSnappinLine = false;
            drawingGridSnappingLine = false;
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                if (mouseDown && !mousePosition.Equals(lastKnownMouseDown))
                {
                    if (!drawingGridSnappingLine && !drawingCornerSnappinLine)
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift))
                        {
                            drawingGridSnappingLine = true;
                        }
                        else
                        {
                            drawingCornerSnappinLine = true;
                        }

                        currentLine = new Line();
                        currentLine.Stroke = source.ForegroundColor;
                        currentLine.StrokeThickness = source.ForegroundThickness;
                        Point beginning = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);
                        currentLine.X1 = beginning.X;
                        currentLine.Y1 = beginning.Y;

                        source.AddDrawing(new Drawing(currentLine));
                    }

                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        Point snapped = grid.SnapToGridLines(new Point(currentLine.X1, currentLine.Y1), new Point(mousePosition.X, mousePosition.Y));
                        currentLine.X2 = snapped.X;
                        currentLine.Y2 = snapped.Y;
                    }
                    else
                    {
                        Point snapped = grid.SnapToGridCorners(mousePosition.X, mousePosition.Y);
                        currentLine.X2 = snapped.X;
                        currentLine.Y2 = snapped.Y;
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

            lastKnownMouseDown = mousePosition;
        }

        public static void StandardizeLineDirection(Line line)
        {
            if (line == null)
            {
                return;
            }

            // make the lines go top to bottom
            if (line.X1 >= line.X2 && line.Y1 >= line.Y2 || line.X1 <= line.X2 && line.Y1 > line.Y2)
            {
                double temp = line.X1;
                line.X1 = line.X2;
                line.X2 = temp;

                temp = line.Y1;
                line.Y1 = line.Y2;
                line.Y2 = temp;
            }
        }

        public void MouseUp(Point mousePosition, MouseEventArgs mouseEvent)
        {
            StandardizeLineDirection(currentLine);

            if (drawingCornerSnappinLine)
            {
                drawingCornerSnappinLine = false;
            }

            if (drawingGridSnappingLine)
            {
                drawingGridSnappingLine = false;
            }
        }

        public void MouseDown(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }

        public void MouseExit(Point mousePosition, MouseEventArgs mouseEvent)
        {
        }

        public void Close()
        {
        }
    }
}