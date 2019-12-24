using Autodesk.Revit.DB.Plumbing;

namespace PipeSplitter.MainWindow
{
	class MyPipeType
	{
		PipeType pt;
		string maxLenght;

		public MyPipeType(PipeType pt)
		{
			this.pt = pt;
			maxLenght = pt.LookupParameter("Длина трубы").AsValueString();
		}

		public PipeType GetPipeType
		{
			get { return pt; }
		}

		public string MaxLength
		{
			get { return maxLenght; }
		}

		public bool IsChecked { get; set; }
	}
}