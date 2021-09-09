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
        public ButtonExternalEvent buttonExternalEvent;
        public ExternalEvent externalEvent;
        public FormForAddingParameter()
        {
            InitializeComponent();

            treeViewParameters.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeViewParameters.DrawNode += new DrawTreeNodeEventHandler(TreeViewParameters_DrawNode);
            treeViewFamilies.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeViewFamilies.DrawNode += new DrawTreeNodeEventHandler(TreeViewFamilies_DrawNode);

            buttonExternalEvent = new ButtonExternalEvent();
            externalEvent = ExternalEvent.Create(buttonExternalEvent);
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
                externalEvent.Raise();
            }
            else
            {
                buttonResult.Text = "не хватает данных";
            }
            //TaskDialog.Show("1", buttonFamily.Text + " <-|-> "+ buttonParameter.Text);
            
            
            //Close();
        }
    }

    public class ButtonExternalEvent : IExternalEventHandler
    {
        public static FormForAddingParameter FormForAddingParameter;
        public static ExternalCommandData CommandData;
        public static string PathToFamily;
        public static string SharedParameter;
        public static bool IsInstance;
        public void Execute(UIApplication uiApp)
        {
            var doc = CommandData.Application.Application.OpenDocumentFile(PathToFamily);
            var resultText = AddSharedParameterInFamily(CommandData, doc, SharedParameter, IsInstance);
            //FormForAddingParameter.buttonResult.Text = resultText;
            FormForAddingParameter.textBoxResult.Text = resultText;

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
        private string AddSharedParameterInFamily(ExternalCommandData commandData, Document doc, string sharedParameterName, bool isInstance)
        {
            string str = "";
            if (!doc.IsFamilyDocument) return "не семейство";
            FamilyManager familyManager = doc.FamilyManager;
            //FamilyType familyType = familyManager.CurrentType;
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
                    FamilyParameter familyParameter = familyManager.AddParameter(externalDefinition, BuiltInParameterGroup.PG_TEXT, isInstance);
                    str = familyParameter.Definition.Name + " был успешно добавлен в семейство " + doc.Title + ".rfa";
                    t.Commit();
                }

            }
            catch (Exception e)
            {
                TaskDialog.Show("!", e.ToString());
            }
            return str;
        }
    }
    
}
