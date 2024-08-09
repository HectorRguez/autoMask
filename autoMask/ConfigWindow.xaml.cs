// Decompiled with JetBrains decompiler
// Type: autoMask.ConfigWindow
// Assembly: autoMask, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93C42831-928E-4BBC-8F9F-03F21363630D
// Assembly location: D:\TFM\automask\autoMask.dll

using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

#nullable enable
namespace autoMask
{
    public partial class ConfigWindow : Window, IComponentConnector
    {
        public static int GOLD_CONDUCTANCE_S = 41000000;
        public static int PALLADIUM_CONDUCTANCE_S = 10000000;
        public static int SPUTTERING_HEIGHT_NM = 30;
        private bool isLoading = true;
        internal
#nullable disable
        TextBox savePath;
        internal TextBox reportPath;
        internal CheckBox siWaferView;
        internal CheckBox miChambView;
        internal CheckBox gridView;
        internal ComboBox MaterialSelector;
        internal TextBox ConducContainer;
        internal TextBox HeightContainer;
        internal CheckBox optWires;
        internal CheckBox eqWires;
        internal Button saveConfig;
        private bool _contentLoaded;

        public ConfigWindow()
        {
            this.InitializeComponent();
            this.isLoading = true;
            this.siWaferView.IsChecked = new bool?(MainWindow.viewWafer);
            this.miChambView.IsChecked = new bool?(MainWindow.viewChamber);
            this.gridView.IsChecked = new bool?(MainWindow.viewGrid);
            this.optWires.IsChecked = new bool?(MainWindow.optWire);
            this.eqWires.IsChecked = new bool?(MainWindow.eqWire);
            this.savePath.Text = MainWindow.maskSavePath;
            this.reportPath.Text = MainWindow.wireReportPath;
            this.ConducContainer.Text = (MainWindow.sputteringMaterial / 1000000).ToString();
            this.HeightContainer.Text = MainWindow.sputteringHeight.ToString();
            this.MaterialSelector.SelectionChanged += new SelectionChangedEventHandler(this.ComboBox_Selected);
            this.isLoading = false;
        }

        private void Button_Wire_Report_Click(
#nullable enable
        object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog();
            commonOpenFileDialog.InitialDirectory = "c:\\";
            commonOpenFileDialog.IsFolderPicker = true;
            commonOpenFileDialog.InitialDirectory = MainWindow.maskOpenPath;
            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                MainWindow.wireReportPath = commonOpenFileDialog.FileName + "\\report.csv";
                this.reportPath.Text = MainWindow.wireReportPath;
            }
            this.Activate();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog();
            commonOpenFileDialog.InitialDirectory = "c:\\";
            commonOpenFileDialog.IsFolderPicker = true;
            commonOpenFileDialog.InitialDirectory = MainWindow.maskOpenPath;
            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                MainWindow.maskSavePath = commonOpenFileDialog.FileName + "\\auto_mask.cif";
                this.savePath.Text = MainWindow.maskSavePath;
            }
            this.Activate();
        }

        private void Button_Save_Changes_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.viewWafer = this.siWaferView.IsChecked.Value;
            MainWindow.viewChamber = this.miChambView.IsChecked.Value;
            MainWindow.viewGrid = this.gridView.IsChecked.Value;
            MainWindow.maskSavePath = this.savePath.Text;
            MainWindow.wireReportPath = this.reportPath.Text;
            MainWindow.optWire = this.optWires.IsChecked.Value;
            MainWindow.eqWire = this.eqWires.IsChecked.Value;
            MainWindow.sputteringHeight = int.Parse(this.HeightContainer.Text);
            MainWindow.sputteringMaterial = int.Parse(this.ConducContainer.Text) * 1000000;
            this.saveConfig.IsEnabled = false;
            MainWindow window = Window.GetWindow((DependencyObject)Application.Current.MainWindow) as MainWindow;
            if (window.file == null)
                return;
            window.Draw();
            window.WireReport();
        }

        private void ComboBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (((Selector)sender).SelectedIndex == 0)
                this.ConducContainer.Text = (ConfigWindow.GOLD_CONDUCTANCE_S / 1000000).ToString();
            else
                this.ConducContainer.Text = (ConfigWindow.PALLADIUM_CONDUCTANCE_S / 1000000).ToString();
        }

        private void Value_Changed(object sender, EventArgs e)
        {
            if (this.isLoading)
                return;
            this.saveConfig.IsEnabled = true;
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "8.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/autoMask;component/configwindow.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "8.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId,
#nullable disable
        object target)
        {
            switch (connectionId)
            {
                case 1:
                    ((ButtonBase)target).Click += new RoutedEventHandler(this.Button_Save_Click);
                    break;
                case 2:
                    this.savePath = (TextBox)target;
                    this.savePath.TextChanged += new TextChangedEventHandler(this.Value_Changed);
                    break;
                case 3:
                    ((ButtonBase)target).Click += new RoutedEventHandler(this.Button_Wire_Report_Click);
                    break;
                case 4:
                    this.reportPath = (TextBox)target;
                    this.reportPath.TextChanged += new TextChangedEventHandler(this.Value_Changed);
                    break;
                case 5:
                    this.siWaferView = (CheckBox)target;
                    this.siWaferView.Click += new RoutedEventHandler(this.Value_Changed);
                    break;
                case 6:
                    this.miChambView = (CheckBox)target;
                    this.miChambView.Click += new RoutedEventHandler(this.Value_Changed);
                    break;
                case 7:
                    this.gridView = (CheckBox)target;
                    this.gridView.Click += new RoutedEventHandler(this.Value_Changed);
                    break;
                case 8:
                    this.MaterialSelector = (ComboBox)target;
                    break;
                case 9:
                    this.ConducContainer = (TextBox)target;
                    this.ConducContainer.TextChanged += new TextChangedEventHandler(this.Value_Changed);
                    break;
                case 10:
                    this.HeightContainer = (TextBox)target;
                    this.HeightContainer.TextChanged += new TextChangedEventHandler(this.Value_Changed);
                    break;
                case 11:
                    this.optWires = (CheckBox)target;
                    this.optWires.Click += new RoutedEventHandler(this.Value_Changed);
                    break;
                case 12:
                    this.eqWires = (CheckBox)target;
                    this.eqWires.Click += new RoutedEventHandler(this.Value_Changed);
                    break;
                case 13:
                    this.saveConfig = (Button)target;
                    this.saveConfig.Click += new RoutedEventHandler(this.Button_Save_Changes_Click);
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }
    }
}
