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
		private readonly Dictionary<int, EntityLoader> loaders = new Dictionary<int, EntityLoader>();
		private readonly int[] batchSizes;
		private readonly IOuterJoinLoadable persister;
		private readonly IType idType;
		private readonly LockMode lockMode;
		private readonly ISessionFactoryImplementor factory;
		private readonly IDictionary<string, IFilter> enabledFilters;

		public BatchingEntityLoader(IOuterJoinLoadable persister, int[] batchSizes, LockMode lockMode, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			this.batchSizes = batchSizes;
			this.persister = persister;
			this.lockMode = lockMode;
			this.factory = factory;
			this.enabledFilters = enabledFilters;
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

		private EntityLoader GetEntityLoader(int index, int smallBatchSize)
		{
			EntityLoader loader;
			if (!this.loaders.TryGetValue(index, out loader))
			{
				loader  = new EntityLoader(this.persister, smallBatchSize, this.lockMode, this.factory, this.enabledFilters);
				this.loaders[index] = loader;
			}
			return loader;
		}

		public object Load(object id, object optionalObject, ISessionImplementor session)
		{
			object[] batch =
				session.PersistenceContext.BatchFetchQueue.GetEntityBatch(persister, id, batchSizes[0]);

			for (int i = 0; i < batchSizes.Length - 1; i++)
			{
				int smallBatchSize = batchSizes[i];
				if (batch[smallBatchSize - 1] != null)
				{
					object[] smallBatch = new object[smallBatchSize];
					Array.Copy(batch, 0, smallBatch, 0, smallBatchSize);

					EntityLoader loader = this.GetEntityLoader(i, smallBatchSize);
					IList results =
						loader.LoadEntityBatch(session, smallBatch, idType, optionalObject, persister.EntityName, id, persister);

					return GetObjectFromList(results, id, session); //EARLY EXIT
				}
			}

			int index = batchSizes.Length - 1;
			return ((IUniqueEntityLoader) this.GetEntityLoader(index, batchSizes[index])).Load(id, optionalObject, session);
		}

		public static IUniqueEntityLoader CreateBatchingEntityLoader(IOuterJoinLoadable persister, int maxBatchSize,
																	 LockMode lockMode, ISessionFactoryImplementor factory,
																	 IDictionary<string, IFilter> enabledFilters)
		{
			if (maxBatchSize > 1)
			{
				int[] batchSizesToCreate = ArrayHelper.GetBatchSizes(maxBatchSize);				
				return new BatchingEntityLoader(persister, batchSizesToCreate, lockMode, factory, enabledFilters);
			}
			else
			{
				return new EntityLoader(persister, lockMode, factory, enabledFilters);
			}
		}
	}
}