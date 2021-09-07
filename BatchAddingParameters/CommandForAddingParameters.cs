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
        public static string FOPPath { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\res\\ФОП2019.txt";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application.Application;

            /*
            var doc = commandData.Application.Application.OpenDocumentFile(@"C:\Users\o.sidorin\Downloads\Шаблон АР\M1_Окно_Проем.rfa");
            var parameters = GetFamilyParametersToString(doc);
            var addedParameter = AddSharedParameterInFamily(app, doc, "05 Необязательные ОБЩИЕ", "Тестовый параметр");

            var myForm = new FormForPresentation();
            myForm.textBox1.Text = parameters;
            myForm.textBox2.Text = addedParameter;
            myForm.Show();

            doc.Close();
            */

            var formForAdding = new FormForAddingParameter();

            List<string> groupNamesInFOP = GetGroupsOfFOP(app);
            formForAdding.treeView1.BeginUpdate();
            int z = 0;
            foreach (string groupName in groupNamesInFOP)
            {
                formForAdding.treeView1.Nodes.Add(groupName);
                formForAdding.treeView1.Nodes[z].NodeFont = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
                z += 1;
            }
            
            List<List<string>> FOPitems = GetGroupItemsOfFOP(app);
            int i = 0;
            foreach (List<string> items in FOPitems)
            {
                int j = 0;
                foreach (string item in items)
                {
                    formForAdding.treeView1.Nodes[i].Nodes.Add(item);
                    formForAdding.treeView1.Nodes[i].Nodes[j].NodeFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Italic);
                    j += 1;
                }
                i += 1;
            }

            formForAdding.treeView1.EndUpdate();

            formForAdding.treeView2.BeginUpdate();
            ListDirectory(formForAdding.treeView2, @"C:\Users\o.sidorin\Downloads");
            formForAdding.treeView2.EndUpdate();

            formForAdding.Show();

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
        private string AddSharedParameterInFamily(Application app, Document doc, string groupName, string sharedParameterName)
        {
            string str = "";
            if (!doc.IsFamilyDocument) return "не семейство";
            FamilyManager familyManager = doc.FamilyManager;
            //FamilyType familyType = familyManager.CurrentType;
            FamilyParameterSet parametersList = familyManager.Parameters;

            foreach (FamilyParameter p in parametersList)
            {
                if (p.Definition.Name == sharedParameterName) return p.Definition.Name + ": параметр существует";
            }

            try
            {
                app.SharedParametersFilename = FOPPath;
                using (Transaction t = new Transaction(doc, "Add paramter"))
                {
                    t.Start();
                    DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
                    DefinitionGroup sharedParametersGroup = sharedParametersFile.Groups.get_Item(groupName);
                    Definition sharedParameterDefinition = sharedParametersGroup.Definitions.get_Item(sharedParameterName);
                    ExternalDefinition externalDefinition = sharedParameterDefinition as ExternalDefinition;
                    FamilyParameter familyParameter = familyManager.AddParameter(externalDefinition, BuiltInParameterGroup.PG_TEXT, false);
                    str = familyParameter.Definition.Name;
                    t.Commit();
                }

            }
            catch (Exception e)
            {
                TaskDialog.Show("!", e.ToString());
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
                    currentNode.Nodes.Add(new TreeNode(file.Name));
            }

            treeView.Nodes.Add(node);
        }
    }
}
