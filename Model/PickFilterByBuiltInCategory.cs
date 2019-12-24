using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace PipeSplitter.Model
{
	class PickFilterByBuiltInCategory : ISelectionFilter
	{
		BuiltInCategory category;
		public PickFilterByBuiltInCategory(BuiltInCategory category)
		{
			this.category = category;
		}
		public bool AllowElement(Element e)
		{
			return (e.Category.Id.IntegerValue.Equals((int)category));
		}
		public bool AllowReference(Reference r, XYZ p)
		{
			return false;
		}
	}
}