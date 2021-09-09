using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace BatchAddingParameters
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class CommandForAddingParameters : IExternalCommand
    {
        public static string DirectoryTreeStartDirectory = @"C:\Users\o.sidorin\Downloads";
        public static string FOPPath { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\res\\ФОП2019.txt";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            #region WindowForm
            var app = commandData.Application.Application;

            app.SharedParametersFilename = FOPPath;
            ButtonExternalEvent.CommandData = commandData;
            ButtonDeleteExternalEvent.CommandData = commandData;
            var formForAddingParameters = new FormForAddingParameter();


            List<string> groupNamesInFOP = GetGroupsOfFOP(app);
            formForAddingParameters.treeViewParameters.BeginUpdate();
            int z = 0;
            foreach (string groupName in groupNamesInFOP)
            {
                formForAddingParameters.treeViewParameters.Nodes.Add(groupName);
                formForAddingParameters.treeViewParameters.Nodes[z].NodeFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                z += 1;
            }
            
            List<List<string>> FOPitems = GetGroupItemsOfFOP(app);
            int i = 0;
            foreach (List<string> items in FOPitems)
            {
                int j = 0;
                foreach (string item in items)
                {
                    formForAddingParameters.treeViewParameters.Nodes[i].Nodes.Add(item);
                    formForAddingParameters.treeViewParameters.Nodes[i].Nodes[j].NodeFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Regular);
                    j += 1;
                }
                i += 1;
            }

            formForAddingParameters.treeViewParameters.EndUpdate();

            formForAddingParameters.treeViewFamilies.BeginUpdate();
            ListDirectory(formForAddingParameters.treeViewFamilies, DirectoryTreeStartDirectory);
            formForAddingParameters.treeViewFamilies.EndUpdate();

            #region Fill the combobox
            formForAddingParameters.comboBoxGroup.Items.Add("Моменты");
            formForAddingParameters.comboBoxGroup.Items.Add("Силы");
            formForAddingParameters.comboBoxGroup.Items.Add("Геометрия разделения");
            formForAddingParameters.comboBoxGroup.Items.Add("Сегменты и соединительные детали");
            formForAddingParameters.comboBoxGroup.Items.Add("Общая легенда");
            formForAddingParameters.comboBoxGroup.Items.Add("Видимость");
            formForAddingParameters.comboBoxGroup.Items.Add("Данные");
            formForAddingParameters.comboBoxGroup.Items.Add("Электросети - Создание цепей");
            formForAddingParameters.comboBoxGroup.Items.Add("Общие");
            formForAddingParameters.comboBoxGroup.Items.Add("Свойства модели");
            formForAddingParameters.comboBoxGroup.Items.Add("Результаты анализа");
            formForAddingParameters.comboBoxGroup.Items.Add("Редактирование формы перекрытия");
            formForAddingParameters.comboBoxGroup.Items.Add("Фотометрические");
            formForAddingParameters.comboBoxGroup.Items.Add("Свойства экологически чистого здания");
            formForAddingParameters.comboBoxGroup.Items.Add("Шрифт заголовков");
            formForAddingParameters.comboBoxGroup.Items.Add("Система пожаротушения");
            formForAddingParameters.comboBoxGroup.Items.Add("Аналитическая модель");
            formForAddingParameters.comboBoxGroup.Items.Add("Набор арматурных стержней");
            formForAddingParameters.comboBoxGroup.Items.Add("Слои");
            formForAddingParameters.comboBoxGroup.Items.Add("Параметры IFC");
            formForAddingParameters.comboBoxGroup.Items.Add("Электросети (А)");
            formForAddingParameters.comboBoxGroup.Items.Add("Рачет энергопотребления");
            formForAddingParameters.comboBoxGroup.Items.Add("Расчет несущих конструкций");
            formForAddingParameters.comboBoxGroup.Items.Add("Механизмы - Расход");
            formForAddingParameters.comboBoxGroup.Items.Add("Механизмы - Нагрузки");
            formForAddingParameters.comboBoxGroup.Items.Add("Электросети - Нагрузки");
            formForAddingParameters.comboBoxGroup.Items.Add("Электросети - Освещение");
            formForAddingParameters.comboBoxGroup.Items.Add("Текст");
            formForAddingParameters.comboBoxGroup.Items.Add("Зависимости");
            formForAddingParameters.comboBoxGroup.Items.Add("Стадии");
            formForAddingParameters.comboBoxGroup.Items.Add("Механизмы");
            formForAddingParameters.comboBoxGroup.Items.Add("Несущие конструкции");
            formForAddingParameters.comboBoxGroup.Items.Add("Сантехника");
            formForAddingParameters.comboBoxGroup.Items.Add("Электросети");
            formForAddingParameters.comboBoxGroup.Items.Add("Материалы и отделка");
            formForAddingParameters.comboBoxGroup.Items.Add("Графика");
            formForAddingParameters.comboBoxGroup.Items.Add("Строительство");
            formForAddingParameters.comboBoxGroup.Items.Add("Размеры");
            formForAddingParameters.comboBoxGroup.Items.Add("Идентификация");
            formForAddingParameters.comboBoxGroup.Items.Add("Прочее");
            #endregion

            ButtonExternalEvent.FormForAddingParameter = formForAddingParameters;
            ButtonDeleteExternalEvent.FormForAddingParameter = formForAddingParameters;

            formForAddingParameters.Show();
            #endregion

            /*
            FormWPF formWPF = new FormWPF();
            formWPF.Show();
            */
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
        private static void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();

            var stack = new Stack<TreeNode>();
            var rootDirectory = new DirectoryInfo(path);
            var node = new TreeNode(rootDirectory.Name) { Tag = rootDirectory };
            stack.Push(node);

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();
                var directoryInfo = (DirectoryInfo)currentNode.Tag;
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    var childDirectoryNode = new TreeNode(directory.Name) { Tag = directory };
                    currentNode.Nodes.Add(childDirectoryNode);
                    stack.Push(childDirectoryNode);
                }
                foreach (var file in directoryInfo.GetFiles())
                {
                    currentNode.Nodes.Add(new TreeNode(file.Name));
                }
                    
            }
            node.NodeFont = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Regular);
            treeView.Nodes.Add(node);

        }
    }
}
