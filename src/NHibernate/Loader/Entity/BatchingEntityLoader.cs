using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// "Batch" loads entities, using multiple primary key values in the
	/// SQL <c>where</c> clause.
	/// </summary>
	/// <seealso cref="EntityLoader"/>
	public class BatchingEntityLoader : IUniqueEntityLoader
	{
		private readonly Loader[] loaders;
		private readonly int[] batchSizes;
		private readonly IEntityPersister persister;
		private readonly IType idType;

		public BatchingEntityLoader(IEntityPersister persister, int[] batchSizes, Loader[] loaders)
		{
			this.batchSizes = batchSizes;
			this.loaders = loaders;
			this.persister = persister;
			idType = persister.IdentifierType;
		}

		private object GetObjectFromList(IList results, object id, ISessionImplementor session)
		{
			// get the right object from the list ... would it be easier to just call getEntity() ??
			foreach (object obj in results)
			{
				bool equal = idType.IsEqual(id, session.GetContextEntityIdentifier(obj), session.EntityMode, session.Factory);

				if (equal)
				{
					return obj;
				}
			}

			return null;
		}

		public object Load(object id, object optionalObject, ISessionImplementor session)
		{
			object[] batch =
				session.PersistenceContext.BatchFetchQueue.GetEntityBatch(persister, id, batchSizes[0], session.EntityMode);

			for (int i = 0; i < batchSizes.Length - 1; i++)
			{
				int smallBatchSize = batchSizes[i];
				if (batch[smallBatchSize - 1] != null)
				{
					object[] smallBatch = new object[smallBatchSize];
					Array.Copy(batch, 0, smallBatch, 0, smallBatchSize);

					IList results =
						loaders[i].LoadEntityBatch(session, smallBatch, idType, optionalObject, persister.EntityName, id, persister);

					return GetObjectFromList(results, id, session); //EARLY EXIT
				}
			}

			return ((IUniqueEntityLoader) loaders[batchSizes.Length - 1]).Load(id, optionalObject, session);
		}

		public static IUniqueEntityLoader CreateBatchingEntityLoader(IOuterJoinLoadable persister, int maxBatchSize,
		                                                             LockMode lockMode, ISessionFactoryImplementor factory,
		                                                             IDictionary<string, IFilter> enabledFilters)
		{
			if (maxBatchSize > 1)
			{
				int[] batchSizesToCreate = ArrayHelper.GetBatchSizes(maxBatchSize);
				Loader[] loadersToCreate = new Loader[batchSizesToCreate.Length];
				for (int i = 0; i < batchSizesToCreate.Length; i++)
				{
					loadersToCreate[i] = new EntityLoader(persister, batchSizesToCreate[i], lockMode, factory, enabledFilters);
				}
				return new BatchingEntityLoader(persister, batchSizesToCreate, loadersToCreate);
			}
			else
			{
				return new EntityLoader(persister, lockMode, factory, enabledFilters);
			}
		}
	}
}