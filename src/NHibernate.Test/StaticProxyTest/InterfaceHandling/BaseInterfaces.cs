using System;

namespace NHibernate.Test.StaticProxyTest.InterfaceHandling
{
	public interface IEntity
	{
		Guid Id { get; set; }

		string Name { get; set; }
	}

	public interface IEntity2
	{
		Guid Id { get; set; }
	}
}
