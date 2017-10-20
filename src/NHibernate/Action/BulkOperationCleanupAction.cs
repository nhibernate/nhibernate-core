using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;

namespace NHibernate.Action
{
	/// <summary>
	/// Implementation of BulkOperationCleanupAction.
	/// </summary>
	[Serializable]
	public partial class BulkOperationCleanupAction: IExecutable
	{
		private readonly ISessionImplementor session;
		private readonly HashSet<string> affectedEntityNames = new HashSet<string>();
		private readonly HashSet<string> affectedCollectionRoles = new HashSet<string>();
		private readonly List<string> spaces;

		public BulkOperationCleanupAction(ISessionImplementor session, IQueryable[] affectedQueryables)
		{
			this.session = session;
			List<string> tmpSpaces = new List<string>();
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
				}
			}
			spaces = new List<string>(tmpSpaces);
		}

		/// <summary>
		/// Create an action that will evict collection and entity regions based on queryspaces (table names).  
		/// </summary>
		public BulkOperationCleanupAction(ISessionImplementor session, ISet<string> querySpaces)
		{
			//from H3.2 TODO: cache the autodetected information and pass it in instead.
			this.session = session;

			ISet<string> tmpSpaces = new HashSet<string>(querySpaces);
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
					}
				}
			}
			spaces = new List<string>(tmpSpaces);
		}

		private bool AffectedEntity(ISet<string> querySpaces, string[] entitySpaces)
		{
			if (querySpaces == null || (querySpaces.Count == 0))
			{
				return true;
			}

			for (int i = 0; i < entitySpaces.Length; i++)
			{
				if (querySpaces.Contains(entitySpaces[i]))
				{
					return true;
				}
			}
			return false;
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

		public BeforeTransactionCompletionProcessDelegate BeforeTransactionCompletionProcess
		{
			get 
			{ 
				return null;
			}
		}

		public AfterTransactionCompletionProcessDelegate AfterTransactionCompletionProcess
		{
			get
			{
				return new AfterTransactionCompletionProcessDelegate((success) =>
				{
					this.EvictEntityRegions();
					this.EvictCollectionRegions();
				});
			}
		}

		private void EvictCollectionRegions()
		{
			if (affectedCollectionRoles != null)
			{
				foreach (string roleName in affectedCollectionRoles)
				{
					session.Factory.EvictCollection(roleName);
				}
			}
		}

		private void EvictEntityRegions()
		{
			if (affectedEntityNames != null)
			{
				foreach (string entityName in affectedEntityNames)
				{
					session.Factory.EvictEntity(entityName);
				}
			}
		}

		#endregion

		public virtual void Init()
		{
			EvictEntityRegions();
			EvictCollectionRegions();
		}
	}
}
