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
	public partial class BulkOperationCleanupAction :
		IAsyncExecutable,
		IAfterTransactionCompletionProcess,
		ICacheableExecutable
	{
		private readonly ISessionFactoryImplementor _factory;
		private readonly HashSet<string> affectedEntityNames;
		private readonly HashSet<string> affectedCollectionRoles;
		private readonly string[] spaces;
		private readonly bool _hasCache;

		public BulkOperationCleanupAction(ISessionImplementor session, IQueryable[] affectedQueryables)
		{
			_factory = session.Factory;
			var tmpSpaces = new HashSet<string>();
			foreach (var queryables in affectedQueryables)
			{
				if (queryables.HasCache)
				{
					_hasCache = true;
					if (affectedEntityNames == null)
					{
						affectedEntityNames = new HashSet<string>();
					}

					affectedEntityNames.Add(queryables.EntityName);
				}

				var roles = _factory.GetCollectionRolesByEntityParticipant(queryables.EntityName);
				if (roles != null)
				{
					if (affectedCollectionRoles == null)
					{
						affectedCollectionRoles = new HashSet<string>();
					}

					affectedCollectionRoles.UnionWith(roles);
				}

				tmpSpaces.UnionWith(queryables.QuerySpaces);
			}

			spaces = tmpSpaces.ToArray();
		}

		/// <summary>
		/// Create an action that will evict collection and entity regions based on queryspaces (table names).  
		/// </summary>
		public BulkOperationCleanupAction(ISessionImplementor session, ISet<string> querySpaces)
		{
			//from H3.2 TODO: cache the autodetected information and pass it in instead.
			_factory = session.Factory;

			var tmpSpaces = new HashSet<string>(querySpaces);
			var acmd = _factory.GetAllClassMetadata();
			foreach (KeyValuePair<string, IClassMetadata> entry in acmd)
			{
				var entityName = entry.Key;
				var persister = _factory.GetEntityPersister(entityName);
				var entitySpaces = persister.QuerySpaces;

				if (AffectedEntity(querySpaces, entitySpaces))
				{
					if (persister.HasCache)
					{
						_hasCache = true;
						if (affectedEntityNames == null)
						{
							affectedEntityNames = new HashSet<string>();
						}

						affectedEntityNames.Add(persister.EntityName);
					}

					var roles = session.Factory.GetCollectionRolesByEntityParticipant(persister.EntityName);
					if (roles != null)
					{
						if (affectedCollectionRoles == null)
						{
							affectedCollectionRoles = new HashSet<string>();
						}

						affectedCollectionRoles.UnionWith(roles);
					}

					tmpSpaces.UnionWith(entitySpaces);
				}
			}
			spaces = tmpSpaces.ToArray();
		}

		private bool AffectedEntity(ISet<string> querySpaces, string[] entitySpaces)
		{
			if (querySpaces == null || (querySpaces.Count == 0))
			{
				return true;
			}

			return entitySpaces.Any(querySpaces.Contains);
		}

		public bool HasCache => _hasCache;

		#region IExecutable Members

		public string[] PropertySpaces => spaces;

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
			if (affectedCollectionRoles != null)
			{
				_factory.EvictCollection(affectedCollectionRoles);
			}
		}

		private void EvictEntityRegions()
		{
			if (affectedEntityNames != null)
			{
				_factory.EvictEntity(affectedEntityNames);
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
