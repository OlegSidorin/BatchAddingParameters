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
        public ButtonAddWindowExternalEvent ButtonAddWindowExternalEvent;
        public ExternalEvent ExternalEventButtonAddWindow;
        public ButtonDeleteWindowExternalEvent ButtonDeleteWindowExternalEvent;
        public ExternalEvent ExternalEventButtonDeleteWindow;

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

            ButtonAddWindowExternalEvent = new ButtonAddWindowExternalEvent();
            ExternalEventButtonAddWindow = ExternalEvent.Create(ButtonAddWindowExternalEvent);
            ButtonDeleteWindowExternalEvent = new ButtonDeleteWindowExternalEvent();
            ExternalEventButtonDeleteWindow = ExternalEvent.Create(ButtonDeleteWindowExternalEvent);

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

            subFoldersCheckBox.IsChecked = true;

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


        private void ButtonAddParametersInToFamily_Click(object sender, RoutedEventArgs e)
        {

            ButtonAddWindowExternalEvent._CommandData = _CommandData;
            ButtonAddWindowExternalEvent._WindowMain = this;

            ExternalEventButtonAddWindow.Raise();

        }


        private void ButtonDeleteParametersFromFamily_Click(object sender, RoutedEventArgs e)
        {
            ButtonDeleteWindowExternalEvent._CommandData = _CommandData;
            ButtonDeleteWindowExternalEvent._WindowMain = this;

            ExternalEventButtonDeleteWindow.Raise();
        }
    }


    public class ButtonAddWindowExternalEvent : IExternalEventHandler
    {
        public static WindowMain _WindowMain;
        public static ExternalCommandData _CommandData;
        
        public void Execute(UIApplication uiApp)
        {

            TreeViewItem treeViewSelectedItem = (TreeViewItem)_WindowMain.FolderView.SelectedItem;
            bool withSubfolders = false;
            if (_WindowMain.subFoldersCheckBox.IsChecked ?? true) withSubfolders = true;
            List<string> PathToFamilyList = CM.CreatePathArray((string)treeViewSelectedItem.Tag, withSubfolders);

            
            
            DateTime a = DateTime.Now;


            //ParameterViewModel parameterForAdd = new ParameterViewModel();
            //parameterForAdd = _WindowMain._ParamsForAdd.First();


            
            var paramNames = new List<string>();
            foreach(var item in _WindowMain._ParamsForAdd)
            {
                paramNames.Add(item.Name);
            };
            _WindowMain.textBlockOutput.AppendText($"-> ДОБАВЛЕНИЕ ПАРАМЕТРОВ: {string.Join(", ",  paramNames.ToArray())}" + Environment.NewLine);

            int i_sucses = 0;
            int i_all = 0;

            foreach (string PathToFamily in PathToFamilyList)
            {
                i_all += 1;
                try
                {
                    if (!PathToFamily.Contains(".00"))
                    {
                        var doc = _CommandData.Application.Application.OpenDocumentFile(PathToFamily);
                        
                        if (!doc.IsReadOnly || !doc.IsReadOnlyFile || doc.IsModifiable)
                        {
                            var resultText = "";
                            foreach (var parameterForAdd in _WindowMain._ParamsForAdd)
                            {
                                resultText = parameterForAdd.AddSharedParameterIntoFamily(_CommandData, doc);

                                _WindowMain.textBlockOutput.AppendText(resultText + Environment.NewLine);
                                _WindowMain.textBlockOutput.CaretIndex = _WindowMain.textBlockOutput.Text.Length;
                                _WindowMain.textBlockOutput.ScrollToEnd();
                            }

                            resultText += CM.SaveAndCloseDocSimple(doc);
                            i_sucses += 1;
                        }
                        else
                        {
                            _WindowMain.textBlockOutput.AppendText(":: " + CM.CloseDoc(doc) + " проигнорировван " + Environment.NewLine);
                        }
                    }
                    else
                    {
                        _WindowMain.textBlockOutput.AppendText(":: ..." + PathToFamily.Substring(PathToFamily.Length - 20) + " проигнорировван " + Environment.NewLine);
                    }

                }

                #region catch block
                catch (Autodesk.Revit.Exceptions.ArgumentNullException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (ArgumentNull): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (Argument): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.CannotOpenBothCentralAndLocalException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (CannotOpenBothCentralAndLocal): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.CentralModelException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (CentralModel): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.CorruptModelException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (CorruptModel): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.FileAccessException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (FileAccess): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.FileNotFoundException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (FileNotFound): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.InsufficientResourcesException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (InsufficientResources): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.InvalidOperationException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (InvalidOperation): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (OperationCanceled): " + openErr.Message + Environment.NewLine);
                }
                catch (NullReferenceException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (Системная, ссылка не ведет к файлу): " + openErr.ToString() + Environment.NewLine);
                }
                catch (Exception openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (Системная): " + openErr.ToString() + Environment.NewLine);
                }
                #endregion
            }
            DateTime b = DateTime.Now;

            _WindowMain.textBlockOutput.AppendText("заняло не более " + ((int)b.Subtract(a).TotalMinutes + 1).ToString() + " мин" + Environment.NewLine);
            _WindowMain.textBlockOutput.AppendText($"всего семейств: {i_all}, обработано без ошибок: {i_sucses}" + Environment.NewLine);
            _WindowMain.textBlockOutput.CaretIndex = _WindowMain.textBlockOutput.Text.Length;
            _WindowMain.textBlockOutput.ScrollToEnd();


            if (PathToFamilyList.Count == 0)
                _WindowMain.textBlockOutput.AppendText("!!! отсутствует путь до семейства \n");

            return;
        }

        public string GetName()
        {
            return "External Event";
        }
        //private void AddPathsAndSubpathsToPathToFamilyList(string targetDirectory)
        //{
        //    var folderPath = new DirectoryInfo(targetDirectory);
        //    foreach (var filePath in folderPath.GetFiles())
        //    {
        //        if (filePath.ToString().Contains(".rfa"))
        //        {
        //            PathToFamilyList.Add(filePath.FullName.ToString().Replace(@"\\?\UNC\", @"\\"));
        //        }
        //    }

        //    var subfolderPaths = folderPath.GetDirectories();
        //    foreach (var subfolderPath in subfolderPaths)
        //    {
        //        AddPathsAndSubpathsToPathToFamilyList(subfolderPath.FullName.ToString().Replace(@"\\?\UNC\", @"\\"));
        //    }


        //}
        //private void AddPathsToPathToFamilyList(string targetDirectory)
        //{
        //    var folderPath = new DirectoryInfo(targetDirectory);
        //    //string[] fileEntries = Directory.GetFiles(targetDirectory);
        //    foreach (var filePath in folderPath.GetFiles())
        //    {
        //        if (filePath.ToString().Contains(".rfa"))
        //        {
        //            PathToFamilyList.Add(filePath.FullName.ToString());
        //        }
        //    }


        //}
    }

    public class ButtonDeleteWindowExternalEvent : IExternalEventHandler
    {
        public static WindowMain _WindowMain;
        public static ExternalCommandData _CommandData;
        public void Execute(UIApplication app)
        {
            TreeViewItem treeViewSelectedItem = (TreeViewItem)_WindowMain.FolderView.SelectedItem;
            bool withSubfolders = false;
            if (_WindowMain.subFoldersCheckBox.IsChecked ?? true) withSubfolders = true;
            List<string> PathToFamilyList = CM.CreatePathArray((string)treeViewSelectedItem.Tag, withSubfolders);

            DateTime a = DateTime.Now;

            var paramNames = new List<string>();
            foreach (var item in _WindowMain._ParamsForAdd)
            {
                paramNames.Add(item.Name);
            };
            _WindowMain.textBlockOutput.AppendText($"<- УДАЛЕНИЕ ПАРАМЕТРОВ: {string.Join(", ", paramNames.ToArray())}" + Environment.NewLine);


            int i_sucses = 0;
            int i_all = 0;

            foreach (string PathToFamily in PathToFamilyList)
            {
                i_all += 1;
                try
                {
                    if (!PathToFamily.Contains(".00"))
                    {
                        var doc = _CommandData.Application.Application.OpenDocumentFile(PathToFamily);
                        if (!doc.IsReadOnly || !doc.IsReadOnlyFile || doc.IsModifiable)
                        {
                            var resultText = "";
                            foreach (var parameterForDelete in _WindowMain._ParamsForAdd)
                            {
                                resultText = parameterForDelete.DeleteSharedParameterFromFamily(_CommandData, doc);
                                _WindowMain.textBlockOutput.AppendText(resultText + Environment.NewLine);
                                _WindowMain.textBlockOutput.CaretIndex = _WindowMain.textBlockOutput.Text.Length;
                                _WindowMain.textBlockOutput.ScrollToEnd();
                            }
                            resultText += CM.SaveAndCloseDocSimple(doc);
                            i_sucses += 1;
                        }
                        else
                        {
                            _WindowMain.textBlockOutput.AppendText(":: " + CM.CloseDoc(doc) + " проигнорировван " + Environment.NewLine);
                            _WindowMain.textBlockOutput.CaretIndex = _WindowMain.textBlockOutput.Text.Length;
                            _WindowMain.textBlockOutput.ScrollToEnd();
                        }
                    }
                    else
                    {
                        _WindowMain.textBlockOutput.AppendText(":: ..." + PathToFamily.Substring(PathToFamily.Length - 20) + " проигнорировван " + Environment.NewLine);
                        _WindowMain.textBlockOutput.CaretIndex = _WindowMain.textBlockOutput.Text.Length;
                        _WindowMain.textBlockOutput.ScrollToEnd();
                    }

                }

                #region catch block
                catch (Autodesk.Revit.Exceptions.ArgumentNullException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (ArgumentNull): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (Argument): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.CannotOpenBothCentralAndLocalException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (CannotOpenBothCentralAndLocal): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.CentralModelException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (CentralModel): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.CorruptModelException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (CorruptModel): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.FileAccessException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (FileAccess): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.FileNotFoundException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (FileNotFound): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.InsufficientResourcesException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (InsufficientResources): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.InvalidOperationException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (InvalidOperation): " + openErr.Message + Environment.NewLine);
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (OperationCanceled): " + openErr.Message + Environment.NewLine);
                }
                catch (NullReferenceException openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (Системная, ссылка не ведет к файлу): " + openErr.ToString() + Environment.NewLine);
                }
                catch (Exception openErr)
                {
                    _WindowMain.textBlockOutput.AppendText($"! ошибка открытия (Системная): " + openErr.ToString() + Environment.NewLine);
                }
                #endregion
            }
            DateTime b = DateTime.Now;

            _WindowMain.textBlockOutput.AppendText("заняло не более " + ((int)b.Subtract(a).TotalMinutes + 1).ToString() + " мин" + Environment.NewLine);
            _WindowMain.textBlockOutput.AppendText($"всего семейств: {i_all}, обработано без ошибок: {i_sucses}" + Environment.NewLine);
            _WindowMain.textBlockOutput.CaretIndex = _WindowMain.textBlockOutput.Text.Length;
            _WindowMain.textBlockOutput.ScrollToEnd();


            if (PathToFamilyList.Count == 0)
                _WindowMain.textBlockOutput.AppendText("!!! отсутствует путь до семейства \n");


            return;
        }

        public string GetName()
        {
            return "External Delete Event";
        }
    }
}
