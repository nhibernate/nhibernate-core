using System;

namespace NHibernate.Test.NHSpecificTest.IlogsProxyTest
{
	public interface IEntityProxy
	{
		Guid Id { get; set; }

		string Name { get; set; }
	}
}
