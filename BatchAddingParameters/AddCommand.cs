using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;

namespace BatchAddingParameters
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class AddCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("!", "Privet_M1");
            return Result.Succeeded;
        }
    }
}
