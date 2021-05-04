using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Action
{
	/// <summary>
	/// Implementation of BulkOperationCleanupAction.
	/// </summary>
	[Serializable]
	public partial class BulkOperationCleanupAction : IAsyncExecutable, IAfterTransactionCompletionProcess, ICacheableExecutable
	{
		private readonly ISessionImplementor session;
		private readonly HashSet<string> affectedEntityNames = new HashSet<string>();
		private readonly HashSet<string> affectedCollectionRoles = new HashSet<string>();
		private readonly List<string> spaces;
		private readonly string[] queryCacheSpaces;

		public string[] QueryCacheSpaces { get { return queryCacheSpaces; } }

		public BulkOperationCleanupAction(ISessionImplementor session, IQueryable[] affectedQueryables)
		{
			this.session = session;
			List<string> tmpSpaces = new List<string>();
			ISet<string> updQueryCacheSpaces = new HashSet<string>();
			for (int i = 0; i < affectedQueryables.Length; i++)
			{
				if (affectedQueryables[i].HasCache)
				{
					affectedEntityNames.Add(affectedQueryables[i].EntityName);
				}
				ISet<string> roles = session.Factory.GetCollectionRolesByEntityParticipant(affectedQueryables[i].EntityName);
				if (roles != null)
				{
					affectedCollectionRoles.UnionWith(roles);
				}
				for (int y = 0; y < affectedQueryables[i].QuerySpaces.Length; y++)
				{
					tmpSpaces.Add(affectedQueryables[i].QuerySpaces[y]);
					if(affectedQueryables[i] is ICacheableEntityPersister cacheablePersister)
					{
						if (cacheablePersister.SupportsQueryCache)
						{
							updQueryCacheSpaces.Add(affectedQueryables[i].QuerySpaces[y]);
						}
					}
					else
					{
						updQueryCacheSpaces.Add(affectedQueryables[i].QuerySpaces[y]);
					}
				}
			}
			spaces = new List<string>(tmpSpaces);
			queryCacheSpaces = updQueryCacheSpaces.ToArray();
		}

		/// <summary>
		/// Create an action that will evict collection and entity regions based on queryspaces (table names).  
		/// </summary>
		public BulkOperationCleanupAction(ISessionImplementor session, ISet<string> querySpaces)
		{
			//from H3.2 TODO: cache the autodetected information and pass it in instead.
			this.session = session;

			ISet<string> tmpSpaces = new HashSet<string>(querySpaces);
			ISet<string> updQueryCacheSpaces = new HashSet<string>();
			ISessionFactoryImplementor factory = session.Factory;
			IDictionary<string, IClassMetadata> acmd = factory.GetAllClassMetadata();
			foreach (KeyValuePair<string, IClassMetadata> entry in acmd)
			{
				string entityName = entry.Key;
				IEntityPersister persister = factory.GetEntityPersister(entityName);
				string[] entitySpaces = persister.QuerySpaces;

				if (AffectedEntity(querySpaces, entitySpaces))
				{
					if (persister.HasCache)
					{
						affectedEntityNames.Add(persister.EntityName);
					}
					ISet<string> roles = session.Factory.GetCollectionRolesByEntityParticipant(persister.EntityName);
					if (roles != null)
					{
						affectedCollectionRoles.UnionWith(roles);
					}
					for (int y = 0; y < entitySpaces.Length; y++)
					{
						tmpSpaces.Add(entitySpaces[y]);
						if (persister is ICacheableEntityPersister cacheablePersister)
						{
							if (cacheablePersister.SupportsQueryCache)
							{
								updQueryCacheSpaces.Add(entitySpaces[y]);
							}
						}
						else
						{
							updQueryCacheSpaces.Add(entitySpaces[y]);
						}
					}
				}
			}
			spaces = new List<string>(tmpSpaces);
			queryCacheSpaces = updQueryCacheSpaces.ToArray();
		}

		private bool AffectedEntity(ISet<string> querySpaces, string[] entitySpaces)
		{
			if (querySpaces == null || (querySpaces.Count == 0))
			{
				return true;
			}

			return entitySpaces.Any(querySpaces.Contains);
		}

		#region IExecutable Members

		public string[] PropertySpaces
		{
			get { return spaces.ToArray(); }
		}

		public void BeforeExecutions()
		{
			// nothing to do
		}

		public void Execute()
		{
			// nothing to do
		}

		//Since v5.2
		[Obsolete("This property is not used and will be removed in a future version.")]
		public BeforeTransactionCompletionProcessDelegate BeforeTransactionCompletionProcess =>
			null;

		//Since v5.2
		[Obsolete("This property is not used and will be removed in a future version.")]
		public AfterTransactionCompletionProcessDelegate AfterTransactionCompletionProcess =>
			ExecuteAfterTransactionCompletion;

		IBeforeTransactionCompletionProcess IAsyncExecutable.BeforeTransactionCompletionProcess =>
			null;

		IAfterTransactionCompletionProcess IAsyncExecutable.AfterTransactionCompletionProcess => 
			this;

		public void ExecuteAfterTransactionCompletion(bool success)
		{
			EvictEntityRegions();
			EvictCollectionRegions();
		}

		private void EvictCollectionRegions()
		{
			if (affectedCollectionRoles != null && affectedCollectionRoles.Any())
			{
				session.Factory.EvictCollection(affectedCollectionRoles);
			}
		}

		private void EvictEntityRegions()
		{
			if (affectedEntityNames != null && affectedEntityNames.Any())
			{
				session.Factory.EvictEntity(affectedEntityNames);
			}
		}

		#endregion

		// Since v5.2
		[Obsolete("This method has no more usage in NHibernate and will be removed in a future version.")]
		public virtual void Init()
		{
			EvictEntityRegions();
			EvictCollectionRegions();
		}

		// Since v5.2
		[Obsolete("This method has no more usage in NHibernate and will be removed in a future version.")]
		public virtual async Task InitAsync(CancellationToken cancellationToken)
		{
			await EvictEntityRegionsAsync(cancellationToken);
			await EvictCollectionRegionsAsync(cancellationToken);
		}
	}
}
