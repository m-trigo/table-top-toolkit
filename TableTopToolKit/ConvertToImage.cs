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

        public static void SaveToPng(Canvas canvas)
        {
            try
            {
                double dpi = 96;
                RenderTargetBitmap target = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, dpi, dpi, PixelFormats.Default);
                canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
                canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));
                target.Render(canvas);

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

        public static void Preview(Canvas canvas)
        {
            double dpi = 96;
            RenderTargetBitmap target = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, dpi, dpi, PixelFormats.Default);
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));
            target.Render(canvas);
            new PrintPreviewWindow(target).ShowDialog();
        }

        public static void Print(Canvas canvas)
        {
            PrintDialog pDialog = new PrintDialog();
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));
            pDialog.ShowDialog();
            pDialog.PrintVisual(canvas, "Canvas");
        }
    }
}