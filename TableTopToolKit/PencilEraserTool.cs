using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    class PencilEraserTool : DrawingTool
    {
        public static Theme theme = Theme.standard;

        private CanvasDrawings canvasDrawings;
        private Polyline eraserLine;
        private List<Drawing> toErase;
        private bool added;

        private const double ERASE_RANGE_SQR = 20;

        public PencilEraserTool( CanvasDrawings cd )
        {
            canvasDrawings = cd;
            toErase = new List<Drawing>();
            eraserLine = null;
            added = false;
        }

        public void MouseDown( Point mousePosition, MouseEventArgs mouseEvent )
        {
            if ( mouseEvent.LeftButton == MouseButtonState.Pressed )
            {
                eraserLine = new Polyline
                {
                    Stroke = theme.EraserColor,
                    StrokeThickness = canvasDrawings.ForegroundThickness
                };
            }

            UpdateIntersectingDrawings();
        }

        public void MouseExit( Point mousePosition, MouseEventArgs mouseEvent )
        {
            CleanUp();
        }

        public void MouseUp( Point mousePosition, MouseEventArgs mouseEvent )
        {
            canvasDrawings.Erase( toErase );
            CleanUp();
        }

        public void MouseMove( Point mousePosition, MouseEventArgs mouseEvent )
        {
            if ( eraserLine == null )
            {
                return;
            }

            eraserLine.Points.Add( mousePosition );

            if ( !added )
            {
                canvasDrawings.AddNonDrawing( eraserLine );
                added = true;
            }

            UpdateIntersectingDrawings();
        }

        public void Close()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            canvasDrawings.RemoveNonDrawing( eraserLine );
            toErase.Clear();
            eraserLine = null;
            added = false;
        }

        private void UpdateIntersectingDrawings()
        {
            foreach ( Drawing drawing in canvasDrawings.Drawings )
            {
                if ( toErase.Contains( drawing ) )
                {
                    continue;
                }

                foreach ( Shape shape in drawing.Elements )
                {
                    if ( !( shape is Polyline ) )
                    {
                        break;
                    }

                    Polyline line = shape as Polyline;
                    foreach ( Point point in line.Points )
                    {
                        foreach ( Point erasePoint in eraserLine.Points )
                        {
                            if ( Point.Subtract( point, erasePoint ).LengthSquared < ERASE_RANGE_SQR )
                            {
                                toErase.Add( drawing );
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
