using System;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace PipeSplitter
{
	[TransactionAttribute(TransactionMode.Manual)]
	[RegenerationAttribute(RegenerationOption.Manual)]
	public class Command : IExternalCommand
	{
		Document doc;

		public Result Execute(ExternalCommandData commandDate,
			ref string messege,
			ElementSet elements)
		{
			doc = commandDate.Application.ActiveUIDocument.Document;

			try
			{
				MainWindow.MainWindow window = new MainWindow.MainWindow()
				{
					DataContext = new MainWindow.ViewModel(doc)
				};
				window.ShowDialog();
						
				return Result.Succeeded;
			}

			catch (Autodesk.Revit.Exceptions.OperationCanceledException)
			{
				return Result.Cancelled;
			}
			catch (Exception ex)
			{
				messege = ex.Message;
				return Result.Failed;
			}
		}
	}
}