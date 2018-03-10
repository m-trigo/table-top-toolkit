using System.Collections.Generic;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    internal class Drawing
    {
        private List<Shape> shapes;

        public IEnumerable<Shape> Shapes { get => shapes; }

        public Drawing(Shape shape)
        {
            shapes = new List<Shape>();
            shapes.Add(shape);
        }

        public void AddToDrawing(Shape shape)
        {
            shapes.Add(shape);
        }
    }
}