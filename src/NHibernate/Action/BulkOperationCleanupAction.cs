using System;
using Iesi.Collections;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Action
{
	/// <summary>
	/// Implementation of BulkOperationCleanupAction.
	/// </summary>
	[Serializable]
	public class BulkOperationCleanupAction: IExecutable
	{
		private readonly ISessionImplementor session;
		private readonly HashedSet<string> affectedEntityNames= new HashedSet<string>();
		private readonly HashedSet affectedCollectionRoles= new HashedSet();
		private readonly ArrayList spaces;

		public BulkOperationCleanupAction(ISessionImplementor session, IQueryable[] affectedQueryables)
		{
			this.session = session;
			ArrayList tmpSpaces = new ArrayList();
			for (int i = 0; i < affectedQueryables.Length; i++)
			{
				if (affectedQueryables[i].HasCache)
				{
					affectedEntityNames.Add(affectedQueryables[i].EntityName);
				}
				ISet roles = session.Factory.GetCollectionRolesByEntityParticipant(affectedQueryables[i].EntityName);
				if (roles != null)
				{
					affectedCollectionRoles.AddAll(roles);
				}
				for (int y = 0; y < affectedQueryables[i].QuerySpaces.Length; y++)
				{
					tmpSpaces.Add(affectedQueryables[i].QuerySpaces[y]);
				}
			}
			spaces = new ArrayList(tmpSpaces);
		}

		/// <summary>
		/// Create an action that will evict collection and entity regions based on queryspaces (table names).  
		/// </summary>
		public BulkOperationCleanupAction(ISessionImplementor session, ISet querySpaces)
		{
			//from H3.2 TODO: cache the autodetected information and pass it in instead.
			this.session = session;

			ISet tmpSpaces = new HashedSet(querySpaces);
			ISessionFactoryImplementor factory = session.Factory;
			IDictionary acmd = factory.GetAllClassMetadata();
			foreach (DictionaryEntry entry in acmd)
			{
				string entityName = (string) entry.Key;
				IEntityPersister persister = factory.GetEntityPersister(entityName);
				object[] entitySpaces = persister.QuerySpaces;

				if (AffectedEntity(querySpaces, entitySpaces))
				{
					if (persister.HasCache)
					{
						affectedEntityNames.Add(persister.EntityName);
					}
					ISet roles = session.Factory.GetCollectionRolesByEntityParticipant(persister.EntityName);
					if (roles != null)
					{
						affectedCollectionRoles.AddAll(roles);
					}
					for (int y = 0; y < entitySpaces.Length; y++)
					{
						tmpSpaces.Add(entitySpaces[y]);
					}
				}
			}
			spaces = new ArrayList(tmpSpaces);
		}

		private bool AffectedEntity(ISet querySpaces, object[] entitySpaces)
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

		public object[] PropertySpaces
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

		public bool HasAfterTransactionCompletion()
		{
			return true;
		}

		public void AfterTransactionCompletion(bool success)
		{
			EvictEntityRegions();
			EvictCollectionRegions();
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
			if(affectedEntityNames!=null)
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