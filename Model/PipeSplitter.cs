using System;
using System.Collections.Generic;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace PipeSplitter.Model
{
	[TransactionAttribute(TransactionMode.Manual)]
	[RegenerationAttribute(RegenerationOption.Manual)]
	class PipeSplitter
	{
		Document doc;

		public PipeSplitter(Document doc)
		{
			this.doc = doc;
		}

		public void Split(List<PipeType> pipeTypes)
		{
			using (TransactionGroup tg = new TransactionGroup(doc, "PipeSplitter"))
			{
				tg.Start();

				foreach (PipeType pipeType in pipeTypes)
				{
					double maxLength = pipeType.LookupParameter("Длина трубы").AsDouble();

					var collector = new FilteredElementCollector(doc)
						.OfClass(typeof(Pipe)).ToElementIds();

					foreach (var id in collector)
					{
						Pipe pipe = doc.GetElement(id) as Pipe;

						if (pipe.PipeType.Name == pipeType.Name)
						{
							LocationCurve locCurve = pipe.Location as LocationCurve;
							Curve curve = locCurve.Curve;

							if (curve.Length > maxLength)
							{
								ElementId systemTypeId =
								pipe.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM)
								.AsElementId();
								ElementId levelId =
									pipe.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM).AsElementId();
								double diameter =
									pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();

								Delete(id);
								CreatePipe(doc, systemTypeId, pipeType, levelId, diameter
									, GetPoints(curve, maxLength));
							}
						}
					}
				}
				tg.Assimilate();
			}
		}

		// Метод создает трубы между точками в List<XYZ> points
		private List<Pipe> CreatePipe(Document doc, ElementId systemTypeId
			, PipeType pipeType, ElementId levelId, double diameter, List<XYZ> points)
		{
			List<Pipe> pipes = new List<Pipe>();
			Pipe pipe;

			for (int i = 0; i < points.Count - 1; i++)
			{
				using (Transaction t = new Transaction(doc, "PipeCreate"))
				{
					t.Start();
					pipe = Pipe.Create(doc, systemTypeId, pipeType.Id, levelId
						, points[i], points[i + 1]);
					pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM)
						.Set(diameter);
					t.Commit();
				}
				pipes.Add(pipe);
			}
			CreateFitting(doc, pipes);
			return pipes;
		}

		// Метод соеденяет трубы прямым фиттингом (UnionFitting) 
		private void CreateFitting(Document doc, List<Pipe> pipes)
		{
			for (int i = 0; i < pipes.Count - 1; i++)
			{
				LocationCurve locCurve = pipes[i].Location as LocationCurve;
				XYZ connectPoint = locCurve.Curve.GetEndPoint(1);
				Connector connector1 = Util.GetConnectorClosestTo(pipes[i], connectPoint);
				Connector connector2 = Util.GetConnectorClosestTo(pipes[i + 1], connectPoint);
				using (Transaction t = new Transaction(doc, "ConnectToConnect"))
				{
					t.Start();
					doc.Create.NewUnionFitting(connector1, connector2);
					t.Commit();
				}
			}
		}

		// Метод возвращает точки на прямой curve с заданным шагом maxLenght
		private List<XYZ> GetPoints(Curve curve, double maxLenght)
		{
			Line line = curve as Line;
			List<XYZ> points = new List<XYZ>();

			XYZ direction = line.Direction;
			XYZ origin = line.GetEndPoint(0);

			points.Add(line.GetEndPoint(0));
			for (double i = 0; i < (line.Length / maxLenght) - 1; i++)
			{
				XYZ p = GetPoint(origin, direction, maxLenght);
				points.Add(p);
				origin = p;
			}
			points.Add(line.GetEndPoint(1));
			return points;
		}

		// Получение точки на растоянии step от заданной точки origin
		private XYZ GetPoint(XYZ origin, XYZ direction, double step)
		{
			double x = origin.X;
			double y = origin.Y;
			double z = origin.Z;

			if (direction.X < 0)
				x = x - Math.Abs(direction.X * step);
			if (direction.X > 0)
				x = x + Math.Abs(direction.X * step);

			if (direction.Y < 0)
				y = y - Math.Abs(direction.Y * step);
			if (direction.Y > 0)
				y = y + Math.Abs(direction.Y * step);

			if (direction.Z < 0)
				z = z - Math.Abs(direction.Z * step);
			if (direction.Z > 0)
				z = z + Math.Abs(direction.Z * step);

			return new XYZ(x, y, z);
		}

		// Удаление элемента (трубы)
		private void Delete(ElementId id)
		{
			using (Transaction t = new Transaction(doc, "PipeDelete"))
			{
				t.Start();
				doc.Delete(id);
				t.Commit();
			}
		}
	}
}