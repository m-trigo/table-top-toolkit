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
            if ( mouseEvent.LeftButton == MouseButtonState.Pressed && eraserLine == null )
            {
                eraserLine = new Polyline
                {
                    Stroke = theme.EraserColor,
                    StrokeThickness = canvasDrawings.ForegroundThickness
                };
            }
            else
            {
                CleanUp();
            }
        }

        public void MouseExit( Point mousePosition, MouseEventArgs mouseEvent )
        {
            CleanUp();
        }

        public void MouseUp( Point mousePosition, MouseEventArgs mouseEvent )
        {
            foreach( Drawing drawing in toErase )
            {
                foreach ( Shape shape in drawing.Elements )
                {
                    if ( !( shape is Polyline ) )
                    {
                        break;
                    }

                    Polyline line = shape as Polyline;
                    line.Stroke = theme.DrawingsColor;
                }
            }

            canvasDrawings.Erase( toErase );
            CleanUp();
        }

        public void MouseMove( Point mousePosition, MouseEventArgs mouseEvent )
        {
            if ( eraserLine == null )
            {
                return;
            }

            if ( eraserLine.Points.Count > 0 )
            {
                Point lastAddedPoint = eraserLine.Points[ eraserLine.Points.Count - 1 ];
                IEnumerable<Point> interpolated = InterpolatedPoints( lastAddedPoint, mousePosition );
                foreach ( Point point in interpolated )
                {
                    eraserLine.Points.Add( point );
                }

                UpdateIntersectingDrawings();
            }

            eraserLine.Points.Add( mousePosition );

            if ( !added )
            {
                canvasDrawings.AddNonDrawing( eraserLine );
                added = true;
            }

            while ( eraserLine.Points.Count > 100 )
            {
                eraserLine.Points.RemoveAt( 0 );
            }
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

        private bool IsCloseEnough( Point A, Point B )
        {
            return Point.Subtract( A, B ).LengthSquared < ERASE_RANGE_SQR;
        }

        private IEnumerable<Point> InterpolatedPoints( Point A, Point B )
        {
            List<Point> interpolated = new List<Point>();

            Vector vector = Point.Subtract( B, A );
            vector.Normalize();

            Point m = A;
            while ( !IsCloseEnough( m, B ) )
            {
                interpolated.Add( m );
                m = Point.Add( m, vector );
            }

            return interpolated;
        }

        private void UpdateIntersectingDrawings()
        {
            foreach ( Drawing drawing in canvasDrawings.Drawings )
            {
                if ( toErase.Contains( drawing ) )
                {
                    continue;
                }

                bool done = false;
                foreach ( Shape shape in drawing.Elements )
                {
                    if ( done || !( shape is Polyline ) )
                    {
                        break;
                    }

                    Polyline line = shape as Polyline;
                    foreach ( Point point in line.Points )
                    {
                        foreach ( Point erasePoint in eraserLine.Points )
                        {
                            if ( IsCloseEnough( point, erasePoint ) )
                            {
                                toErase.Add( drawing );
                                line.Stroke = theme.EraserSelectionColor;
                                done = true;
                                break;
                            }
                        }

                        if ( done )
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
