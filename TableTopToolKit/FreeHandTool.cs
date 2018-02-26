using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    internal class FreeHandTool : DrawingTool
    {
        private CanvasDrawings source;
        private Point lastKnownMouseDown;
        private bool startDrawing;
        private bool continueDrawning;

        public FreeHandTool(CanvasDrawings canvasDrawings)
        {
            startDrawing = false;
            continueDrawning = false;
            source = canvasDrawings;
        }

        public void MouseMove(Point mousePosition, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                if (startDrawing && !mousePosition.Equals(lastKnownMouseDown))
                {
                    Line lineSegment = new Line();
                    lineSegment.Stroke = source.defaultBrush;
                    lineSegment.X1 = lastKnownMouseDown.X;
                    lineSegment.Y1 = lastKnownMouseDown.Y;
                    lineSegment.X2 = mousePosition.X;
                    lineSegment.Y2 = mousePosition.Y;

                    if (continueDrawning)
                    {
                        source.ContinueDrawing(lineSegment);
                    }
                    else
                    {
                        source.StartDrawing(lineSegment);
                        continueDrawning = true;
                    }
                }

                if (!startDrawing)
                {
                    startDrawing = true;
                }

                lastKnownMouseDown = mousePosition;
            }
            else
            {
                if (startDrawing && continueDrawning)
                {
                    startDrawing = false;
                    continueDrawning = false;
                    Line lineSegment = new Line();
                    lineSegment.Stroke = source.defaultBrush;
                    lineSegment.X1 = lastKnownMouseDown.X;
                    lineSegment.Y1 = lastKnownMouseDown.Y;
                    lineSegment.X2 = mousePosition.X;
                    lineSegment.Y2 = mousePosition.Y;
                    source.EndDrawing(lineSegment);
                }
            }
        }
    }
}