using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPICreateSheets.Wrappers
{
    public class FormatWrapper
    {
        public FormatWrapper(FamilySymbol familySymbol)
        {
            FamSymbol = familySymbol;
            Name = familySymbol.Name;
        }

        public bool IsSelected { get; set; }
        public FamilySymbol FamSymbol { get; }
        public string Name { get; }

    }
}
