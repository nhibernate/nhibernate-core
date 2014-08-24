using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1136
{
	public class MilestoneCollection<TKey, TValue> : SortedDictionary<TKey, TValue>, IMilestoneCollection<TKey, TValue>
		where TKey : IComparable<TKey>
	{
		public MilestoneCollection() : base(new ReverseSortComparer<TKey>()){}

		#region IMilestoneCollection<TKey,TValue> Members

		public TValue FindValueFor(TKey key)
		{
			foreach (TKey milestone in this.Keys)
			{
				if (milestone.CompareTo(key) <= 0) return this[milestone];
			}
			return default(TValue);
		}

		#endregion
	}
}
