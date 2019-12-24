using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace PipeSplitter.MainWindow
{
	class ViewModel : INotifyPropertyChanged
	{
		Document doc;
		public ViewModel(Document doc)
		{
			this.doc = doc;
			GetPipesTypes(doc);
		}

		private ObservableCollection<MyPipeType> pipeTypes =
			new ObservableCollection<MyPipeType>();

		public ObservableCollection<MyPipeType> PipeTypes
		{
			get { return pipeTypes; }
		}

		private void GetPipesTypes(Document doc)
		{
			FilteredElementCollector collector = new FilteredElementCollector(doc);
			collector.OfClass(typeof(PipeType));
			foreach (PipeType pt in collector)
			{
				pipeTypes.Add(new MyPipeType(pt));
			}
		}

		public bool IsChecked
		{
			get { return selectedPipeType.IsChecked; }
			set
			{
				selectedPipeType.IsChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}

		private MyPipeType selectedPipeType;
		public MyPipeType SelectedPipeType
		{
			get { return selectedPipeType; }
			set
			{
				selectedPipeType = value;
				OnPropertyChanged("SelectedPipeType");
			}
		}

		private RelayCommand close;
		public RelayCommand Close
		{
			get
			{
				return close ??
					  (close = new RelayCommand(obj =>
					  {
						  try
						  {
							  Window window = obj as Window;
							  window.Close();
						  }
						  catch (Exception ex)
						  {
							  MessageBox.Show(ex.Message);
						  }
					  },
					  obj =>
					  {
						  return true;
					  }));
			}
		}

		private RelayCommand ok;
		public RelayCommand Ok
		{
			get
			{
				return ok ??
					  (ok = new RelayCommand(obj =>
					  {
						  try
						  {
							  Window window = obj as Window;
							  window.Close();

							  Model.PipeSplitter ps = new Model.PipeSplitter(doc);
							  List<PipeType> pt = new List<PipeType>();
							  foreach (MyPipeType mpt in pipeTypes)
							  {
								  if (mpt.IsChecked)
								  {
									  pt.Add(mpt.GetPipeType);
								  }
							  }
							  ps.Split(pt);
						  }
						  catch (Exception ex)
						  {
							  MessageBox.Show(ex.Message);
						  }
					  },
					  obj =>
					  {
						  return true;
					  }));
			}
		}

		// INotifyPropertyChanged interface implementation
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}