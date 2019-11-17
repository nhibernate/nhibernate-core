using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public interface INamedEntity
	{
		int Id { get; }

		string Name { get; }
	}
}
