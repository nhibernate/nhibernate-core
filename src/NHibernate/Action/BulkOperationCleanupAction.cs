using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Engine;
using NHibernate.Persister;
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
		private readonly string[] spaces;
		private readonly string[] queryCacheSpaces;

		public BulkOperationCleanupAction(ISessionImplementor session, IQueryable[] affectedQueryables)
		{
			this.session = session;
			ISet<string> affectedSpaces = new HashSet<string>();
			ISet<string> affectedQueryCacheSpaces = new HashSet<string>();
			foreach (var affectedQueryable in affectedQueryables)
			{
				if (affectedQueryable.HasCache)
				{
					affectedEntityNames.Add(affectedQueryable.EntityName);
				}
				ISet<string> roles = session.Factory.GetCollectionRolesByEntityParticipant(affectedQueryable.EntityName);
				if (roles != null)
				{
					affectedCollectionRoles.UnionWith(roles);
				}

				foreach (var querySpace in affectedQueryable.QuerySpaces)
				{
					affectedSpaces.Add(querySpace);
					if ((affectedQueryable as IPersister)?.SupportsQueryCache != false)
					{
						affectedQueryCacheSpaces.Add(querySpace);
					}
				}
			}

			spaces = affectedSpaces.ToArray();
			queryCacheSpaces = affectedQueryCacheSpaces.ToArray();
		}

		/// <summary>
		/// Create an action that will evict collection and entity regions based on queryspaces (table names).  
		/// </summary>
		public BulkOperationCleanupAction(ISessionImplementor session, ISet<string> querySpaces)
		{
			//from H3.2 TODO: cache the autodetected information and pass it in instead.
			this.session = session;

			ISet<string> affectedSpaces = new HashSet<string>(querySpaces);
			ISet<string> affectedQueryCacheSpaces = new HashSet<string>();

			foreach (var persister in session.Factory.GetEntityPersisters(querySpaces))
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

				foreach (var querySpace in persister.QuerySpaces)
				{
					affectedSpaces.Add(querySpace);
					if ((persister as IPersister)?.SupportsQueryCache != false)
					{
						affectedQueryCacheSpaces.Add(querySpace);
					}
				}
			}

			spaces = affectedSpaces.ToArray();
			queryCacheSpaces = affectedQueryCacheSpaces.ToArray();
		}

		#region IExecutable Members

		/// <inheritdoc />
		public string[] QueryCacheSpaces => queryCacheSpaces;

		public string[] PropertySpaces
		{
			get { return spaces; }
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
