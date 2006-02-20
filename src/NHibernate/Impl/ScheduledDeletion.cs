using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Impl
{
	/// <summary>
	/// A scheduled deletion of an object.
	/// </summary>
	[Serializable]
	internal class ScheduledDeletion : ScheduledEntityAction
	{
		private object version;
		private ISoftLock lck;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledDeletion"/>.
		/// </summary>
		/// <param name="id">The identifier of the object.</param>
		/// <param name="version">The version of the object being deleted.</param>
		/// <param name="instance">The actual object instance.</param>
		/// <param name="persister">The <see cref="IEntityPersister"/> that is responsible for the persisting the object.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledDeletion( object id, object version, object instance, IEntityPersister persister, ISessionImplementor session )
			: base( session, id, instance, persister )
		{
			this.version = version;
		}

		/// <summary></summary>
		public override void Execute()
		{
			if( Persister.HasCache )
			{
				lck = Persister.Cache.Lock( Id, version );
			}
			Persister.Delete( Id, version, Instance, Session );
			Session.PostDelete( Instance );
		}

		/// <summary></summary>
		public override void AfterTransactionCompletion( bool success )
		{
			if( Persister.HasCache )
			{
				Persister.Cache.Release( Id, lck );
			}
		}
	}
}