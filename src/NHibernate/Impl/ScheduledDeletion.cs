using NHibernate.Engine;
using NHibernate.Persister;

namespace NHibernate.Impl
{
	/// <summary>
	/// A scheduled deletion of an object.
	/// </summary>
	internal class ScheduledDeletion : ScheduledEntityAction
	{
		private object _version;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledDeletion"/>.
		/// </summary>
		/// <param name="id">The identifier of the object.</param>
		/// <param name="version">The version of the object being deleted.</param>
		/// <param name="instance">The actual object instance.</param>
		/// <param name="persister">The <see cref="IClassPersister"/> that is responsible for the persisting the object.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledDeletion( object id, object version, object instance, IClassPersister persister, ISessionImplementor session )
			: base( session, id, instance, persister )
		{
			_version = version;
		}

		/// <summary></summary>
		public override void Execute()
		{
			if( Persister.HasCache )
			{
				Persister.Cache.Lock( Id );
			}
			Persister.Delete( Id, _version, Instance, Session );
			Session.PostDelete( Instance );
		}

		/// <summary></summary>
		public override void AfterTransactionCompletion()
		{
			if( Persister.HasCache )
			{
				Persister.Cache.Release( Id );
			}
		}
	}
}