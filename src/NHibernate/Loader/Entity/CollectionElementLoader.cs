using System.Collections;
using System.Collections.Generic;
using System.Data;
using log4net;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Entity
{
	public class CollectionElementLoader : OuterJoinLoader
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (CollectionElementLoader));

		private readonly IOuterJoinLoadable persister;
		private readonly IType keyType;
		private readonly IType indexType;
		private readonly string entityName;

		public CollectionElementLoader(IQueryableCollection collectionPersister, ISessionFactoryImplementor factory,
		                               IDictionary<string, IFilter> enabledFilters) : base(factory, enabledFilters)
		{
			keyType = collectionPersister.KeyType;
			indexType = collectionPersister.IndexType;
			persister = (IOuterJoinLoadable) collectionPersister.ElementPersister;
			entityName = persister.EntityName;

			JoinWalker walker =
				new EntityJoinWalker(persister,
				                     ArrayHelper.Join(collectionPersister.KeyColumnNames, collectionPersister.IndexColumnNames), 1,
				                     LockMode.None, factory, enabledFilters);
			InitFromWalker(walker);

			PostInstantiate();

			log.Debug("Static select for entity " + entityName + ": " + SqlString);
		}

		protected override bool IsSingleRowLoader
		{
			get { return true; }
		}

		public virtual object LoadElement(ISessionImplementor session, object key, object index)
		{
			IList list = LoadEntity(session, key, index, keyType, indexType, persister);

			if (list.Count == 1)
			{
				return list[0];
			}
			else if (list.Count == 0)
			{
				return null;
			}
			else
			{
				if (CollectionOwners != null)
				{
					return list[0];
				}
				else
				{
					throw new HibernateException("More than one row was found");
				}
			}
		}

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer transformer, IDataReader rs,
		                                               ISessionImplementor session)
		{
			return row[row.Length - 1];
		}
	}
}