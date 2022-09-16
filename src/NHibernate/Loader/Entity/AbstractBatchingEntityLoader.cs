using System.Collections;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// The base contract for loaders capable of performing batch-fetch loading of entities using multiple primary key
	/// values in the SQL <tt>WHERE</tt> clause.
	/// </summary>
	public abstract partial class AbstractBatchingEntityLoader : IUniqueEntityLoader
	{
		protected IEntityPersister Persister { get; }

		protected AbstractBatchingEntityLoader(IEntityPersister persister)
		{
			Persister = persister;
		}

		protected virtual QueryParameters BuildQueryParameters(object id, object[] ids, object optionalObject)
		{
			IType[] types = new IType[ids.Length];
			ArrayHelper.Fill(types, Persister.IdentifierType);

			QueryParameters qp = new QueryParameters
			{
				PositionalParameterTypes = types,
				PositionalParameterValues = ids,
				OptionalObject = optionalObject,
				OptionalEntityName = Persister.EntityName,
				OptionalId = id,
			};
			return qp;
		}

		protected object GetObjectFromList(IList results, object id, ISessionImplementor session)
		{
			// get the right object from the list ... would it be easier to just call getEntity() ??
			foreach (object obj in results)
			{
				bool equal = Persister.IdentifierType.IsEqual(id, session.GetContextEntityIdentifier(obj), session.Factory);

				if (equal)
				{
					return obj;
				}
			}

			return null;
		}

		public abstract object Load(object id, object optionalObject, ISessionImplementor session);
	}
}
