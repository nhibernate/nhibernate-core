using System;

namespace NHibernate.Test.StaticProxyTest.InterfaceHandling
{
	public interface IEntity
	{
		Guid Id { get; set; }

		string Name { get; set; }
	}
	
	public interface IEntityId
	{
		Guid Id { get; set; }
	}
}
