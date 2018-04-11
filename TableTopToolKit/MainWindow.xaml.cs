using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Windows.Shapes;

namespace TableTopToolKit
{
    public partial class MainWindow : Window
    {
        private App main;
        private const string ICON_IMAGES_DIRECTORY = @"..\..\imgs\icons\";
        private Point startDragMousePosition;
        private bool iconViewHasSwitched = true;

        public Image SelectedIcon
        {
            get { return (Image)GetValue(SelectedIconProperty); }
            set { SetValue(SelectedIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIconProperty =
            DependencyProperty.Register("SelectedIcon", typeof(Image), typeof(MainWindow), new PropertyMetadata(null));

        public ObservableCollection<Image> Icons { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void LoadIconImages()
        {
            List<Image> iconImages = new List<Image>();
            string[] imagePaths = Directory.GetFiles(ICON_IMAGES_DIRECTORY);

            foreach (string imagePath in imagePaths)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(imagePath, UriKind.Relative);
                bmp.EndInit();
                Image image = new Image
                {
                    Source = bmp,
                    Height = 32,
                    Width = 32
                };
                iconImages.Add(image);
            }
            Icons = new ObservableCollection<Image>(iconImages);
            SelectedIcon = Icons[0];
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            main = Application.Current as App;
            DataContext = this;
            LoadIconImages();
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
            else if (e.RightButton == MouseButtonState.Pressed && SelectedIcon != null)
            {
                main.PlaceIcon(currentPoint, SelectedIcon);
            }
        }

        private void OnCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(Canvas);
            main.CurrentTool.MouseExit(currentPoint, e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.G:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        main.Command(App.Controls.ToggleGrid);
                    }
                    break;

                case Key.P:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        main.Command(App.Controls.PrintPreview);
                    }
                    break;

                case Key.C:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        main.Command(App.Controls.ClearCanvas);
                    }
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
            else if (item.Equals(IconToggleButton))
            {
                IconSwitch();
            }
            else if (item.Equals(GridDotsToggleButton))
            {
                main.Command(App.Controls.ToggleGridMode);
            }
            else if (item.Equals(RestoreLastSessionButton))
            {
                main.Command(App.Controls.LoadPreviousAutoSave);
            }
            else if (item.Equals(SaveAs))
            {
                main.Command(App.Controls.SaveAs);
            }
            else if (item.Equals(LoadFile))
            {
                main.Command(App.Controls.LoadFile);
            }
        }

        private void IconSwitch()
        {
            if (!iconViewHasSwitched)
            {
                {
                    Image img = new Image();
                    img.Height = 32;
                    img.Width = 32;

                    //Grid Toggle Button
                    img.Source = new BitmapImage(new Uri(@"../../imgs/menu_icons/toggle_grid.png", UriKind.Relative));
                    ToggleGridButton.Content = img;
                }
                {
                    Image img = new Image();
                    img.Height = 32;
                    img.Width = 32;
                    //Print Preview
                    img.Source = new BitmapImage(new Uri(@"../../imgs/menu_icons/print_preview.png", UriKind.Relative));

                    PrintPreviewButton.Content = img;
                }
                {
                    Image img = new Image();
                    img.Height = 32;
                    img.Width = 32;

                    //Clear Canvas
                    img.Source = new BitmapImage(new Uri(@"../../imgs/menu_icons/clear_canvas.png", UriKind.Relative));

                    ToggleClearCanvas.Content = img;
                }
                {
                    Image img = new Image();
                    img.Height = 32;
                    img.Width = 32;

                    //Undo
                    img.Source = new BitmapImage(new Uri(@"../../imgs/menu_icons/undo.png", UriKind.Relative));

                    ToggleUndoButton.Content = img;
                }
                {
                    Image img = new Image();
                    img.Height = 32;
                    img.Width = 32;

                    //Redo
                    img.Source = new BitmapImage(new Uri(@"../../imgs/menu_icons/redo.png", UriKind.Relative));

                    ToggleRedoButton.Content = img;
                }
                {
                    Image img = new Image();
                    img.Height = 32;
                    img.Width = 32;

                    //Tools
                    //Pencil Tool
                    img.Source = new BitmapImage(new Uri(@"../../imgs/menu_icons/pencil.png", UriKind.Relative));

                    ToggleDrawPencilButton.Content = img;
                }
                {
                    Image img = new Image();
                    img.Height = 32;
                    img.Width = 32;
                    //Line
                    img.Source = new BitmapImage(new Uri(@"../../imgs/menu_icons/line.png", UriKind.Relative));

                    ToggleDrawLineButton.Content = img;
                }
                {
                    Image img = new Image();
                    img.Height = 32;
                    img.Width = 32;
                    //Line Erase
                    img.Source = new BitmapImage(new Uri(@"../../imgs/menu_icons/erase.png", UriKind.Relative));

                    ToggleLineEraser.Content = img;
                }
                {
                    Image img = new Image();
                    img.Height = 32;
                    img.Width = 32;
                    //Rectangle
                    img.Source = new BitmapImage(new Uri(@"../../imgs/menu_icons/rectangle.png", UriKind.Relative));

                    ToggleDrawRectangleButton.Content = img;
                }
                {
                    Image img = new Image();
                    img.Height = 32;
                    img.Width = 32;
                    // Ruler
                    img.Source = new BitmapImage(new Uri(@"../../imgs/menu_icons/ruler.png", UriKind.Relative));

                    ToggleDrawRectangleButton.Content = img;
                }

                iconViewHasSwitched = true;
            }
            else
            {
                //Canvas Operations
                ToggleGridButton.Content = "Toggle Grid";
                PrintPreviewButton.Content = "Print Preview";
                ToggleClearCanvas.Content = "Clear Canvas";
                ToggleUndoButton.Content = "Undo";
                ToggleRedoButton.Content = "Redo";

                //Tools
                ToggleDrawPencilButton.Content = "Pencil";
                ToggleDrawLineButton.Content = "Line";
                ToggleLineEraser.Content = "Line Erase";
                ToggleDrawRectangleButton.Content = "Rectangle";
                ToggleRulerButton.Content = "Ruler";

                iconViewHasSwitched = false;
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
            else if (button.Equals(ToggleLineEraser))
            {
                main.Command(App.Controls.SelectEraserTool);
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

        private void OnIconDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Image)))
            {
                Image icon = e.Data.GetData(typeof(Image)) as Image;
                Point dropPosition = e.GetPosition(Canvas);
                main.PlaceIcon(dropPosition, icon);
            }
        }
    }
}