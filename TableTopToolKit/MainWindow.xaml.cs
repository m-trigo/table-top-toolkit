﻿using System;
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
            }
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item.Equals(PrintMenuItemButton))
            {
                main.Command(App.Controls.Print);
            }
            else if (item.Equals(IconToggleMenuItemButton))
            {
                main.Command(App.Controls.ToggleIconView);
            }
            else if (item.Equals(RestoreLastSessionButton))
            {
                main.Command(App.Controls.LoadPreviousAutoSave);
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

        private void IconListButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Image icon = button.Content as Image;
            main.CommandWithButton(App.Controls.SelectIcon, icon);
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            main.Command(App.Controls.AutoSave);
        }
    }
}