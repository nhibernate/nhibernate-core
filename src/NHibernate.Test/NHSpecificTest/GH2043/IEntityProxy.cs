using System;

namespace NHibernate.Test.NHSpecificTest.GH2043
{
	public interface IEntityProxy
	{
		Guid Id { get; set; }

		string Name { get; set; }
	}
}
