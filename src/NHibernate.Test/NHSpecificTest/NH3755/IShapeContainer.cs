using System;

namespace NHibernate.Test.NHSpecificTest.NH3755
{
	public interface IShapeContainer
	{
		Guid Id { get; set; }
		string Name { get; set; }
		IShape Shape { get; set; }
	}
}
