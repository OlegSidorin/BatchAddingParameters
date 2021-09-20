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
using System.Collections.ObjectModel;
using System.IO;
using Path = System.IO.Path;

namespace BatchAddingParameters
{
    /// <summary>
    /// Логика взаимодействия для FormWPF.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        public Autodesk.Revit.ApplicationServices.Application _Application;
        ComboBoxItem _ComboBoxItem = new ComboBoxItem();
        ParameterViewModel _ParameterProperties = new ParameterViewModel();
        FolderTreeNodeItem _TreeViewItems = new FolderTreeNodeItem();

        public WindowMain()
        {
            InitializeComponent();
            Loaded += WindowMain_Loaded;
        }

        private void WindowMain_Loaded(object sender, RoutedEventArgs e)
        {
            listViewParameters.ItemsSource = _ParameterProperties.AllParameters(_Application);
            /*
            comboBoxStartFolder.ItemsSource = _ComboBoxItem.StartFolders();
            comboBoxStartFolder.SelectedIndex = 0;
            */
            //folderTree.ItemsSource = _TreeViewItems.FolderTreeNodeItems();
            /*
            foreach (var drive in Directory.GetLogicalDrives())
            {
                var item = new TreeViewItem()
                {
                    Header = drive,
                    Tag = drive
                };

                
                item.Items.Add(null);
                item.Expanded += Folder_Expanded;
                FolderView.Items.Add(item);
            }
            */
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;
            item.Items.Clear();
            var fullpath = item.Tag as string;
            // add folders
            var directories = new List<string>();
            try
            {
                var dirs = Directory.GetDirectories(fullpath);
                if (dirs.Length > 0)
                    directories.AddRange(dirs);

            }
            catch
            {

            }
            directories.ForEach(directoryPath =>
            {
                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(directoryPath),
                    Tag = directoryPath
                };
                subItem.Items.Add(null);
                subItem.Expanded += Folder_Expanded;
                item.Items.Add(subItem);
            });
            // add files
            var files = new List<string>();
            try
            {
                var fs = Directory.GetFiles(fullpath);
                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch
            {

            }
            files.ForEach(filePath =>
            {
                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(filePath),
                    Tag = filePath
                };
                item.Items.Add(subItem);

            });
        }

        public static string GetFileFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            var normalizedPath = path.Replace('/', '\\');
            var lastIndex = normalizedPath.LastIndexOf('\\');
            if (lastIndex <= 0)
                return path;
            return path.Substring(lastIndex + 1);
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

        private void UserFolder_ButtonClick(object sender, RoutedEventArgs e)
        {
            FolderView.Items.Clear();
            var item = new TreeViewItem()
            {
                Header = Environment.UserName,
                Tag = Main.UserFolder
            };


            item.Items.Add(null);
            item.Expanded += Folder_Expanded;
            FolderView.Items.Add(item);

        }

        private void AllFolders_ButtonClick(object sender, RoutedEventArgs e)
        {
            FolderView.Items.Clear();
            foreach (var drive in Directory.GetLogicalDrives())
            {
                var item = new TreeViewItem()
                {
                    Header = drive,
                    Tag = drive
                };


                item.Items.Add(null);
                item.Expanded += Folder_Expanded;
                FolderView.Items.Add(item);
            }

        }

        private void NetFolder_ButtonClick(object sender, RoutedEventArgs e)
        {
            FolderView.Items.Clear();
            var item = new TreeViewItem()
            {
                Header = "01. Библиотека семейств",
                Tag = @"\\ukkalita.local\iptg\Строительно-девелоперский дивизион\М1 Проект\Проекты\10. Отдел информационного моделирования\01. REVIT\01. Библиотека семейств"
            };


            item.Items.Add(null);
            item.Expanded += Folder_Expanded;
            FolderView.Items.Add(item);
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
