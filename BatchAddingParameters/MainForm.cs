using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using System.Windows;

namespace BatchAddingParameters
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public ButtonAddExternalEvent ButtonAddExternalEvent;
        public ExternalEvent ExternalEventButtonAdd;
        public ButtonDeleteExternalEvent ButtonDeleteExternalEvent;
        public ExternalEvent ExternalEventButtonDelete;
        public static List<string> PathToFamilyList;
        public static string Parameter;
        public static string OutputTextFromFoldersTree;
        public MainForm()
        {
            InitializeComponent();
             
            treeViewParameters.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeViewParameters.DrawNode += new DrawTreeNodeEventHandler(TreeViewParameters_DrawNode);
            treeViewFamilies.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeViewFamilies.DrawNode += new DrawTreeNodeEventHandler(TreeViewFolders_DrawNode);

            ButtonAddExternalEvent = new ButtonAddExternalEvent();
            ExternalEventButtonAdd = ExternalEvent.Create(ButtonAddExternalEvent);
            ButtonDeleteExternalEvent = new ButtonDeleteExternalEvent();
            ExternalEventButtonDelete = ExternalEvent.Create(ButtonDeleteExternalEvent);

            PathToFamilyList = new List<string>();
        }
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (PathToFamilyList.Count != 0)
            {
                ButtonAddExternalEvent.PathToFamilyList = PathToFamilyList;
                ButtonAddExternalEvent.SharedParameter = Parameter;
                ButtonAddExternalEvent.IsInstance = checkBoxInstance.Checked;

                if ((labelFamily.Text != "") && (labelParameter.Text != ""))
                {
                    ExternalEventButtonAdd.Raise();
                }
                else
                {
                    textBoxResult.AppendText("не хватает данных" + Environment.NewLine);
                }
            }

            //TaskDialog.Show("1", buttonFamily.Text + " <-|-> "+ buttonParameter.Text);
            //Close();
        }
        private void ButtonDelete_Click(object sender, EventArgs e)
        {

            if (PathToFamilyList.Count != 0)
            {
                ButtonDeleteExternalEvent.PathToFamilyList = PathToFamilyList;
                ButtonDeleteExternalEvent.SharedParameter = Parameter;

                if ((labelFamily.Text != "") && (labelParameter.Text != ""))
                {
                    ExternalEventButtonDelete.Raise();
                }
                else
                {
                    textBoxResult.AppendText("не хватает данных" + Environment.NewLine);
                }
            }

            //string str = "";
            //foreach(var path in PathToFamilyList)
            //{
            //    str += path + "\n";
            //}
            //System.Windows.Forms.MessageBox.Show(str);
        }
        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            WindowHelp windowHelp = new WindowHelp();
            windowHelp.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            windowHelp.Topmost = true;
            windowHelp.Show();
        }
        private void TreeViewFolders_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            try
            {
                if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected)
                {
                    Graphics g = e.Graphics;
                    // get node font and node fore color
                    Font nodeFont = GetTreeNodeFont(e.Node);
                    Color nodeForeColor = GetTreeNodeForeColor(e.Node, e.State);

                    // fill node background
                    using (SolidBrush brush = new SolidBrush(Color.DarkSlateBlue))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }

                    // draw node text
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, nodeFont, e.Bounds, nodeForeColor, TextFormatFlags.Left | TextFormatFlags.Top);

                    using (Pen pen = new Pen(nodeForeColor))
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        Rectangle penBounds = e.Bounds;
                        penBounds.Width -= 1;
                        penBounds.Height -= 1;
                        e.Graphics.DrawRectangle(pen, penBounds);
                    }
                    string selectedNoteText = e.Node.Text;

                    bool selectedNoteTextIsFolder = false;
                    bool selectedNoteTextIsFamily = false;
                    if (selectedNoteText.Contains(".rfa")) selectedNoteTextIsFamily = true;
                    if (!selectedNoteText.Contains(".")) selectedNoteTextIsFolder = true;
                    if (selectedNoteTextIsFolder)
                    {
                        PathToFamilyList.Clear();
                        bool isNodeHasParent = false;
                        StringBuilder path = new StringBuilder();
                        var node = e.Node;
                        path.Append(node.Text);
                        TreeNode nodePrevios = null;
                        do
                        {
                            nodePrevios = node.Parent;
                            if (nodePrevios != null)
                            {
                                isNodeHasParent = true;
                                path.Insert(0, nodePrevios.Text + @"\");
                            }
                            else
                            {
                                isNodeHasParent = false;
                            }
                            node = nodePrevios;
                        }
                        while (isNodeHasParent);
                        path.Replace(treeViewFamilies.Nodes[0].Text, "");
                        string output = MainCommand.DirectoryTreeStartDirectory + path.ToString();
                        OutputTextFromFoldersTree = output;
                        labelFamily.Text = NormalizeLength(output, 54);
                        if (checkBoxSubfolders.Checked)
                        {
                            PathToFamilyList.Clear();
                            AddPathsAndSubpathsToPathToFamilyList(output);
                        }

                        if (!checkBoxSubfolders.Checked)
                        {
                            PathToFamilyList.Clear();
                            AddPathsToPathToFamilyList(output);
                        }
                    }
                    if (selectedNoteTextIsFamily)
                    {
                        PathToFamilyList.Clear();
                        bool isNodeHasParent = false;
                        StringBuilder path = new StringBuilder();
                        var node = e.Node;
                        path.Append(node.Text);
                        TreeNode nodePrevios = null;
                        do
                        {
                            nodePrevios = node.Parent;
                            if (nodePrevios != null)
                            {
                                isNodeHasParent = true;
                                path.Insert(0, nodePrevios.Text + @"\");
                            }
                            else
                            {
                                isNodeHasParent = false;
                            }
                            node = nodePrevios;
                        }
                        while (isNodeHasParent);
                        path.Replace(treeViewFamilies.Nodes[0].Text, "");
                        string output = MainCommand.DirectoryTreeStartDirectory + path.ToString();
                        OutputTextFromFoldersTree = output;
                        labelFamily.Text = NormalizeLength(output, 54);
                        PathToFamilyList.Add(output);
                    }
                }
                else 
                {
                    e.DrawDefault = true;
                }
            }
            catch (Exception e1)
            {
                //System.Windows.MessageBox.Show(e1.ToString());
            }
            
        }
        private void TreeViewParameters_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected)
            {
                Graphics g = e.Graphics;
                // get node font and node fore color
                Font nodeFont = GetTreeNodeFont(e.Node);
                Color nodeForeColor = GetTreeNodeForeColor(e.Node, e.State);

                // fill node background
                using (SolidBrush brush = new SolidBrush(Color.DarkSlateBlue))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                // draw node text
                TextRenderer.DrawText(e.Graphics, e.Node.Text, nodeFont, e.Bounds, nodeForeColor, TextFormatFlags.Left | TextFormatFlags.Top);

                using (Pen pen = new Pen(nodeForeColor))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle penBounds = e.Bounds;
                    penBounds.Width -= 1;
                    penBounds.Height -= 1;
                    e.Graphics.DrawRectangle(pen, penBounds);
                }
                string nodeText = e.Node.Text;
                string[] groupNames = { 
                    "01 Обязательные ОБЩИЕ",
                    "02 Обязательные АРХИТЕКТУРА",
                    "03 Обязательные КОНСТРУКЦИИ",
                    "04 Обязательные ИНЖЕНЕРИЯ",
                    "05 Необязательные ОБЩИЕ",
                    "06 Необязательные АРХИТЕКТУРА",
                    "07 Необязательные КОНСТРУКЦИИ",
                    "08 Необязательные ИНЖЕНЕРИЯ",
                    "09 Заполнение штампа",
                    "10 Размеры",
                    "11 Обязательные ТХ",
                    "12 М1 Архитектура",
                    "13 М1 Технология",
                    "14 М1 Общие",
                    "15 М1 Медгазы",
                    "16 М1 Техгазы",
                    "BIM Electrical Design",
                    "17 М1 Сети связи",
                    "М1_Заполнение штампа",
                    "Экспортированные параметры"
                };
                bool nodeTextIsInGroupNames = Array.Exists(groupNames, text => text == nodeText);
                if (!nodeTextIsInGroupNames)
                {
                    Parameter = e.Node.Text;
                    labelParameter.Text = Parameter;
                }
                    
            }
            else
            {
                e.DrawDefault = true;
            }

        }
        string NormalizeLength(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : "..." + value.Substring(value.Length - maxLength, maxLength);
        }
        private Font GetTreeNodeFont(TreeNode node)
        {
            Font font = new System.Drawing.Font("Segoe UI Light", 10);
            Font nodeFont = node.NodeFont;
            if (nodeFont == null)
            {
                //nodeFont = this.Font;
                nodeFont = font;
            }
            return nodeFont;
        }
        private System.Drawing.Color GetTreeNodeForeColor(TreeNode node, TreeNodeStates nodeState)
        {
            Color nodeForeColor = Color.Empty;

            if ((nodeState & TreeNodeStates.Selected) == TreeNodeStates.Selected)
            {
                nodeForeColor = Color.FromKnownColor(KnownColor.LightYellow);
            }
            else
            {
                nodeForeColor = node.ForeColor;
                if (nodeForeColor == Color.Empty)
                {
                    nodeForeColor = this.ForeColor;
                }
            }

            return nodeForeColor;
        }
        private void ComboBoxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            ButtonAddExternalEvent.PGGroup = GetPGGroup(comboBoxGroup.Text);   
        }
        public BuiltInParameterGroup GetPGGroup(string insert)
        {
            if (insert == "Моменты") return BuiltInParameterGroup.PG_MOMENTS;
            if (insert == "Силы") return BuiltInParameterGroup.PG_FORCES;
            if (insert == "Геометрия разделения") return BuiltInParameterGroup.PG_DIVISION_GEOMETRY;
            if (insert == "Сегменты и соединительные детали") return BuiltInParameterGroup.PG_SEGMENTS_FITTINGS;
            if (insert == "Общая легенда") return BuiltInParameterGroup.PG_OVERALL_LEGEND;
            if (insert == "Видимость") return BuiltInParameterGroup.PG_VISIBILITY;
            if (insert == "Данные") return BuiltInParameterGroup.PG_DATA;
            if (insert == "Электросети - Создание цепей") return BuiltInParameterGroup.PG_ELECTRICAL_CIRCUITING;
            if (insert == "Общие") return BuiltInParameterGroup.PG_GENERAL;
            if (insert == "Свойства модели") return BuiltInParameterGroup.PG_ADSK_MODEL_PROPERTIES;
            if (insert == "Результаты анализа") return BuiltInParameterGroup.PG_ANALYSIS_RESULTS;
            if (insert == "Редактирование формы перекрытия") return BuiltInParameterGroup.PG_SLAB_SHAPE_EDIT;
            if (insert == "Фотометрические") return BuiltInParameterGroup.PG_LIGHT_PHOTOMETRICS;
            if (insert == "Свойства экологически чистого здания") return BuiltInParameterGroup.PG_GREEN_BUILDING;
            if (insert == "Шрифт заголовков") return BuiltInParameterGroup.PG_TITLE;
            if (insert == "Система пожаротушения") return BuiltInParameterGroup.PG_FIRE_PROTECTION;
            if (insert == "Аналитическая модель") return BuiltInParameterGroup.PG_ANALYTICAL_MODEL;
            if (insert == "Набор арматурных стержней") return BuiltInParameterGroup.PG_REBAR_ARRAY;
            if (insert == "Слои") return BuiltInParameterGroup.PG_REBAR_SYSTEM_LAYERS;
            if (insert == "Параметры IFC") return BuiltInParameterGroup.PG_IFC;
            if (insert == "Электросети (А)") return BuiltInParameterGroup.PG_AELECTRICAL;
            if (insert == "Рачет энергопотребления") return BuiltInParameterGroup.PG_ENERGY_ANALYSIS;
            if (insert == "Расчет несущих конструкций") return BuiltInParameterGroup.PG_STRUCTURAL_ANALYSIS;
            if (insert == "Механизмы - Расход") return BuiltInParameterGroup.PG_MECHANICAL_AIRFLOW;
            if (insert == "Механизмы - Нагрузки") return BuiltInParameterGroup.PG_MECHANICAL_LOADS;
            if (insert == "Электросети - Нагрузки") return BuiltInParameterGroup.PG_ELECTRICAL_LOADS;
            if (insert == "Электросети - Освещение") return BuiltInParameterGroup.PG_ELECTRICAL_LIGHTING;
            if (insert == "Текст") return BuiltInParameterGroup.PG_TEXT;
            if (insert == "Зависимости") return BuiltInParameterGroup.PG_CONSTRAINTS;
            if (insert == "Стадии") return BuiltInParameterGroup.PG_PHASING;
            if (insert == "Механизмы") return BuiltInParameterGroup.PG_MECHANICAL;
            if (insert == "Несущие конструкции") return BuiltInParameterGroup.PG_STRUCTURAL;
            if (insert == "Сантехника") return BuiltInParameterGroup.PG_PLUMBING;
            if (insert == "Электросети") return BuiltInParameterGroup.PG_ELECTRICAL;
            if (insert == "Материалы и отделка") return BuiltInParameterGroup.PG_MATERIALS;
            if (insert == "Графика") return BuiltInParameterGroup.PG_GRAPHICS;
            if (insert == "Строительство") return BuiltInParameterGroup.PG_CONSTRUCTION;
            if (insert == "Размеры") return BuiltInParameterGroup.PG_GEOMETRY;
            if (insert == "Идентификация") return BuiltInParameterGroup.PG_IDENTITY_DATA;
            if (insert == "Прочее") return BuiltInParameterGroup.INVALID;


            return BuiltInParameterGroup.INVALID;
        }
        private void CheckBoxInstance_CheckedChanged(object sender, EventArgs e)
        {
            ButtonAddExternalEvent.IsInstance = checkBoxInstance.Checked;
        }
        public static void AddPathsToPathToFamilyList(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                if (fileName.Contains(".rfa"))
                {
                    PathToFamilyList.Add(fileName);
                }
            }
            
        }
        public static void AddPathsAndSubpathsToPathToFamilyList(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                if (fileName.Contains(".rfa"))
                {
                    PathToFamilyList.Add(fileName);
                }
            }

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                AddPathsAndSubpathsToPathToFamilyList(subdirectory);


        }
        private void ComboBoxStartFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            treeViewFamilies.BeginUpdate();
            MainCommand.ListDirectory(treeViewFamilies, comboBoxStartFolder.Text);
            treeViewFamilies.EndUpdate();
        }

        private void CheckBoxSubfolders_CheckedChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(OutputTextFromFoldersTree))
            {
                if (!OutputTextFromFoldersTree.Contains(".rfa"))
                {
                    PathToFamilyList.Clear();
                    if (checkBoxSubfolders.Checked) AddPathsAndSubpathsToPathToFamilyList(OutputTextFromFoldersTree);
                    if (!checkBoxSubfolders.Checked) AddPathsToPathToFamilyList(OutputTextFromFoldersTree);
                }
            }

        }
    }

    public class ButtonAddExternalEvent : IExternalEventHandler
    {
        public static MainForm MainForm;
        public static ExternalCommandData CommandData;
        public static List<string> PathToFamilyList;
        public static string SharedParameter;
        public static bool IsInstance;
        public static BuiltInParameterGroup PGGroup;
        public void Execute(UIApplication uiApp)
        {
            foreach (string PathToFamily in PathToFamilyList)
            {
                var doc = CommandData.Application.Application.OpenDocumentFile(PathToFamily);
                var resultText = AddSharedParameterIntoFamily(CommandData, doc, SharedParameter, IsInstance, PGGroup);
                MainForm.textBoxResult.AppendText(resultText + Environment.NewLine);
            }

            /*
            MessageBox.Show(
                PathToFamily + 
                Environment.NewLine + 
                CommandData.Application.ActiveUIDocument.ActiveView.Name +
                Environment.NewLine +
                SharedParameter +
                Environment.NewLine +
                GroupNameBySharedParameterName(CommandData, SharedParameter)
                );
            */
            return;
        }

        public string GetName()
        {
            return "External Event";
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
            if (!doc.IsFamilyDocument) return "не семейство";
            FamilyManager familyManager = doc.FamilyManager;
            FamilyType familyType = familyManager.CurrentType;
            FamilyTypeSet types = familyManager.Types;

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

            FamilyParameterSet parametersList = familyManager.Parameters;
            foreach (FamilyParameter p in parametersList)
            {
                if (p.Definition.Name == sharedParameterName) return "! " + "Параметр " + sharedParameterName + " существует в семействе " + doc.Title + ".rfa";
            }

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
                //using (Transaction t = new Transaction(doc, "Something happen"))
                //{
                //    t.Start();
                    //DefinitionFile sharedParametersFile = commandData.Application.Application.OpenSharedParameterFile();
                    //DefinitionGroup sharedParametersGroup = sharedParametersFile.Groups.get_Item(GroupNameBySharedParameterName(commandData, sharedParameterName));
                    //Definition sharedParameterDefinition = sharedParametersGroup.Definitions.get_Item(sharedParameterName);
                    //ExternalDefinition externalDefinition = sharedParameterDefinition as ExternalDefinition;
                    //str = sharedParameterName + " не удалось добавить в семейство " + doc.Title + ".rfa";
                //    t.Commit();
                //}
                str = "! " + sharedParameterName + " не удалось добавить в семейство " + doc.Title + ".rfa";
                //TaskDialog.Show("!", e.ToString());
            }
            doc.Save();
            string docDir = Path.GetDirectoryName(doc.PathName);
            doc.Close();
            try
            {
                string[] fileEntries = Directory.GetFiles(docDir, "*.0???.rfa");
                foreach (string fileName in fileEntries)
                    File.Delete(fileName);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
            return str;
        }

    }
    public class ButtonDeleteExternalEvent : IExternalEventHandler
    {
        public static List<string> PathToFamilyList;
        public static MainForm MainForm;
        public static ExternalCommandData CommandData;
        public static string SharedParameter;
        public void Execute(UIApplication app)
        {
            foreach (string PathToFamily in PathToFamilyList)
            {
                var doc = CommandData.Application.Application.OpenDocumentFile(PathToFamily);
                var resultText = DeleteSharedParameterFromFamily(CommandData, doc, SharedParameter);
                MainForm.textBoxResult.AppendText(resultText + Environment.NewLine);
            }
            return;
        }

        public string GetName()
        {
            return "External Delete Event";
        }
        private string DeleteSharedParameterFromFamily(ExternalCommandData commandData, Document doc, string sharedParameterName)
        {
            string str = "";
            if (!doc.IsFamilyDocument) return "не семейство";

            FamilyManager familyManager = doc.FamilyManager;
            FamilyType familyType;
            familyType = familyManager.CurrentType;
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

            #region clear 
            //TaskDialog.Show("Warning", "Privet");
            try
            {
                commandData.Application.Application.SharedParametersFilename = MainCommand.FOPPath;
                using (Transaction t = new Transaction(doc, "Clear"))
                {
                    t.Start();
                    FamilyParameterSet parametersList = familyManager.Parameters;

                    try
                    {
                        DefinitionFile sharedParametersFile = commandData.Application.Application.OpenSharedParameterFile();
                        DefinitionGroup sharedParametersGroup = sharedParametersFile.Groups.get_Item(GroupNameBySharedParameterName(commandData, sharedParameterName));
                        Definition sharedParameterDefinition = sharedParametersGroup.Definitions.get_Item(sharedParameterName);
                        ExternalDefinition externalDefinition = sharedParameterDefinition as ExternalDefinition;

                        var p = familyManager.get_Parameter(externalDefinition.GUID);
                        familyManager.RemoveParameter(p);

                        str = "- " + sharedParameterName + " был успешно удален из семейства " + doc.Title + ".rfa";
                    }
                    catch
                    {
                        str = "! " + sharedParameterName + " отсутсвует в семействе " + doc.Title + ".rfa";
                    }


                    t.Commit();
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("Warning 1", e.ToString());
            }

            #endregion

            doc.Save();
            string docDir = Path.GetDirectoryName(doc.PathName);
            doc.Close();
            try
            {
                string[] fileEntries = Directory.GetFiles(docDir, "*.0???.rfa");
                foreach (string fileName in fileEntries)
                    File.Delete(fileName);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
            return str;
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
    }
}
