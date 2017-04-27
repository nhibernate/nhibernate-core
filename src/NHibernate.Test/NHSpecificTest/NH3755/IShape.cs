using System;

namespace NHibernate.Test.NHSpecificTest.NH3755
{
	public interface IShape
	{
		Guid Id { get; set; }
		string Property1 { get; set; }
	}
}
