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

namespace BatchAddingParameters
{
    /// <summary>
    /// Логика взаимодействия для WindowAddParameterToList.xaml
    /// </summary>
    public partial class WindowAddParameterToList : Window
    {
        public Autodesk.Revit.ApplicationServices.Application _Application;
        public ObservableCollection<ParameterViewModel> _AllParams; // коллекция для treeview с параметрами
        ObservableCollection<GroupInFamilyViewModel> _GroupsCollection; // коллекция для combobox
        ParameterViewModel _ParameterProperties = new ParameterViewModel();
        GroupInFamilyViewModel _GroupsInFamilyViewModel = new GroupInFamilyViewModel();
        public WindowMain _WindowMain;
        public WindowAddParameterToList()
        {
            InitializeComponent();
            Loaded += WindowAddParameterToList_Loaded;
        }

        private void WindowAddParameterToList_Loaded(object sender, RoutedEventArgs e)
        {

            #region Combobox filling
            _GroupsCollection = new ObservableCollection<GroupInFamilyViewModel>();
            var listOfGroups = _GroupsInFamilyViewModel.GetGroups();
            foreach (var group in listOfGroups)
            {
                var item = new GroupInFamilyViewModel()
                {
                    GroupName = group
                };
                _GroupsCollection.Add(item);
            }
            comboBox_Groups.ItemsSource = _GroupsCollection;
            comboBox_Groups.SelectedIndex = 0;
            #endregion


            #region treeView filling by paramerters
            _AllParams = new ObservableCollection<ParameterViewModel>(_ParameterProperties.AllParameters(_Application));
            List<string> groups = _ParameterProperties.GroupsList(_Application);
            foreach (var group in groups)
            {
                var item = new TreeViewItem()
                {
                    Header = group
                };
                foreach (ParameterViewModel p in _AllParams.Where(x => x.ParameterGroup == group))
                {
                    var subitem = new TreeViewItem()
                    {
                        Header = p.Name,
                        Tag = p
                    };
                    item.Items.Add(subitem);
                }

                treeViewParameters.Items.Add(item);
            }
            #endregion

        }

        private void ButtonAddParameter_ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem selItem = treeViewParameters.SelectedItem as TreeViewItem;
                ParameterViewModel copyTag = selItem.Tag as ParameterViewModel;
                int index = 0;
                int i = 0;
                foreach (var item in _AllParams)
                {
                    if (item.Name == copyTag.Name)
                    {
                        index = i;
                    }
                    i += 1;
                }
                var parameter = _AllParams[index];
                bool isInListForAdd = false;
                foreach (var item in _WindowMain._ParamsForAdd)
                {
                    if (item.Name == copyTag.Name) isInListForAdd = true;
                }
                GroupInFamilyViewModel group = comboBox_Groups.SelectedItem as GroupInFamilyViewModel;
                parameter.FamilyParameterGroup = group.GroupName;
                parameter.FamilyValue = parameterValue.Text;
                if (checkBox.IsChecked ?? true)
                {
                    parameter.FamilyParameterType = "Экземпляр";
                } 
                else
                {
                    parameter.FamilyParameterType = "Тип";
                }
                if (!isInListForAdd)
                {
                    _WindowMain._ParamsForAdd.Add(parameter);
                }
                //MessageBox.Show(index.ToString() + " -> " + _AllParams[index].Name);
            }
            catch
            {

            }
            

        }

        private void ButtonClose_ButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ParameterIsSelectedOnTree(object sender, RoutedEventArgs e)
        {

            try
            {
                TreeViewItem selItem = treeViewParameters.SelectedItem as TreeViewItem;
                ParameterViewModel copyTag = selItem.Tag as ParameterViewModel;
                labelParameterType.Content = copyTag.ParameterType;

            }
            catch
            {
                labelParameterType.Content = "";
            }

        }

        private void MouseDoubleClickOnTreeViewItemOfParameters(object sender, RoutedEventArgs e)
        {
            ButtonAddParameter_ButtonClick(sender, e);
        }
    }
}
