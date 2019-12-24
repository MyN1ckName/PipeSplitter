using System;
using Autodesk.Revit.DB;

namespace PipeSplitter.Model
{
	class Util
	{
		#region скопировал отсюда https://adn-cis.org/prisoedinenie-naklonnogo-segmenta-truboprovoda-k-sosednim-trubam.html
		/// <summary>
		/// Возвращает ConnectorManager для заданного элемента,
		/// используя свойство MEPModel для экземпляра семейства
		/// или напрямую, если заданный элемент является
		/// трубой или воздуховодом
		/// </summary>
		static ConnectorManager GetConnectorManager(
		  Element e)
		{
			MEPCurve mc = e as MEPCurve;
			FamilyInstance fi = e as FamilyInstance;

			if (null == mc && null == fi)
			{
				throw new ArgumentException(
				  "Элемент не является ни фитингом, ни трубой или воздуховодом.");
			}

			return null == mc
			  ? fi.MEPModel.ConnectorManager
			  : mc.ConnectorManager;
		}

		/// <summary>
		/// Соединяет два заданных элемента в точке p.
		/// </summary>
		/// <exception cref="ArgumentException">Возникает, если один из элементов не имеет соединителей
		/// </exception>
		public static void Connect(
		  XYZ p,
		  Element a,
		  Element b)
		{
			ConnectorManager cm = GetConnectorManager(a);

			if (null == cm)
			{
				throw new ArgumentException(
				  "Элемент А не имеет соединителей.");
			}

			Connector ca = GetConnectorClosestTo(
			  cm.Connectors, p);

			cm = GetConnectorManager(b);

			if (null == cm)
			{
				throw new ArgumentException(
				  " Элемент В не имеет соединителей.");
			}

			Connector cb = GetConnectorClosestTo(
			  cm.Connectors, p);

			ca.ConnectTo(cb);
			//cb.ConnectTo( ca );
		}
		#endregion

		#region скопировал отсюда https://github.com/jeremytammik/the_building_coder_samples/blob/master/BuildingCoder/BuildingCoder/Util.cs
		/// <summary>
		/// Return the connector set element
		/// closest to the given point.
		/// </summary>
		/// 
		static Connector GetConnectorClosestTo(
		  ConnectorSet connectors,
		  XYZ p)
		{
			Connector targetConnector = null;
			double minDist = double.MaxValue;

			foreach (Connector c in connectors)
			{
				double d = c.Origin.DistanceTo(p);

				if (d < minDist)
				{
					targetConnector = c;
					minDist = d;
				}
			}
			return targetConnector;
		}

		/// <summary>
		/// Return the connector on the element 
		/// closest to the given point.
		/// </summary>
		public static Connector GetConnectorClosestTo(
		  Element e,
		  XYZ p)
		{
			ConnectorManager cm = GetConnectorManager(e);

			return null == cm
			  ? null
			  : GetConnectorClosestTo(cm.Connectors, p);
		}

		#endregion
	}
}