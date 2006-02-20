using System;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Impl
{
	/// <summary>
	/// Summary description for ScheduledIdentityInsertion.
	/// </summary>
	[Serializable]
	internal sealed class ScheduledIdentityInsertion : ScheduledEntityAction, IExecutable
	{
		private readonly object[] state;
		//private CacheEntry cacheEntry;
		private object generatedId;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		/// <param name="instance"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		public ScheduledIdentityInsertion( object[] state, object instance, IEntityPersister persister, ISessionImplementor session ) : base( session, null, instance, persister )
		{
			this.state = state;
		}

		public override void Execute()
		{
			IEntityPersister persister = Persister;
			ISessionImplementor session = Session;
			object obj = Instance;

			// Don't need to lock the cache here, since if someone
			// else inserted the same pk first, the insert would fail.

			generatedId = persister.Insert( state, obj, session );

			// TODO: This bit has to be called after all the cascades
			/*
			if ( persister.HasCache && !persister.IsCacheInvalidationRequired )
			{
				cacheEntry = new CacheEntry( obj, persister, session );
				persister.Cache.Put( generatedId, cacheEntry, 0 );
			}
			*/
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sucess"></param>
		public override void AfterTransactionCompletion( bool sucess )
		{
			// TODO: renable
			/*
			IClassPersister persister = Persister;
			if ( success && persister.HasCache && !persister.IsCacheInvalidationRequired ) 
			{
				persister.Cache.AfterInsert( GeneratedId, cacheEntry );
			}
			*/
		}

		/// <summary>
		/// 
		/// </summary>
		// TODO: Remove once AfterTransactionCompletion is active
		public override bool HasAfterTransactionCompletion
		{
			get { return false; }
		}

		public object GeneratedId
		{
			get { return generatedId; }
		}
	}
}
