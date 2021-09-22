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
using System.Data;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BatchAddingParameters
{
    /// <summary>
    /// Логика взаимодействия для FormWPF.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        public Autodesk.Revit.ApplicationServices.Application _Application;
        public Autodesk.Revit.UI.ExternalCommandData _CommandData;
        ComboBoxItem _ComboBoxItem = new ComboBoxItem();
        ParameterViewModel _ParameterProperties = new ParameterViewModel();
        FolderTreeNodeItem _TreeViewItems = new FolderTreeNodeItem();
        ObservableCollection<ParameterViewModel> _AllParams;
        public ObservableCollection<ParameterViewModel> _ParamsForAdd;
        public WindowMain()
        {
            InitializeComponent();
            Loaded += WindowMain_Loaded;
        }

        private void WindowMain_Loaded(object sender, RoutedEventArgs e)
        {

            var listAllParameters = _ParameterProperties.AllParameters(_Application);
            _AllParams = new ObservableCollection<ParameterViewModel>();
            _ParamsForAdd = new ObservableCollection<ParameterViewModel>();
            foreach (var item in listAllParameters)
            {
                _AllParams.Add(item);
            }

            listViewParameters.ItemsSource = _ParamsForAdd;


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
                var fsAll = Directory.GetFiles(fullpath);
                var fs = new List<string>();
                foreach (var path in fsAll)
                {
                    if (path.Contains(".rfa"))
                        fs.Add(path);
                }
                
                if (fs.Count > 0)
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

        private void UserFolder_ButtonClick(object sender, RoutedEventArgs e)
        {
            FolderView.Items.Clear();
            var directories = new List<string>();
            try
            {
                var dirs = Directory.GetDirectories(Main.UserFolder);
                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }
            catch
            {

            }

            foreach (var dir in directories)
            {
                var item = new TreeViewItem()
                {
                    Header = GetFileFolderName(dir),
                    Tag = dir
                };
                item.Items.Add(null);
                item.Expanded += Folder_Expanded;
                FolderView.Items.Add(item);
            }
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
            var directories = new List<string>();
            try
            {
                var dirs = Directory.GetDirectories(@"\\ukkalita.local\iptg\Строительно-девелоперский дивизион\М1 Проект\Проекты\10. Отдел информационного моделирования\01. REVIT\01. Библиотека семейств");
                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }
            catch
            {

            }

            foreach (var dir in directories)
            {
                var item = new TreeViewItem()
                {
                    Header = GetFileFolderName(dir),
                    Tag = dir
                };
                item.Items.Add(null);
                item.Expanded += Folder_Expanded;
                FolderView.Items.Add(item);
            }



        }

        private void ButtonFromListView_ButtonClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ParameterViewModel parameter = b.CommandParameter as ParameterViewModel;
            _ParamsForAdd.Remove(parameter);
        }

        private void ButtonAdd_ButtonClick(object sender, RoutedEventArgs e)
        {
            WindowAddParameterToList window = new WindowAddParameterToList();
            window._Application = _Application;
            window._WindowMain = this;
            window.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAddParametersInToFamily_Click(object sender, RoutedEventArgs e)
        {
            var treeViewSelectedItem = FolderView.SelectedItem as TreeViewItem;
            string path = treeViewSelectedItem.Tag as string;
            string str = "";
            foreach (var par in _ParamsForAdd)
            {
                str += par.Name + ", ";
            }
            MessageBox.Show(str + " : " + path);
            
        }
        private string GroupNameBySharedParameterName(ExternalCommandData commandData, string sharedParameterName)
        {
            string outputGroupName = "";

            DefinitionFile sharedParametersFile = commandData.Application.Application.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (DefinitionGroup definitionGroup in definitionGroups)
            {
                foreach (Definition definition in definitionGroup.Definitions)
                {
                    if (definition.Name == sharedParameterName)
                        outputGroupName = definitionGroup.Name;
                }
            }

            return outputGroupName;
        }
        private string AddSharedParameterIntoFamily(ExternalCommandData commandData, Document doc, string sharedParameterName, bool isInstance, BuiltInParameterGroup group)
        {
            string str = "";

            FamilyManager familyManager = doc.FamilyManager;
            FamilyType familyType = familyManager.CurrentType;
            FamilyTypeSet types = familyManager.Types;

            #region check if family has no type
            if (familyType == null)
            {
                using (Transaction t = new Transaction(doc, "change"))
                {
                    t.Start();
                    familyType = familyManager.NewType("Тип 1");
                    familyManager.CurrentType = familyType;
                    t.Commit();
                }
            }
            #endregion

            FamilyParameterSet parametersList = familyManager.Parameters;

            #region check tha parameter already in doc
            foreach (FamilyParameter p in parametersList)
            {
                if (p.Definition.Name == sharedParameterName)
                {
                    string addedToStr = "";
                    var docName = doc.Title + ".rfa";
                    try
                    {
                        addedToStr += CM.CloseDocSimple(doc);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    return ":: " + "Параметр " + sharedParameterName + " существует в семействе " + docName + addedToStr;
                }

            }
            #endregion

            #region add parameter
            try
            {
                //app.SharedParametersFilename = CommandForAddingParameters.FOPPath;
                using (Transaction t = new Transaction(doc, "Add paramter"))
                {
                    t.Start();
                    DefinitionFile sharedParametersFile = commandData.Application.Application.OpenSharedParameterFile();
                    DefinitionGroup sharedParametersGroup = sharedParametersFile.Groups.get_Item(GroupNameBySharedParameterName(commandData, sharedParameterName));
                    Definition sharedParameterDefinition = sharedParametersGroup.Definitions.get_Item(sharedParameterName);
                    ExternalDefinition externalDefinition = sharedParameterDefinition as ExternalDefinition;
                    FamilyParameter familyParameter = familyManager.AddParameter(externalDefinition, group, isInstance);
                    str = "+ " + familyParameter.Definition.Name + " был успешно добавлен в семейство " + doc.Title + ".rfa";
                    t.Commit();
                }

            }
            catch (Exception e)
            {
                str = "! " + sharedParameterName + " не удалось добавить в семейство " + doc.Title + ".rfa";
            }
            #endregion

            str += CM.SaveAndCloseDocSimple(doc);

            return str;

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
