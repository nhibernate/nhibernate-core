using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister;

namespace NHibernate.Impl 
{
	/// <summary>
	/// A scheduled insertion of an object.
	/// </summary>
	internal class ScheduledInsertion : ScheduledEntityAction 
	{
		
		private readonly object[] _state;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledInsertion"/>.
		/// </summary>
		/// <param name="id">The identifier of the object.</param>
		/// <param name="state">An object array that contains the state of the object being inserted.</param>
		/// <param name="instance">The actual object instance.</param>
		/// <param name="persister">The <see cref="IClassPersister"/> that is responsible for the persisting the object.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledInsertion(object id, object[] state, object instance, IClassPersister persister, ISessionImplementor session) 
			: base(session, id, instance, persister) 
		{
			_state = state;
		}

		public override void Execute() 
		{
			Persister.Insert( Id, _state, Instance, Session);
			Session.PostInsert( Instance );
		}

		public override void AfterTransactionCompletion() 
		{
			// do nothing
		}
	}
}
