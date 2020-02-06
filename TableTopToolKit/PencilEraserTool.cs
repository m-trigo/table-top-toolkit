using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    class PencilEraserTool : DrawingTool
    {
        private CanvasDrawings canvasDrawings;
        private bool erasing = false;

        public PencilEraserTool( CanvasDrawings cd )
        {
            canvasDrawings = cd;
        }

        public void MouseDown( Point mousePosition, MouseEventArgs mouseEvent )
        {
            erasing = mouseEvent.LeftButton == MouseButtonState.Pressed;
            EraseAt( mousePosition );
        }

        public void MouseExit( Point mousePosition, MouseEventArgs mouseEvent )
        {
            erasing = false;
        }

        public void MouseUp( Point mousePosition, MouseEventArgs mouseEvent )
        {
            erasing = false;
        }

        public void MouseMove( Point position, MouseEventArgs mouseEvent )
        {
            if ( !erasing )
            {
                return;
            }

            EraseAt( position );
        }

        public void Close()
        {
        }

        private void EraseAt(Point mousePosition)
        {
            List<UIElement> removables = new List<UIElement>();

            foreach ( Drawing drawing in canvasDrawings.Drawings )
            {
                foreach ( Shape shape in drawing.Elements )
                {
                    if ( !( shape is Polyline ) )
                    {
                        continue;
                    }

                    Polyline line = shape as Polyline;
                    foreach ( Point point in line.Points )
                    {
                        if ( Point.Subtract( point, mousePosition ).LengthSquared < 20 )
                        {
                            removables.Add( line );
                        }
                    }
                }
            }

            canvasDrawings.RemoveNonDrawing( removables );
        }
    }
}
