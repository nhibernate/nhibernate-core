using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public interface IEntity<TId>
	{
		TId Id { get; set; }
	}

	public interface IEntity : IEntity<int>
	{
	}
}
