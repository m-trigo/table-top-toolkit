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
        private int selectedDrawingIndex;

        public Brush defaultBrush;
        public Brush highlightBrush;

        public CanvasDrawings(Canvas source)
        {
            canvas = source;
            drawings = new List<Drawing>();
            selectedDrawingIndex = -1;
            defaultBrush = Brushes.Black;
            highlightBrush = Brushes.White;
        }

        public void StartDrawing(UIElement element)
        {
            drawings.Add(new Drawing(canvas.Children.Count));
            canvas.Children.Add(element);
        }

        public void ContinueDrawing(UIElement element)
        {
            canvas.Children.Add(element);
        }

        public void EndDrawing(UIElement element)
        {
            canvas.Children.Add(element);
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
            for (int i = d.StartIndex; i < d.EndIndex; i++)
            {
                Line l = canvas.Children[i] as Line;
                l.Stroke = defaultBrush;
            }
        }

        public void HightlightDrawing(Drawing d)
        {
            for (int i = d.StartIndex; i < d.EndIndex; i++)
            {
                Line l = canvas.Children[i] as Line;
                l.Stroke = highlightBrush;
            }
        }
    }
}