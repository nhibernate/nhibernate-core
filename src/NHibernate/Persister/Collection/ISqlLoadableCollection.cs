using System;
#if NET_2_0
using System.Collections.Generic;
#endif
using System.Text;

namespace NHibernate.Persister.Collection
{
	public interface ISqlLoadableCollection : IQueryableCollection
	{
		string[] GetCollectionPropertyColumnAliases( string propertyName, string str );
		string IdentifierColumnName { get; }
	}
}
