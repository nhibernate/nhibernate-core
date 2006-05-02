using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Persister.Collection
{
	public interface ISqlLoadableCollection : IQueryableCollection
	{
		string[] GetCollectionPropertyColumnAliases( string propertyName, string str );
		string IdentifierColumnName { get; }
	}
}
