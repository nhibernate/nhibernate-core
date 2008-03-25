using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Persister.Collection
{
	/// <summary>
	/// Summary description for CollectionPropertyMapping.
	/// </summary>
	public class CollectionPropertyMapping : IPropertyMapping
	{
		private readonly IQueryableCollection memberPersister;

		public CollectionPropertyMapping(IQueryableCollection memberPersister)
		{
			this.memberPersister = memberPersister;
		}

		public IType ToType(string propertyName)
		{
			switch (propertyName)
			{
				case CollectionPropertyNames.Elements:
					return memberPersister.ElementType;

				case CollectionPropertyNames.Indices:
					if (!memberPersister.HasIndex)
						throw new QueryException("unindexed collection before indices()");
					return memberPersister.IndexType;

				case CollectionPropertyNames.Size:
					return NHibernateUtil.Int32;

				case CollectionPropertyNames.MaxIndex:
				case CollectionPropertyNames.MinIndex:
					return memberPersister.IndexType;

				case CollectionPropertyNames.MaxElement:
				case CollectionPropertyNames.MinElement:
					return memberPersister.ElementType;
				default:
					throw new QueryException("illegal syntax near collection: " + propertyName);
			}
		}

		public string[] ToColumns(string alias, string propertyName)
		{
			string[] cols;
			switch (propertyName)
			{
				case CollectionPropertyNames.Elements:
					return memberPersister.GetElementColumnNames(alias);

				case CollectionPropertyNames.Indices:
					if (!memberPersister.HasIndex)
						throw new QueryException("unindexed collection before indices()");
					return memberPersister.GetIndexColumnNames(alias);

				case CollectionPropertyNames.Size:
					cols = memberPersister.KeyColumnNames;
					return new string[] { "count(" + alias + '.' + cols[0] + ')' };

				case CollectionPropertyNames.MaxIndex:
					if (!memberPersister.HasIndex)
						throw new QueryException("unindexed collection in maxIndex()");
					cols = memberPersister.GetIndexColumnNames(alias);
					if (cols.Length != 1)
						throw new QueryException("composite collection index in maxIndex()");
					return new string[] { "max(" + cols[0] + ')' };

				case CollectionPropertyNames.MinIndex:
					if (!memberPersister.HasIndex)
						throw new QueryException("unindexed collection in minIndex()");
					cols = memberPersister.GetIndexColumnNames(alias);
					if (cols.Length != 1)
						throw new QueryException("composite collection index in minIndex()");
					return new string[] { "min(" + cols[0] + ')' };

				case CollectionPropertyNames.MaxElement:
					cols = memberPersister.GetElementColumnNames(alias);
					if (cols.Length != 1)
						throw new QueryException("composite collection element in maxElement()");
					return new string[] { "max(" + cols[0] + ')' };

				case CollectionPropertyNames.MinElement:
					cols = memberPersister.GetElementColumnNames(alias);
					if (cols.Length != 1)
						throw new QueryException("composite collection element in minElement()");
					return new System.String[] { "min(" + cols[0] + ')' };

				default:
					throw new QueryException("illegal syntax near collection: " + propertyName);
			}
		}

		public string[] ToColumns(string propertyName)
		{
			throw new System.NotSupportedException("References to collections must be define a SQL alias");
		}

		public IType Type
		{
			get { return memberPersister.CollectionType; }
		}
	}
}