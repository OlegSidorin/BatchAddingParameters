using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BatchAddingParameters
{
    /// <summary>
    /// Логика взаимодействия для FormWPF.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        public Autodesk.Revit.ApplicationServices.Application _Application;
        ComboBoxItem _ComboBoxItem = new ComboBoxItem();
        ParameterProperties _ParameterProperties = new ParameterProperties();
        public WindowMain()
        {
            InitializeComponent();
            Loaded += WindowMain_Loaded;
        }

        private void WindowMain_Loaded(object sender, RoutedEventArgs e)
        {
            listViewParameters.ItemsSource = _ParameterProperties.AllParameters(_Application);
            comboBoxStartFolder.ItemsSource = _ComboBoxItem.StartFolders();
            comboBoxStartFolder.SelectedIndex = 0;
        }

        private void onButtonHelpClick(object sender, RoutedEventArgs e)
        {
            var helpWindow = new WindowHelp();
            helpWindow.Show();
        }

        private void buttonChangeParametersList_Click(object sender, RoutedEventArgs e)
        {
            var windowAddParameter = new WindowAddParameterToList();
            windowAddParameter._Application = _Application;
        }
    }
    public class ComboBoxItem
    {
        public string StartFolder { get; set; }

        public ComboBoxItem(string startFolder)
        {
            StartFolder = startFolder;
        }
        public ComboBoxItem()
        {

        }
        public ComboBoxItem[] StartFolders()
        {
            return new ComboBoxItem[]
            {
                new ComboBoxItem(@"C:\Users\" + Environment.UserName),
                new ComboBoxItem(@"\\ukkalita.local\iptg\Строительно-девелоперский дивизион\М1 Проект\Проекты\10. Отдел информационного моделирования\01. REVIT\01. Библиотека семейств")
            };
        }
    }


}
