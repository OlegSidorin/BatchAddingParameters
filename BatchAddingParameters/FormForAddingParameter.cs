using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System;
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

namespace BatchAddingParameters
{
    public partial class FormForAddingParameter : System.Windows.Forms.Form
    {
        public ButtonExternalEvent ButtonExternalEvent;
        public ExternalEvent ExternalEvent;
        public ButtonDeleteExternalEvent ButtonDeleteExternalEvent;
        public ExternalEvent ExternalEventButtonDelete;
        public FormForAddingParameter()
        {
            InitializeComponent();
             
            treeViewParameters.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeViewParameters.DrawNode += new DrawTreeNodeEventHandler(TreeViewParameters_DrawNode);
            treeViewFamilies.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeViewFamilies.DrawNode += new DrawTreeNodeEventHandler(TreeViewFamilies_DrawNode);

            ButtonExternalEvent = new ButtonExternalEvent();
            ExternalEvent = ExternalEvent.Create(ButtonExternalEvent);
            ButtonDeleteExternalEvent = new ButtonDeleteExternalEvent();
            ExternalEventButtonDelete = ExternalEvent.Create(ButtonDeleteExternalEvent);
        }

        private void TreeViewFamilies_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected)
            {
                Graphics g = e.Graphics;
                // get node font and node fore color
                Font nodeFont = GetTreeNodeFont(e.Node);
                Color nodeForeColor = GetTreeNodeForeColor(e.Node, e.State);

                // fill node background
                using (SolidBrush brush = new SolidBrush(Color.DarkOliveGreen))
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
                string selectedFamily = e.Node.Text;
                if (selectedFamily.Contains(".rfa"))
                {
                    //buttonFamily.Text = e.Node.Text;
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
                    buttonFamily.Text = CommandForAddingParameters.DirectoryTreeStartDirectory + path.ToString();
                }
                    
            }
            else
            {
                e.DrawDefault = true;
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
                using (SolidBrush brush = new SolidBrush(Color.DarkOliveGreen))
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
                    buttonParameter.Text = e.Node.Text;
            }
            else
            {
                e.DrawDefault = true;
            }

        }
        private Font GetTreeNodeFont(TreeNode node)
        {
            Font font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Regular);
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

        private void ButtonAction_Click(object sender, EventArgs e)
        {
            ButtonExternalEvent.PathToFamily = buttonFamily.Text;
            ButtonExternalEvent.SharedParameter = buttonParameter.Text;
            ButtonExternalEvent.IsInstance = checkBoxInstance.Checked;

            if ((buttonFamily.Text != "") && (buttonParameter.Text != ""))
            {
                ExternalEvent.Raise();
            }
            else
            {
                textBoxResult.Text += "не хватает данных" + Environment.NewLine;
            }
            //TaskDialog.Show("1", buttonFamily.Text + " <-|-> "+ buttonParameter.Text);
            
            
            //Close();
        }

        private void ComboBoxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            ButtonExternalEvent.PGGroup = GetPGGroup(comboBoxGroup.Text);   
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
            ButtonExternalEvent.IsInstance = checkBoxInstance.Checked;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            ButtonDeleteExternalEvent.PathToFamily = buttonFamily.Text;
            ButtonDeleteExternalEvent.SharedParameter = buttonParameter.Text;

            if ((buttonFamily.Text != "") && (buttonParameter.Text != ""))
            {
                ExternalEventButtonDelete.Raise();
            }
            else
            {
                textBoxResult.Text += "не хватает данных" + Environment.NewLine;
            }
        }
    }

    public class ButtonExternalEvent : IExternalEventHandler
    {
        public static FormForAddingParameter FormForAddingParameter;
        public static ExternalCommandData CommandData;
        public static string PathToFamily;
        public static string SharedParameter;
        public static bool IsInstance;
        public static BuiltInParameterGroup PGGroup;
        public void Execute(UIApplication uiApp)
        {
            var doc = CommandData.Application.Application.OpenDocumentFile(PathToFamily);
            var resultText = AddSharedParameterInFamily(CommandData, doc, SharedParameter, IsInstance, PGGroup);
            FormForAddingParameter.textBoxResult.Text += resultText + Environment.NewLine;

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
        private string AddSharedParameterInFamily(ExternalCommandData commandData, Document doc, string sharedParameterName, bool isInstance, BuiltInParameterGroup group)
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
                if (p.Definition.Name == sharedParameterName) return "Параметр " + p.Definition.Name + " существует";
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
                    str = familyParameter.Definition.Name + " был успешно добавлен в семейство " + doc.Title + ".rfa";
                    t.Commit();
                }

            }
            catch (Exception e)
            {
                using (Transaction t = new Transaction(doc, "Something happen"))
                {
                    t.Start();
                    DefinitionFile sharedParametersFile = commandData.Application.Application.OpenSharedParameterFile();
                    DefinitionGroup sharedParametersGroup = sharedParametersFile.Groups.get_Item(GroupNameBySharedParameterName(commandData, sharedParameterName));
                    Definition sharedParameterDefinition = sharedParametersGroup.Definitions.get_Item(sharedParameterName);
                    ExternalDefinition externalDefinition = sharedParameterDefinition as ExternalDefinition;
                    str = sharedParameterName + " не удалось добавить в семейство " + doc.Title + ".rfa";
                    t.Commit();
                }
                
                //TaskDialog.Show("!", e.ToString());
            }
            doc.Save();
            doc.Close();
            return str;
        }
        
    }
    public class ButtonDeleteExternalEvent : IExternalEventHandler
    {
        public static string PathToFamily;
        public static FormForAddingParameter FormForAddingParameter;
        public static ExternalCommandData CommandData;
        public static string SharedParameter;
        public void Execute(UIApplication app)
        {
            var doc = CommandData.Application.Application.OpenDocumentFile(PathToFamily);
            var resultText = DeleteSharedParameterFromFamily(CommandData, doc, SharedParameter);
            //MessageBox.Show("Ops");
            FormForAddingParameter.textBoxResult.Text += resultText + Environment.NewLine;
            return;
        }

        public string GetName()
        {
            return "External Delete Event";
        }
        private string DeleteSharedParameterFromFamily(ExternalCommandData commandData, Document doc, string sharedParameter)
        {
            if (doc.IsFamilyDocument)
            {
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
                    commandData.Application.Application.SharedParametersFilename = CommandForAddingParameters.FOPPath;
                    using (Transaction t = new Transaction(doc, "Clear"))
                    {
                        t.Start();
                        FamilyParameterSet parametersList = familyManager.Parameters;


                            try
                            {
                                //var p = familyManager.get_Parameter(new Guid(guid));
                                familyManager.RemoveParameter(p);
                            }
                            catch
                            {

                            }


                        t.Commit();
                    }
                }
                catch (Exception e)
                {
                    TaskDialog.Show("Warning 1", e.ToString());
                }

                #endregion

            }
            else
            {
                TaskDialog.Show("Warning main", "Это не семейство, команда работает только в семействе");
            }

            doc.Save();
            doc.Close();
            return "не реализовано";
        }
    }
}
