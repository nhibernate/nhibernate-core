using System;

namespace NHibernate.Test.NHSpecificTest.NH3755
{
	public class ShapeContainer : IShapeContainer
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IShape Shape { get; set; }
	}
}
