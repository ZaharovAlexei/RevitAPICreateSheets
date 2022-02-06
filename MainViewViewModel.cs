using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using RevitAPICreateSheets.Wrappers;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPICreateSheets
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        private Document _doc;

        public List<FamilySymbol> Formats { get; } = new List<FamilySymbol>();
        public List<View> Views { get; } = new List<View>();
        public DelegateCommand CreateCommand { get; }
        public View SelectedView { get; set; }
        public int SheetsCount { get; set; } = 1;
        public string DesignBy { get; set; } = "Захаров";
        public FamilySymbol SelectedFormat { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            _doc = _commandData.Application.ActiveUIDocument.Document;
            Formats = FormatUtils.GetFormats(_doc);
            Views = ViewsUtils.GetViews(_doc);
            CreateCommand = new DelegateCommand(OnCreateCommand);
        }

        private void OnCreateCommand()
        {
            if (SelectedView == null || SelectedFormat == null)
                return;

            using (var ts = new Transaction(_doc, "Create sheets"))
            {
                ts.Start();
                List<ViewSheet> sheets = new List<ViewSheet>();

                for (int i = 0; i < SheetsCount; i++)
                {
                    ViewSheet viewSheet = ViewSheet.Create(_doc, SelectedFormat.Id);
                    viewSheet.Name = $"Лист {i + 1}";
                    viewSheet.SheetNumber = (i + 1).ToString();

                    Parameter parameter = viewSheet.get_Parameter(BuiltInParameter.SHEET_DESIGNED_BY);
                    parameter.Set(DesignBy);

                    sheets.Add(viewSheet);
                }
                ViewSheet viewSheet1 = sheets.FirstOrDefault();
                UV location = new UV((viewSheet1.Outline.Max.U - viewSheet1.Outline.Min.U) / 2,
                (viewSheet1.Outline.Max.V - viewSheet1.Outline.Min.V) / 2);

                Viewport.Create(_doc, viewSheet1.Id, SelectedView.Id, new XYZ(location.U, location.V, 0));

                ts.Commit();

            }
            RaiseCloseRequest();
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

    }

}

