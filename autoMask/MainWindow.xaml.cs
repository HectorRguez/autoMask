using Microsoft.Win32;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace autoMask
{
    public partial class MainWindow : Window
    {
        // Default configuraion
        public static string maskSavePath = AppDomain.CurrentDomain.BaseDirectory + "\\auto_mask.cif";
        public static string maskOpenPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string wireReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\wire_report.csv";
        public static string configMaskPath = AppDomain.CurrentDomain.BaseDirectory + "\\config_mask.csv";
        public static bool viewChamber = false;
        public static bool optWire = true;
        public static bool eqWire = true;
        public static int sputteringHeight = ConfigWindow.SPUTTERING_HEIGHT_NM;
        public static int sputteringMaterial = ConfigWindow.GOLD_CONDUCTANCE_S;
        private string Author = "Hector Rodriguez Rodriguez TFM - UPM - CEI";

        public CifFile file;
        private Canvas cv;
        private ScaleTransform cv_st = new ScaleTransform();
        private TranslateTransform cv_tt = new TranslateTransform();
        public int cvWidth = 1180;
        public int cvHeight = 1000;

        public MainWindow()
        {
            this.InitializeComponent();
            this.cv = new Canvas();
            file = new CifFile(this.Author, new List<int>() { 0 }, MainWindow.maskSavePath);
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
            file = new CifFile(openFileDialog.FileName);
            file.Read();
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
            this.cv = canvas;
            this.cv.CacheMode = (CacheMode)new BitmapCache(100.0);
            this.container.Children.Clear();
            this.container.Children.Add((UIElement)this.cv);
            this.file.Draw(this.cv);
      
            if (!MainWindow.viewChamber)
                return;
            foreach (Element subElement in this.file.MainElement.SubElements)
                subElement.DrawChamber(this.cv, auxFun.Scale(subElement.Position, this.cv));
        }
    }
}
