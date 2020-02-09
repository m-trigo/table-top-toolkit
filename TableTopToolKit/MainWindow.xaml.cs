using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TableTopToolKit
{
    public partial class MainWindow : Window
    {
        private App main;
        private const string ICON_IMAGES_DIRECTORY = @"..\..\imgs\icons_alt\"; // Fix this later
        private const string ALT_ICON_IMAGES_DIRECTORY = @"..\..\imgs\icons\";
        private Point startDragMousePosition;
        private const double ZOOM_RATE = 0.05; // Fix this later
        public double ZoomLevel
        {
            get { return (double)GetValue(ZoomLevelProperty); }
            set
            {
                if (value < 0.5)
                {
                    value = 0.5;
                }
                else if (value > 1.0)
                {
                    value = 1.0;
                }
                SetValue(ZoomLevelProperty, value);
            }
        }

        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(double), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            ZoomLevel = 1;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            main = Application.Current as App;
            DataContext = this;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            main.InitializeCanvasDrawing(Canvas);
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(Canvas);
            main.CurrentTool.MouseMove(currentPoint, e);
        }

        private void OnCanvasMouseUp(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(Canvas);
            main.CurrentTool.MouseUp(currentPoint, e);
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point currentPoint = e.GetPosition(Canvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                main.CurrentTool.MouseDown(currentPoint, e);
            }
        }

        private void OnCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(Canvas);
            main.CurrentTool.MouseExit(currentPoint, e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Repeatable
            switch (e.Key)
            {
                case Key.Z:
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    main.Command(App.Controls.Undo);
                    return;
                }
                break;

                case Key.Y:
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    main.Command(App.Controls.Redo);
                    return;
                }
                break;
            }

            if (e.IsRepeat)
            {
                return;
            }

            // Non-Repeatable
            switch (e.Key)
            {
                case Key.G:
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    main.Command(App.Controls.ToggleGridDisplay);
                }
                break;

                case Key.P:
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    // Temporary fix-hack
                    double zoom = ZoomLevel;
                    ZoomLevel = 1;
                    main.Command(App.Controls.PrintPreview);
                    ZoomLevel = zoom;
                }
                break;

                case Key.C:
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    main.Command(App.Controls.ClearCanvas);
                }
                break;

                case Key.S:
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    main.Command(App.Controls.SaveAs);
                }
                break;

                case Key.Add:
                case Key.OemPlus:
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    ZoomIn();
                }
                break;

                case Key.Subtract:
                case Key.OemMinus:
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    ZoomOut();
                }
                break;

                case Key.T:
                if ( Keyboard.IsKeyDown( Key.LeftCtrl ) || Keyboard.IsKeyDown( Key.RightCtrl ) )
                {
                    main.Command(App.Controls.ToggleGridMode);
                }
                break;

                case Key.D1:
                case Key.NumPad1:
                main.Command(App.Controls.SelectPencilTool);
                break;

                case Key.D2:
                case Key.NumPad2:
                main.Command(App.Controls.SelectLineTool);
                break;

                case Key.D3:
                case Key.NumPad3:
                main.Command(App.Controls.SelectRectangleTool);
                break;

                case Key.D4:
                case Key.NumPad4:
                main.Command(App.Controls.SelectPencilEraser);
                break;

                case Key.D5:
                case Key.NumPad5:
                main.Command(App.Controls.SelectEraserTool);
                break;

                case Key.D6:
                case Key.NumPad6:
                main.Command(App.Controls.SelectRulerTool);
                break;
            }
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (item.Equals(NewMenuItem))
            {
                main.Command(App.Controls.ClearCanvas);
            }
            else if (item.Equals(OpenMenuItem))
            {
                main.Command(App.Controls.LoadFile);
            }
            else if (item.Equals(SaveMenuItem))
            {
                main.Command(App.Controls.SaveAs);
            }
            else if (item.Equals(PrintMenuItem))
            {
                // Temporary fix-hack
                double zoom = ZoomLevel;
                ZoomLevel = 1;
                main.Command(App.Controls.Print);
                ZoomLevel = zoom;
            }
            else if (item.Equals(RestorePreviousSessionMenuItem))
            {
                main.Command(App.Controls.LoadPreviousAutoSave);
            }
            else if (item.Equals(ExitMenuItem))
            {
                Close();
            }
            else if (item.Equals(UndoMenuItem))
            {
                main.Command( App.Controls.Undo );
            }
            else if (item.Equals(RedoMenuItem))
            {
                main.Command( App.Controls.Redo );
            }
            else if (item.Equals(ZoomInMenuitem))
            {
                ZoomIn();
            }
            else if (item.Equals(ZoomOutMenuItem))
            {
                ZoomOut();
            }
            else if (item.Equals(ToggleGridTypeMenuItem))
            {
                main.Command( App.Controls.ToggleGridMode );
            }
            else if (item.Equals(ToggleGridDisplayMenuItem))
            {
                main.Command( App.Controls.ToggleGridMode );
            }
            else if (item.Equals(PencilMenuItem))
            {
                main.Command(App.Controls.SelectPencilTool);
            }
            else if (item.Equals(LineMenuItem))
            {
                main.Command(App.Controls.SelectLineTool);
            }
            else if (item.Equals(RectangleMenuItem))
            {
                main.Command(App.Controls.SelectRectangleTool);
            }
            else if (item.Equals(PencilEraserMenuItem))
            {
                main.Command(App.Controls.SelectPencilEraser);
            }
            else if (item.Equals(LineEraserMenuItem))
            {
                main.Command(App.Controls.SelectEraserTool);
            }
            else if (item.Equals(RulerMenuItem))
            {
                main.Command(App.Controls.SelectRulerTool);
            }
            else if (item.Equals(StandardThemeButton))
            {
                main.Command(App.Controls.SetStandardTheme);
            }
            else if (item.Equals(InkThemeButton))
            {
                main.Command(App.Controls.SetInkTheme);
            }
            else if (item.Equals(BlueprintThemeButton))
            {
                main.Command(App.Controls.SetBlueprintTheme);
            }
        }
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.Equals(PrintButton))
            {
                // Temporary fix-hack
                double zoom = ZoomLevel;
                ZoomLevel = 1;
                main.Command(App.Controls.PrintPreview);
                ZoomLevel = zoom;
            }
            else if (button.Equals(PencilButton))
            {
                main.Command(App.Controls.SelectPencilTool);
            }
            else if (button.Equals(LineButton))
            {
                main.Command(App.Controls.SelectLineTool);
            }
            else if (button.Equals(RectangleButton))
            {
                main.Command(App.Controls.SelectRectangleTool);
            }
            else if (button.Equals(PencilEraserButton))
            {
                main.Command( App.Controls.SelectPencilEraser );
            }
            else if (button.Equals(LineEraserButton))
            {
                main.Command(App.Controls.SelectEraserTool);
            }
            else if (button.Equals(RulerButton))
            {
                main.Command(App.Controls.SelectRulerTool);
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            main.Command(App.Controls.AutoSave);
        }

        private void OnIconListPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            startDragMousePosition = e.GetPosition(null); // absolute
        }

        private void OnIconListMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(null); // absolute
            Vector mouseDragDistance = mousePosition - startDragMousePosition;
            double adx = Math.Abs(mouseDragDistance.X);
            double ady = Math.Abs(mouseDragDistance.Y);
            if (e.LeftButton == MouseButtonState.Pressed
            && (adx > SystemParameters.MinimumHorizontalDragDistance || ady > SystemParameters.MinimumVerticalDragDistance))
            {
                ListBox iconList = sender as ListBox;
                object selectedItem = iconList.SelectedItem;
                if (selectedItem == null)
                {
                    return;
                }

                DataObject target = new DataObject(typeof(Image), selectedItem);
                DragDrop.DoDragDrop(iconList, target, DragDropEffects.Copy);
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                e.Handled = true;
                if (e.Delta > 0)
                {
                    ZoomIn();
                }
                else if (e.Delta < 0)
                {
                    ZoomOut();
                }
            }
        }

        private void ZoomIn()
        {
            scrollViewer.IsEnabled = false;
            ZoomLevel += ZOOM_RATE;
            scrollViewer.IsEnabled = true;
        }

        private void ZoomOut()
        {
            scrollViewer.IsEnabled = false;
            ZoomLevel -= ZOOM_RATE;
            scrollViewer.IsEnabled = true;
        }
    }
}