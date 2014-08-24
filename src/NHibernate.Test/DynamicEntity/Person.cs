using System.Collections.Generic;

namespace NHibernate.Test.DynamicEntity
{
	public interface Person
	{
		long Id { get; set;}
		string Name { get; set;}
		Address Address { get;set;}
		ISet<Person> Family { get; set;}
	}
}