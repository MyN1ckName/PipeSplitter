using System;
using System.Windows;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.Attributes;

namespace PipeSplitter.MainWindow
{
	[TransactionAttribute(TransactionMode.Manual)]
	[RegenerationAttribute(RegenerationOption.Manual)]
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
			set
			{
				try
				{
					using (Transaction t = new Transaction(pt.Document, "SetMaxLength"))
					{
						t.Start();
						param.SetValueString(value);
						t.Commit();
					}
				}
				catch (Exception)
				{
					MessageBox.Show("Failed to Set Value the Parameter", "Error");
				}
			}
		}
		public bool IsChecked { get; set; }
	}
}