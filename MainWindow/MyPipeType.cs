using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace PipeSplitter.MainWindow
{
	class MyPipeType
	{
		PipeType pt;
		Parameter param;

		public MyPipeType(PipeType pt)
		{
			this.pt = pt;
			param = pt.LookupParameter("Длина трубы");
		}

		public PipeType GetPipeType
		{
			get { return pt; }
		}

		public string MaxLength
		{
			get
			{
				if (param != null)
					return param.AsValueString();
				else return "Not Find";
			}
		}

		public bool IsChecked { get; set; }
	}
}