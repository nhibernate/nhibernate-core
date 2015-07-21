using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1136
{
	public interface IMilestoneCollection<TKey, TValue> : IDictionary<TKey, TValue>
		where TKey : IComparable<TKey>
	{
		TValue FindValueFor(TKey key);
	}
}
