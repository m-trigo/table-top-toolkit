using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TableTopToolKit
{
    public static class ConvertToImage
    {
        public const string DATA_PATH = "./";

        public static void createPNG(Canvas canvas)
        {
            try
            {
                double dpi = 96d;
                Rect rect = VisualTreeHelper.GetDescendantBounds(canvas);
                RenderTargetBitmap target = new RenderTargetBitmap((int)rect.Width, (int)rect.Height, dpi, dpi, PixelFormats.Default);
                target.Render(canvas);
                DrawingVisual drawing = new DrawingVisual();

                //retrieve drawing context to create a new drawing

                DrawingContext drawingContext = drawing.RenderOpen();
                VisualBrush brush = new VisualBrush(canvas);

                drawingContext.DrawRectangle(brush, null, new Rect(new Point(), rect.Size));

                MemoryStream memStream = new MemoryStream();

                PngBitmapEncoder png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(target));
                png.Save(memStream);
                memStream.Close();
                File.WriteAllBytes($"{DATA_PATH}image.png", memStream.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void printPreview(Canvas canvas)
        {
            PrintDialog pDialog = new PrintDialog();
            pDialog.ShowDialog();

            pDialog.PrintVisual(canvas, "Canvas");
        }
    }
}