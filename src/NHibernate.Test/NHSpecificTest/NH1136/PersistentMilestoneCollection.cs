using System;
using NHibernate.Collection.Generic;
using NHibernate.Engine;

namespace NHibernate.Test.NHSpecificTest.NH1136
{
	public class PersistentMilestoneCollection<TKey, TValue> : PersistentGenericMap<TKey, TValue>, IMilestoneCollection<TKey, TValue>
		where TKey : IComparable<TKey>
	{
		public PersistentMilestoneCollection(ISessionImplementor session, IMilestoneCollection<TKey, TValue> map) : base(session, map)
		{
		}

		public PersistentMilestoneCollection(ISessionImplementor session) : base(session)
		{
		}

		#region IMilestoneCollection<TKey,TValue> Members

		public TValue FindValueFor(TKey key)
		{
			Read();
			return ((IMilestoneCollection<TKey, TValue>) WrappedMap).FindValueFor(key);
		}

		#endregion
	}
}