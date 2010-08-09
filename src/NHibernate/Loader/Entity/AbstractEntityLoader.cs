using System.Collections;
using System.Collections.Generic;
using System.Data;

using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// Abstract superclass for entity loaders that use outer joins
	/// </summary>
	public abstract class AbstractEntityLoader : OuterJoinLoader, IUniqueEntityLoader
	{
		protected static readonly ILogger log = LoggerProvider.LoggerFor(typeof (AbstractEntityLoader));
		protected readonly IOuterJoinLoadable persister;
		protected readonly IType uniqueKeyType;
		protected readonly string entityName;

		public AbstractEntityLoader(IOuterJoinLoadable persister, IType uniqueKeyType, ISessionFactoryImplementor factory,
		                            IDictionary<string, IFilter> enabledFilters) : base(factory, enabledFilters)
		{
			this.uniqueKeyType = uniqueKeyType;
			entityName = persister.EntityName;
			this.persister = persister;
		}

		protected override bool IsSingleRowLoader
		{
			get { return true; }
		}

		public object Load(object id, object optionalObject, ISessionImplementor session)
		{
			return Load(session, id, optionalObject, id);
		}

		protected virtual object Load(ISessionImplementor session, object id, object optionalObject, object optionalId)
		{
			IList list = LoadEntity(session, id, uniqueKeyType, optionalObject, entityName, optionalId, persister);

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
					throw new HibernateException(
						string.Format("More than one row with the given identifier was found: {0}, for class: {1}", id,
						              persister.EntityName));
				}
			}
		}

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, IDataReader rs,
		                                               ISessionImplementor session)
		{
			return row[row.Length - 1];
		}
	}
}