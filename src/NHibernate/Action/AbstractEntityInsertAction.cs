using System;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Action
{
	[Serializable]
	public abstract class AbstractEntityInsertAction : EntityAction
	{
		protected internal AbstractEntityInsertAction(
			object id,
			object[] state,
			object instance,
			IEntityPersister persister,
			ISessionImplementor session)
			: base(session, id, instance, persister)
		{
			State = state;
		}

		public object[] State { get; }
	}
}
