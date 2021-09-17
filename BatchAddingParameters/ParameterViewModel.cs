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
    public class ParameterViewModel
    {
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string ParameterGroup { get; set; }
        public string ParameterType { get; set; }
        public string FamilyParameterGroup { get; set; }
        public string FamilyParameterType { get; set; }
        public string FamilyValue { get; set; }

        public ParameterViewModel()
        {

        }

        private List<string> GetGroupsOfFOP(Application app)
        {
            List<string> output = new List<string>();
            app.SharedParametersFilename = MainCommand.FOPPath;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (var item in definitionGroups)
            {
                output.Add(item.Name);
            }
            return output;
        }
        private List<List<string>> GetGroupItemsOfFOP(Application app)
        {
            List<List<string>> output = new List<List<string>>();
            app.SharedParametersFilename = MainCommand.FOPPath;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (DefinitionGroup definitionGroup in definitionGroups)
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
        public ParameterViewModel[] AllParameters(Application app)
        {
            var output = new List<ParameterViewModel>();
            app.SharedParametersFilename = MainCommand.FOPPath;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (DefinitionGroup definitionGroup in definitionGroups)
            {
                foreach (Definition definition in definitionGroup.Definitions)
                {
                    ExternalDefinition externalDefinition = definition as ExternalDefinition;
                    var parameterProperties = new ParameterViewModel();
                    parameterProperties.Id = "";
                    parameterProperties.Guid = externalDefinition.GUID.ToString();
                    parameterProperties.Name = externalDefinition.Name;
                    parameterProperties.ParameterGroup = externalDefinition.OwnerGroup.Name;
                    parameterProperties.ParameterType = definition.ParameterType.ToString();
                    parameterProperties.FamilyParameterGroup = "Прочее";
                    parameterProperties.FamilyParameterType = "Тип";
                    parameterProperties.FamilyValue = "";

                    output.Add(parameterProperties);
                }
            }


            return output.ToArray();
        }
    }


}
