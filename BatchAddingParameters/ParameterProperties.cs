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
    public class ParameterProperties
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Group { get; set; }
        public string ValueType { get; set; }
        public string Value { get; set; }

        public ParameterProperties()
        {

        }
        public ParameterProperties(string id, string name, string type, string group, string valueType, string value)
        {
            Id = id;
            Name = name;
            Type = type;
            Group = group;
            ValueType = valueType;
            Value = value;
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

        public ParameterProperties[] AllParameters(Application app)
        {
            var output = new List<ParameterProperties>();
            app.SharedParametersFilename = MainCommand.FOPPath;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (DefinitionGroup definitionGroup in definitionGroups)
            {
                foreach (Definition definition in definitionGroup.Definitions)
                {
                    var parameterProperties = new ParameterProperties();
                    parameterProperties.Id = "";
                    parameterProperties.Name = definition.Name;
                    parameterProperties.Group = definitionGroup.Name;
                    parameterProperties.Type = "";
                    parameterProperties.ValueType = definition.ParameterType.ToString();
                    parameterProperties.Value = "";

                    output.Add(parameterProperties);
                }
            }


            return output.ToArray();
        }
    }


}
