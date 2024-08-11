﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;

namespace autoMask
{
    public partial class ConfigWindow : Window
    {
        public static int GOLD_CONDUCTANCE_S = 41000000;
        public static int PALLADIUM_CONDUCTANCE_S = 10000000;
        public static int SPUTTERING_HEIGHT_NM = 30;
        private bool isLoading = true;

        public ConfigWindow()
        {
            this.InitializeComponent();
            this.isLoading = true;
            this.miChambView.IsChecked = new bool?(MainWindow.viewChamber);
            this.optWires.IsChecked = new bool?(MainWindow.optWire);
            this.eqWires.IsChecked = new bool?(MainWindow.eqWire);
            this.savePath.Text = MainWindow.maskSavePath;
            this.reportPath.Text = MainWindow.wireReportPath;
            this.ConducContainer.Text = (MainWindow.sputteringMaterial / 1000000).ToString();
            this.HeightContainer.Text = MainWindow.sputteringHeight.ToString();
            this.MaterialSelector.SelectionChanged += new SelectionChangedEventHandler(this.ComboBox_Selected);
            this.isLoading = false;
        }

        private void Button_Wire_Report_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog folderOpenFileDialog = new OpenFolderDialog();
            folderOpenFileDialog.InitialDirectory = MainWindow.maskOpenPath;
            if (folderOpenFileDialog.ShowDialog() == true)
            {
                MainWindow.wireReportPath = folderOpenFileDialog.FolderName + "\\report.csv";
                this.reportPath.Text = folderOpenFileDialog.FolderName;
            }
            this.Activate();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog folderOpenFileDialog = new OpenFolderDialog();
            folderOpenFileDialog.InitialDirectory = MainWindow.maskOpenPath;
            if (folderOpenFileDialog.ShowDialog() == true)
            {
                MainWindow.maskSavePath = folderOpenFileDialog.FolderName + "\\CifAuto_mask.cif";
                this.savePath.Text = folderOpenFileDialog.FolderName;
            }
            this.Activate();
        }

        private void Button_Save_Changes_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.viewChamber = this.miChambView.IsChecked.Value;
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
    }
}
