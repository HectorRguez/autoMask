using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace autoMask
{
    public partial class MaskWindow : Window
    {
        private List<List<TextBox>> tbs = new List<List<TextBox>>();
        private bool isLoading;

        public static List<Chip> Chips
        { get; set; }

        public MaskWindow()
        {
            this.InitializeComponent();
            MaskWindow.Chips = new List<Chip>();
            this.Load_Chips();
        }

        public void Load_Chips()
        {
            MaskWindow.Chips.Clear();
            List<string> stringList = new List<string>();
            using (StreamReader streamReader = new StreamReader(MainWindow.configMaskPath))
            {
                string str;
                while ((str = ((TextReader)streamReader).ReadLine()) != null)
                    stringList.Add(str);
            }
            for (int index = 0; index < stringList.Count; ++index)
                MaskWindow.Chips.Add(new Chip(stringList[index]));
            this.isLoading = true;
            for (int i = 0; i < MaskWindow.Chips.Count; ++i)
            {
                this.Generate_Tab_Item();
                this.UpdateTextBoxes(i);
            }
            this.isLoading = false;
        }

        private void Generate_File_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateChips();
            MainWindow window = Window.GetWindow((DependencyObject)Application.Current.MainWindow) as MainWindow;
            window.Generate_Chips();
            window.WireReport();
            window.Draw();
            this.Close();
        }

        private void Save_Config_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateChips();
            this.SaveChips();
        }

        private void Generate_Tab_Item()
        {
            int num1 = 0;
            int num2 = 0;
            while (num2 < this.chipReports.Items.Count)
            {
                bool flag = false;
                for (int index = 0; index < this.chipReports.Items.Count; ++index)
                {
                    if (int.Parse(((this.chipReports.Items[index] as TabItem).Header as string).Split(' ', StringSplitOptions.None)[1]) == num2)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    num1 = num2;
                    break;
                }
                ++num2;
                ++num1;
            }
            string[] strArray = new string[9]
            {
                "ELECTRODE N",
                "ELECTRODE DISTANCE",
                "ELECTRODE DIAMETER",
                "PAD_L",
                "WIRE MIN WIDTH",
                "SQUARE WIDTH",
                "SQUARE L",
                "CHIP MARGIN",
                "REF SEPARATION"
            };
            TabItem newItem = new TabItem();
            newItem.Header = (object)("Chip " + num1.ToString());
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            List<TextBox> textBoxList = new List<TextBox>();
            for (int index = 0; index < strArray.Length; ++index)
            {
                DockPanel element1 = new DockPanel();
                Label element2 = new Label();
                element2.Content = (object)strArray[index];
                element2.VerticalAlignment = VerticalAlignment.Center;
                TextBox element3 = new TextBox();
                element3.HorizontalAlignment = HorizontalAlignment.Right;
                element3.VerticalAlignment = VerticalAlignment.Center;
                element3.Width = 100.0;
                element3.Margin = new Thickness(0.0, 0.0, 10.0, 0.0);
                element3.TextChanged += new TextChangedEventHandler(this.TextBoxEventHandler_TextChanged);
                DockPanel.SetDock((UIElement)element3, Dock.Right);
                element1.Children.Add((UIElement)element2);
                element1.Children.Add((UIElement)element3);
                stackPanel.Children.Add((UIElement)element1);
                textBoxList.Add(element3);
            }
            this.tbs.Add(textBoxList);
            newItem.Content = (object)stackPanel;
            this.chipReports.Items.Add((object)newItem);
        }

        private void Add_Chip_Click(object sender, RoutedEventArgs e)
        {
            this.Generate_Tab_Item();
            MaskWindow.Chips.Add(new Chip());
            this.UpdateTextBoxes(MaskWindow.Chips.Count - 1);
            this.saveMaskBtn.IsEnabled = true;
            this.chipReports.SelectedIndex = this.chipReports.Items.Count - 1;
        }

        private void Remove_Chip_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = this.chipReports.SelectedIndex;
            this.chipReports.Items.RemoveAt(selectedIndex);
            MaskWindow.Chips.RemoveAt(selectedIndex);
            this.tbs.RemoveAt(selectedIndex);
            this.saveMaskBtn.IsEnabled = true;
        }

        private void UpdateTextBoxes(int i)
        {
            string[] strArray = new string[10]
            {
                MaskWindow.Chips[i].ELECTRODE_N.ToString(),
                this.FormatData(MaskWindow.Chips[i].ELECTRODE_DISTANCE),
                this.FormatData(MaskWindow.Chips[i].ELECTRODE_DIAMETER),
                this.FormatData(MaskWindow.Chips[i].PAD_L),
                this.FormatData(MaskWindow.Chips[i].WIRE_MIN_WIDTH),
                this.FormatData(MaskWindow.Chips[i].SQUARE_WIDTH),
                this.FormatData(MaskWindow.Chips[i].SQUARE_L),
                this.FormatData(MaskWindow.Chips[i].CHIP_MARGIN),
                this.FormatData(MaskWindow.Chips[i].REF_SEPARATION),
                this.FormatData(MaskWindow.Chips[i].REF_H)
            };
            int index = 0;
            foreach (TextBox textBox in this.tbs[i])
            {
                textBox.Text = strArray[index];
                ++index;
            }
        }

        private void UpdateChips()
        {
            int index = 0;
            foreach (Chip chip in MaskWindow.Chips)
            {
                MaskWindow.Chips[index].ELECTRODE_N        = int.Parse(this.tbs[index].ElementAt<TextBox>(0).Text);
                MaskWindow.Chips[index].ELECTRODE_DISTANCE = this.ParseData(this.tbs[index].ElementAt<TextBox>(1).Text);
                MaskWindow.Chips[index].ELECTRODE_DIAMETER = this.ParseData(this.tbs[index].ElementAt<TextBox>(2).Text);
                MaskWindow.Chips[index].PAD_L              = this.ParseData(this.tbs[index].ElementAt<TextBox>(3).Text);
                MaskWindow.Chips[index].WIRE_MIN_WIDTH     = this.ParseData(this.tbs[index].ElementAt<TextBox>(4).Text);
                MaskWindow.Chips[index].SQUARE_WIDTH       = this.ParseData(this.tbs[index].ElementAt<TextBox>(5).Text);
                MaskWindow.Chips[index].SQUARE_L           = this.ParseData(this.tbs[index].ElementAt<TextBox>(6).Text);
                MaskWindow.Chips[index].CHIP_MARGIN        = this.ParseData(this.tbs[index].ElementAt<TextBox>(7).Text);
                MaskWindow.Chips[index].REF_SEPARATION     = this.ParseData(this.tbs[index].ElementAt<TextBox>(8).Text);
                ++index;
            }
        }

        private void SaveChips()
        {
            string str = "";
            foreach (Chip chip in MaskWindow.Chips)
                str += chip.csvLine();
            using (StreamWriter streamWriter = new StreamWriter(MainWindow.configMaskPath, false))
                ((TextWriter)streamWriter).Write(str);
            this.saveMaskBtn.IsEnabled = false;
        }

        private void TextBoxEventHandler_TextChanged(object sender, EventArgs e)
        {
            if (this.isLoading)
                return;
            this.saveMaskBtn.IsEnabled = true;
        }

        private int ParseData(string data)
        {
            string pattern = "^(\\d+[.,]?\\d*)\\s*(mm|um|nm)\\s*$";
            Match match = Regex.Match(data, pattern);
            if (!match.Success)
                throw new Exception("Incorrect data format, please introduce a decimal number " +
                    "followed the corresponding units, which can be either 'mm', 'um' or 'nm'");
            string s = match.Groups[1].Value.Replace('.', ',');
            string str = match.Groups[2].Value;
            double num = double.Parse(s);
            int data1 = 0;
            switch (str)
            {
                case "mm":
                    data1 = (int)(num * 1000000.0);
                    break;
                case "um":
                    data1 = (int)(num * 1000.0);
                    break;
                case "nm":
                    data1 = (int)num;
                    break;
            }
            return data1;
        }

        private string FormatData(int value)
        {
            if (value == 0)
                return "0";
            double num1 = (double)value;
            string str = "";
            int num2 = 0;
            for (int index = value; index % 10 == 0; index /= 10)
                ++num2;
            if (num2 < 3)
                str = num1.ToString() + "nm";
            else if (3 <= num2 && num2 < 6)
                str = (num1 / 1000.0).ToString() + "um";
            else if (num2 >= 6)
                str = (num1 / 1000000.0).ToString() + "mm";
            return str;
        }
    }
}
