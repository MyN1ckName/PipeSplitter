using System;
using Autodesk.Revit.UI;

namespace PipeSplitter
{
	class App : IExternalApplication
	{
		static readonly string ExecutingAssemblyPath = System.Reflection.Assembly
			.GetExecutingAssembly().Location;

		public Result OnStartup(UIControlledApplication app)
		{
			RibbonPanel rvtRibbonPanel = app.CreateRibbonPanel("PipeSplitter");
			PushButtonData dataButton = new PushButtonData("Button", "PipeSplitter"
				, ExecutingAssemblyPath, "PipeSplitter.Command");

			PushButton button = rvtRibbonPanel.AddItem(dataButton) as PushButton;

			button.LargeImage = new System.Windows.Media.Imaging.BitmapImage
				(new Uri("pack://application:,,,/PipeSplitter;component/icon.ico"
				, UriKind.Absolute));

			button.ToolTip =
				"Split Pipes by Selected Pipe Type";

			return Result.Succeeded;
		}

		public Result OnShutdown(UIControlledApplication app)
		{
			return Result.Succeeded;
		}
	}
}