using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;
using ZetaLongPaths;

namespace BatchAddingParameters
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class MainCommand : IExternalCommand
    {
        public static string DirectoryTreeStartDirectory =  @"C:\Users\" + Environment.UserName; //@"\\ukkalita.local\iptg\Строительно-девелоперский дивизион\М1 Проект\Проекты\10. Отдел информационного моделирования\01. REVIT\01. Библиотека семейств";
        public static string FOPPath { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\res\\ФОП2019.txt";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {


            #region WindowForm
            
            var app = commandData.Application.Application;

            

            app.SharedParametersFilename = FOPPath;
            ButtonAddExternalEvent.CommandData = commandData;
            ButtonDeleteExternalEvent.CommandData = commandData;
            var mainForm = new MainForm();


            List<string> groupNamesInFOP = GetGroupsOfFOP(app);
            mainForm.treeViewParameters.BeginUpdate();
            int z = 0;
            foreach (string groupName in groupNamesInFOP)
            {
                mainForm.treeViewParameters.Nodes.Add(groupName);
                mainForm.treeViewParameters.Nodes[z].NodeFont = new System.Drawing.Font("Segoe UI Light", 10);
                z += 1;
            }
            
            List<List<string>> FOPitems = GetGroupItemsOfFOP(app);
            int i = 0;
            foreach (List<string> items in FOPitems)
            {
                int j = 0;
                foreach (string item in items)
                {
                    mainForm.treeViewParameters.Nodes[i].Nodes.Add(item);
                    mainForm.treeViewParameters.Nodes[i].Nodes[j].NodeFont = new System.Drawing.Font("Segoe UI Light", 10);
                    j += 1;
                }
                i += 1;
            }

            mainForm.treeViewParameters.EndUpdate();

            mainForm.treeViewFamilies.BeginUpdate();
            ListDirectory(mainForm.treeViewFamilies, DirectoryTreeStartDirectory);
            mainForm.treeViewFamilies.EndUpdate();

            #region Fill the combobox
            mainForm.comboBoxGroup.Items.Add("Прочее");
            mainForm.comboBoxGroup.Items.Add("Размеры");
            mainForm.comboBoxGroup.Items.Add("Графика");
            mainForm.comboBoxGroup.Items.Add("Текст");
            mainForm.comboBoxGroup.Items.Add("Материалы и отделка");
            mainForm.comboBoxGroup.Items.Add("Данные");
            mainForm.comboBoxGroup.Items.Add("Идентификация");
            mainForm.comboBoxGroup.Items.Add("Общие");
            mainForm.comboBoxGroup.Items.Add("Видимость");
            mainForm.comboBoxGroup.Items.Add("Свойства модели");
            mainForm.comboBoxGroup.Items.Add("Параметры IFC");
            mainForm.comboBoxGroup.Items.Add("Зависимости");

            mainForm.comboBoxGroup.Items.Add("Строительство");
            mainForm.comboBoxGroup.Items.Add("Несущие конструкции");
            mainForm.comboBoxGroup.Items.Add("Расчет несущих конструкций");
            mainForm.comboBoxGroup.Items.Add("Набор арматурных стержней");
            mainForm.comboBoxGroup.Items.Add("Редактирование формы перекрытия");
            mainForm.comboBoxGroup.Items.Add("Слои");
            mainForm.comboBoxGroup.Items.Add("Моменты");
            mainForm.comboBoxGroup.Items.Add("Силы");

            mainForm.comboBoxGroup.Items.Add("Механизмы");
            mainForm.comboBoxGroup.Items.Add("Механизмы - Расход");
            mainForm.comboBoxGroup.Items.Add("Механизмы - Нагрузки");
            mainForm.comboBoxGroup.Items.Add("Сантехника");
            mainForm.comboBoxGroup.Items.Add("Сегменты и соединительные детали");
            mainForm.comboBoxGroup.Items.Add("Система пожаротушения");

            mainForm.comboBoxGroup.Items.Add("Электросети");
            mainForm.comboBoxGroup.Items.Add("Электросети (А)");
            mainForm.comboBoxGroup.Items.Add("Электросети - Нагрузки");
            mainForm.comboBoxGroup.Items.Add("Электросети - Освещение");
            mainForm.comboBoxGroup.Items.Add("Электросети - Создание цепей");
            mainForm.comboBoxGroup.Items.Add("Рачет энергопотребления");
            mainForm.comboBoxGroup.Items.Add("Фотометрические");

            mainForm.comboBoxGroup.Items.Add("Аналитическая модель");
            mainForm.comboBoxGroup.Items.Add("Результаты анализа");
            
            mainForm.comboBoxGroup.Items.Add("Геометрия разделения");
            mainForm.comboBoxGroup.Items.Add("Стадии");
            mainForm.comboBoxGroup.Items.Add("Общая легенда");
            mainForm.comboBoxGroup.Items.Add("Шрифт заголовков");
            mainForm.comboBoxGroup.Items.Add("Свойства экологически чистого здания");

            mainForm.comboBoxGroup.SelectedIndex = 0;

            mainForm.comboBoxStartFolder.Items.Add(@"C:\Users\" + Environment.UserName); //@"\Downloads"
            mainForm.comboBoxStartFolder.Items.Add(@"\\ukkalita.local\iptg\Строительно-девелоперский дивизион\М1 Проект\Проекты\10. Отдел информационного моделирования\01. REVIT\01. Библиотека семейств");

            mainForm.comboBoxStartFolder.SelectedIndex = 0;
            #endregion

            ButtonAddExternalEvent.MainForm = mainForm;
            ButtonDeleteExternalEvent.MainForm = mainForm;

            mainForm.Show();
            
            #endregion


            //var windowMain = new WindowMain();
            //windowMain._Application = commandData.Application.Application;
            //windowMain.Show();

            return Result.Succeeded;
        }
        private string GetFamilyParametersToString(Document doc)
        {
            string str = "";
            if (!doc.IsFamilyDocument) return "не семейство";

            FamilyManager familyManager = doc.FamilyManager;
            //FamilyType familyType = familyManager.CurrentType;
            FamilyParameterSet parametersList = familyManager.Parameters;
            //string fopFilePath = "";

            //DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            //DefinitionGroup sharedParametersGroup = sharedParametersFile.Groups.get_Item("14_Управление семействами");
            //Definition sharedParameterDefinition;
            //ExternalDefinition externalDefinition;

            parametersList = familyManager.Parameters;

            foreach (FamilyParameter p in parametersList)
            {
                str += p.Definition.Name + Environment.NewLine;
            }

            return str;
        }
        private List<string> GetGroupsOfFOP(Application app)
        {
            List<string> output = new List<string>();
            app.SharedParametersFilename = FOPPath;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach(var item in definitionGroups)
            {
                output.Add(item.Name);
            }
            return output;
        }
        private List<List<string>> GetGroupItemsOfFOP(Application app)
        {
            List<List<string>> output = new List<List<string>>();
            app.SharedParametersFilename = FOPPath;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach(DefinitionGroup definitionGroup in definitionGroups)
            {
                List<string> items = new List<string>();
                foreach (Definition definition in definitionGroup.Definitions)
                {
                    items.Add(definition.Name);
                }
                output.Add(items);
            }
            return output;
        }
        public static void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();

            var stack = new Stack<TreeNode>();
            var rootDirectory = new ZlpDirectoryInfo(path);
            var node = new TreeNode(rootDirectory.Name) { Tag = rootDirectory };
            stack.Push(node);

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();
                var directoryInfo = (ZlpDirectoryInfo)currentNode.Tag;
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    var childDirectoryNode = new TreeNode(directory.Name) { Tag = directory };
                    currentNode.Nodes.Add(childDirectoryNode);
                    stack.Push(childDirectoryNode);
                }
                foreach (var file in directoryInfo.GetFiles())
                {
                    if (file.Name.Contains(".rfa"))
                    {
                        currentNode.Nodes.Add(new TreeNode(file.Name));
                    }
                    
                }
                    
            }
            node.NodeFont = new System.Drawing.Font("Segoe UI Light", 10);
            treeView.Nodes.Add(node);

        }

    }
}
