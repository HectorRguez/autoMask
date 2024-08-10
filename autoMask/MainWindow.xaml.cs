﻿using Microsoft.Win32;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace autoMask
{
    public partial class MainWindow : Window, IComponentConnector
    {
        private double zoomMax = 0.15;
        private double zoomMin = 0.001;
        private double zoomSpeed = 1E-05;
        private double zoom = 0.01;
        protected bool isDragging;
        private System.Windows.Point clickPosition;
        private System.Windows.Point previousPosition;
        public static string maskSavePath = AppDomain.CurrentDomain.BaseDirectory + "\\auto_mask.cif";
        public static string maskOpenPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string wireReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\wire_report.csv";
        public static string configMaskPath = AppDomain.CurrentDomain.BaseDirectory + "\\config_mask.csv";
        public static bool viewWafer = true;
        public static bool viewChamber = true;
        public static bool viewGrid = true;
        public static bool optWire = true;
        public static bool eqWire = true;
        public static int sputteringHeight = ConfigWindow.SPUTTERING_HEIGHT_NM;
        public static int sputteringMaterial = ConfigWindow.GOLD_CONDUCTANCE_S;
        private string Author = "Hector Rodriguez Rodriguez TFM - UPM - CEI";
        public CifFile file;
        private Canvas cv;
        private ScaleTransform cv_st = new ScaleTransform();
        private TranslateTransform cv_tt = new TranslateTransform();
        private int cvWidth = 1180;
        private int cvHeight = 1000;

        public MainWindow()
        {
            this.InitializeComponent();
            this.cv_st.ScaleX = this.zoom;
            this.cv_st.ScaleY = this.zoom;
            this.cv_st.CenterX = (double)(this.cvWidth / 2);
            this.cv_st.CenterY = (double)(this.cvHeight / 2);
            this.previousPosition.X = 0.0;
            this.previousPosition.Y = 0.0;
        }

        private void cv_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.zoom += this.zoomSpeed * (double)e.Delta;
            if (this.zoom < this.zoomMin)
                this.zoom = this.zoomMin;
            if (this.zoom > this.zoomMax)
                this.zoom = this.zoomMax;
            System.Windows.Point position = e.GetPosition((IInputElement)this.cv);
            if (this.zoom > 1.0)
            {
                this.cv_st.CenterX = position.X;
                this.cv_st.CenterY = position.Y;
                this.cv_st.ScaleX = this.zoom;
                this.cv_st.ScaleY = this.zoom;
            }
            else
            {
                this.cv_st.ScaleX = this.zoom;
                this.cv_st.ScaleY = this.zoom;
            }
        }

        private void cv_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = true;
            this.clickPosition = e.GetPosition((IInputElement)(this.Parent as UIElement));
            this.cv.CaptureMouse();
            this.Cursor = Cursors.Hand;
        }

        private void cv_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = false;
            this.previousPosition.X = this.cv_tt.X;
            this.previousPosition.Y = this.cv_tt.Y;
            this.cv.ReleaseMouseCapture();
            this.Cursor = Cursors.Arrow;
        }

        private void cv_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.isDragging)
                return;
            System.Windows.Point position = e.GetPosition((IInputElement)(this.Parent as Window));
            this.cv_tt.X = this.previousPosition.X + (position.X - this.clickPosition.X);
            this.cv_tt.Y = this.previousPosition.Y + (position.Y - this.clickPosition.Y);
        }

        private void Button_New_File_Click(object sender, RoutedEventArgs e)
        {
            new MaskWindow().ShowDialog();
        }

        private void Button_Read_File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Mask files (*.cif)|*.cif;";
            openFileDialog.InitialDirectory = MainWindow.maskOpenPath;
            bool? nullable = openFileDialog.ShowDialog();
            bool flag = true;
            if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
                return;
            this.cv_st.ScaleX = 0.005;
            this.cv_st.ScaleY = 0.005;
            new CifFile(openFileDialog.FileName).Read();
            this.Draw();
            this.WireReport();
        }

        public void WireReport()
        {
            this.wireReports.Items.Clear();
            TabItem newItem1 = new TabItem();
            newItem1.Header = (object)"Document";
            newItem1.BorderThickness = new Thickness(2.0);
            TextBox textBox = new TextBox();
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            textBox.BorderThickness = new Thickness(0.0);
            textBox.Text = this.file.Data;
            newItem1.Content = (object)textBox;
            this.wireReports.Items.Add((object)newItem1);
            this.wireReports.SelectedIndex = 0;
            for (int index = 0; index < this.file.MainElement.SubElements.Count<Element>(); ++index)
            {
                TabItem newItem2 = new TabItem();
                newItem2.Header = (object)("Element " + index.ToString() + " wires");
                newItem2.BorderThickness = new Thickness(2.0);
                DataGrid dataGrid = new DataGrid();
                dataGrid.AutoGenerateColumns = true;
                dataGrid.ItemsSource = (IEnumerable)CifAuto.WireReport(this.file.MainElement.SubElements[index], MainWindow.wireReportPath);
                newItem2.Content = (object)dataGrid;
                this.wireReports.Items.Add((object)newItem2);
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new ConfigWindow().ShowDialog();
        }

        public void Generate_Chips()
        {
            this.file = new CifFile(this.Author, new List<int>(){0}, MainWindow.maskSavePath);
            List<Element> els = new List<Element>();
            for (int index = 0; index < MaskWindow.Chips.Count<Chip>(); ++index)
            {
                Element el = new Element();
                el.Index = index;
                el.Name = "Element " + index.ToString();
                CifAuto.Electrodes(el, MaskWindow.Chips[index]);
                CifAuto.Pads2sides(el, MaskWindow.Chips[index]);
                if (MainWindow.optWire)
                    CifAuto.VariableWires(el, MaskWindow.Chips[index], MainWindow.eqWire);
                else
                    CifAuto.Wires(el, MaskWindow.Chips[index]);
                CifAuto.Squares(el, MaskWindow.Chips[index]);
                CifAuto.GroundReference(el, MaskWindow.Chips[index]);
                els.Add(el);
            }
            this.file.MainElement.Name = "Main Element";
            this.file.MainElement.Index = els.Count;
            CifAuto.ElementPlace(this.file, els, MaskWindow.Chips[0]);
            this.file.Save();
        }

        public void Draw()
        {
            Canvas canvas = new Canvas();
            canvas.Width = (double)this.cvWidth;
            canvas.Height = (double)this.cvHeight;
            canvas.HorizontalAlignment = HorizontalAlignment.Center;
            canvas.VerticalAlignment = VerticalAlignment.Center;
            canvas.Background = (Brush)new BrushConverter().ConvertFrom((object)"#f8f5f2");
            this.cv = canvas;
            this.cv.RenderTransform = (Transform)new TransformGroup()
            {
                Children = {(Transform) this.cv_st, (Transform) this.cv_tt}
            };
            this.cv.CacheMode = (CacheMode)new BitmapCache(100.0);
            this.container.Children.Clear();
            this.container.Children.Add((UIElement)this.cv);
            this.file.Draw(this.cv);
            if (MainWindow.viewWafer)
                this.file.DrawWafer(this.cv);
            if (MainWindow.viewGrid)
                this.file.DrawGrid(this.cv);
            if (!MainWindow.viewChamber)
                return;
            foreach (Element subElement in this.file.MainElement.SubElements)
                subElement.DrawChamber(this.cv, auxFun.Scale(subElement.Position, this.cv));
        }
    }
}
