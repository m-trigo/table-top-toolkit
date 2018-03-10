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
    internal class CanvasDrawings
    {
        private Canvas canvas;
        private List<Drawing> drawings;
        private List<Shape> undoDrawings;
        private List<Shape> newDrawings;
        private List<List<Shape>> allUndos;
        private int selectedDrawingIndex;

        public Brush backgroundBrush;
        public Brush foregroundBrush;
        public Brush highlightBrush;

        private double backgroundThickness;
        private double foregroundThickness;

        public int Width => (int)canvas.Width;
        public int Height => (int)canvas.Height;

        public CanvasDrawings(Canvas source)
        {
            canvas = source;
            drawings = new List<Drawing>();
            undoDrawings = new List<Shape>();
            newDrawings = new List<Shape>();
            allUndos = new List<List<Shape>>();
            selectedDrawingIndex = -1;
            backgroundBrush = Brushes.LightGray;
            foregroundBrush = Brushes.Black;
            highlightBrush = Brushes.Blue;
            backgroundThickness = 1;
            foregroundThickness = 3;
        }

        public void AddBackground(Shape element)
        {
            element.StrokeThickness = backgroundThickness;
            element.Stroke = backgroundBrush;
            canvas.Children.Add(element);
        }

        public void AddForeGround(Shape element)
        {
            element.StrokeThickness = foregroundThickness;
            element.Stroke = foregroundBrush;
            canvas.Children.Add(element);
        }

        public void AddSimpleDrawing(Shape element)
        {
            if (undoDrawings.Count > 0)
                addToAllUndoDrawings();
            undoDrawings.Clear();
            drawings.Add(new Drawing(canvas.Children.Count, canvas.Children.Count));
            AddForeGround(element);
            newDrawings.Add(element);
        }

        public void StartDrawing(Shape element)
        {
            if (undoDrawings.Count > 0)
                addToAllUndoDrawings();
            undoDrawings.Clear();
            drawings.Add(new Drawing(canvas.Children.Count));
            AddForeGround(element);
            newDrawings.Add(element);
        }

        private void addToAllUndoDrawings()
        {
            List<Shape> list = new List<Shape>();
            foreach (Shape item in undoDrawings)
            {
                list.Add(item);
            }
            allUndos.Add(list);
            newDrawings.Clear();
        }

        public void UndoDrawing()
        {
            
            if (newDrawings.Count > 0)
            {
                if (drawings.Count > 0)
                {
                    Shape s = canvas.Children[canvas.Children.Count - 1] as Shape;
                    undoDrawings.Add(s);
                    drawings.RemoveAt(drawings.Count - 1);
                    canvas.Children.RemoveAt(canvas.Children.Count - 1);
                    newDrawings.RemoveAt(newDrawings.Count - 1);
                }
            }
            else if (allUndos.Count > 0 && allUndos[allUndos.Count - 1].Count > 0)
            {
                int x = allUndos[allUndos.Count - 1].Count - 1;
                canvas.Children.Add(allUndos[allUndos.Count - 1][x]);
                drawings.Add(new Drawing(canvas.Children.Count));
                allUndos[allUndos.Count - 1].RemoveAt(x);
                
            }
            else if (drawings.Count > 0)
            {
                Shape s = canvas.Children[canvas.Children.Count - 1] as Shape;
                undoDrawings.Add(s);
                drawings.RemoveAt(drawings.Count - 1);

                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }

        }

        public void RedoDrawing()
        {
            if (newDrawings.Count != 0)
            {
                if (undoDrawings.Count > 0)
                {
                    drawings.Add(new Drawing(canvas.Children.Count));
                    canvas.Children.Add(undoDrawings[undoDrawings.Count - 1]);
                    undoDrawings.RemoveAt(undoDrawings.Count - 1);
                }
                else if (allUndos.Count > 0 && allUndos[allUndos.Count - 1].Count > 0)
                {
                    int x = allUndos[allUndos.Count - 1].Count - 1;

                    drawings.Add(new Drawing(canvas.Children.Count));
                    canvas.Children.Add(allUndos[allUndos.Count - 1][x]);
                    allUndos[allUndos.Count - 1].RemoveAt(x);

                }
            }
            
        }

        public void ContinueDrawing(Shape element)
        {
            AddForeGround(element);
        }

        public void EndDrawing(Shape element)
        {
            AddForeGround(element);
            drawings[drawings.Count - 1].EndIndex = canvas.Children.Count;
            
        }

        public void SelectDrawing(bool reverse = false)
        {
            if (drawings.Count > 0)
            {
                if (selectedDrawingIndex >= 0)
                {
                    UnhightLightDrawing(drawings[selectedDrawingIndex]);
                }

                if (reverse)
                {
                    selectedDrawingIndex--;
                    if (selectedDrawingIndex < 0)
                    {
                        selectedDrawingIndex = drawings.Count - 1;
                    }
                }
                else
                {
                    selectedDrawingIndex++;
                    if (selectedDrawingIndex >= drawings.Count)
                    {
                        selectedDrawingIndex = 0;
                    }
                }

                HightlightDrawing(drawings[selectedDrawingIndex]);
            }
        }

        public void UnhightLightDrawing(Drawing d)
        {
            int end = d.EndIndex;
            if (d.IsComplete() && d.StartIndex == d.EndIndex)
            {
                end++;
            }
            for (int i = d.StartIndex; i < end; i++)
            {
                Line l = canvas.Children[i] as Line;
                l.Stroke = foregroundBrush;
            }
        }

        public void HightlightDrawing(Drawing d)
        {
            int end = d.EndIndex;
            if (d.IsComplete() && d.StartIndex == d.EndIndex)
            {
                end++;
            }
            for (int i = d.StartIndex; i < end; i++)
            {
                Line l = canvas.Children[i] as Line;
                l.Stroke = highlightBrush;
            }
        }

        public void SaveAsPNG(string filename)
        {
            ConvertToImage.createPNG(canvas);
        }
    }
}