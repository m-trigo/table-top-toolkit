using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableTopToolKit
{
    public partial class MainWindow : Window
    {
        private App main;

        public MainWindow()
        {
            InitializeComponent();
            main = Application.Current as App;
            main.InitializeCanvasDrawing(Canvas);

            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
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
            main.CurrentTool.MouseDown(currentPoint, e);
        }

        private void OnCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(Canvas);
            main.CurrentTool.MouseExit(currentPoint, e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    main.Command(App.Controls.AutoSave);
                    break;

                case Key.F2:
                    main.Command(App.Controls.LoadPreviousAutoSave);
                    break;

                case Key.Z:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        main.Command(App.Controls.Undo);
                    }
                    break;

                case Key.Y:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        main.Command(App.Controls.Redo);
                    }
                    break;

                case Key.Space:
                    main.Command(App.Controls.ToggleGrid);
                    break;

                case Key.D1:
                    main.Command(App.Controls.SelectPencilTool);
                    break;

                case Key.D2:
                    main.Command(App.Controls.SelectLineTool);
                    break;

                case Key.D3:
                    main.Command(App.Controls.SelectRectangleTool);
                    break;

                case Key.D4:  // maybe come up with a better key?
                    main.Command(App.Controls.SelectEraserTool);
                    break;

            }
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item.Equals(PrintMenuItemButton))
            {
                main.Command(App.Controls.Print);
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Equals(ToggleGridButton))
            {
                main.Command(App.Controls.ToggleGrid);
            }
            else if (button.Equals(PrintPreviewButton))
            {
                main.Command(App.Controls.PrintPreview);
            }
            else if (button.Equals(ToggleUndoButton))
            {
                main.Command(App.Controls.Undo);
            }
            else if (button.Equals(ToggleRedoButton))
            {
                main.Command(App.Controls.Redo);
            }
            else if (button.Equals(ToggleDrawPencilButton))
            {
                main.Command(App.Controls.SelectPencilTool);
            }
            else if (button.Equals(ToggleDrawLineButton))
            {
                main.Command(App.Controls.SelectLineTool);
            }
            else if (button.Equals(ToggleDrawRectangleButton))
            {
                main.Command(App.Controls.SelectRectangleTool);
            }
            else if (button.Equals(ToggleClearCanvas))
            {
                main.Command(App.Controls.ClearCanvas);
            }
        }
    }
}