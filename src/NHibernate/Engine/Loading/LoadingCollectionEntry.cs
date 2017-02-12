using System;
using System.Data.Common;
using NHibernate.Collection;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Engine.Loading
{
	/// <summary> 
	/// Represents a collection currently being loaded. 
	/// </summary>
	public class LoadingCollectionEntry
	{
		private readonly DbDataReader resultSet;
		private readonly ICollectionPersister persister;
		private readonly object key;
		private readonly IPersistentCollection collection;

		public LoadingCollectionEntry(DbDataReader resultSet, ICollectionPersister persister, object key, IPersistentCollection collection)
		{
			this.resultSet = resultSet;
			this.persister = persister;
			this.key = key;
			this.collection = collection;
		}

		public DbDataReader ResultSet
		{
			get { return resultSet; }
		}

		public ICollectionPersister Persister
		{
			get { return persister; }
		}

		public object Key
		{
			get { return key; }
		}

		public IPersistentCollection Collection
		{
			get { return collection; }
		}

		public override string ToString()
		{
			return GetType().FullName + "<rs=" + ResultSet + ", coll=" + MessageHelper.InfoString(Persister.Role, Key) + ">@" + Convert.ToString(GetHashCode(), 16);
		}
	}
}
