using System;
using NHibernate.JCollections;

namespace NHibernate.Odmg
{
	/// <summary>
	/// A translation of ODMG DSet
	/// </summary>
	public interface IDSet : IDCollection, ISet {
		IDSet Union(IDSet set);
		IDSet Difference(IDSet set);
		IDSet Intersection(IDSet set);
		IDSet ProperSubsetOf(IDSet set);
		IDSet ProperSupersetOf(IDSet set);
		IDSet SubsetOf(IDSet set);
		IDSet SupersetOf(IDSet set);
	}
}
