using System;
using NHibernate.JCollections;

namespace NHibernate.Odmg
{
	/// <summary>
	/// The ODMG Set collection interface.
	/// A IDSet object is an unordered collection that does not support
	/// multiple elements with the same value. An implementation typically is very
	/// efficient at determining whether the collection contains a particular value.
	/// 
	/// All of the operations defined by JCollections.ISet
	/// interface are supported by an ODMG implementation of IDSet,
	/// Author: edgar.sanchez@objeq.com
	/// Version: ODMG 3.0
	/// </summary>
	public interface IDSet : IDCollection, ISet {

		/// <summary>
		/// Create a new IDSet object that is the set union of this
		/// IDSet object and the set referenced by otherSet.
		/// </summary>
		/// <param name="otherSet">The other set to be used in the union operation.</param>
		/// <returns>A newly created IDSet instance that contains the union of the two sets.</returns>
		IDSet Union(IDSet otherSet);

		/// <summary>
		/// Create a new IDSet object that is the set intersection of this
		/// IDSet object and the set referenced by otherSet.
		/// </summary>
		/// <param name="otherSet">The other set to be used in the intersection operation.</param>
		/// <returns>A newly created IDSet instance that contains the intersection of the two sets.</returns>
		IDSet Intersection(IDSet otherSet);

		/// <summary>
		/// Create a new IDSet object that contains the elements of this
		/// collection minus the elements in otherSet.
		/// </summary>
		/// <param name="otherSet">A set containing elements that should not be in the result set.</param>
		/// <returns>A newly created IDSet instance that contains the elements
		/// of this set minus those elements in otherSet</returns>
		IDSet Difference(IDSet otherSet);

		/// <summary>
		/// Determine whether this set is a subset of the set referenced by otherSet.
		/// collection minus the elements in otherSet.
		/// </summary>
		/// <param name="otherSet">Another set.</param>
		/// <returns>True if this set is a subset of the set referenced by otherSet,
		/// otherwise false.</returns>
		bool SubsetOf(IDSet otherSet);

		/// <summary>
		/// Determine whether this set is a proper subset of the set referenced by otherSet.
		/// collection minus the elements in otherSet.
		/// </summary>
		/// <param name="otherSet">Another set.</param>
		/// <returns>True if this set is a proper subset of the set referenced by otherSet,
		/// otherwise false.</returns>
		bool ProperSubsetOf(IDSet otherSet);

		/// <summary>
		/// Determine whether this set is a superset of the set referenced by otherSet.
		/// </summary>
		/// <param name="otherSet">Another set.</param>
		/// <returns>True if this set is a superset of the set referenced by otherSet,
		/// otherwise false.</returns>
		bool SupersetOf(IDSet otherSet);

		/// <summary>
		/// Determine whether this set is a proper superset of the set referenced by otherSet.
		/// </summary>
		/// <param name="otherSet">Another set.</param>
		/// <returns>True if this set is a proper superset of the set referenced by otherSet,
		/// otherwise false.</returns>
		bool ProperSupersetOf(IDSet otherSet);
	}
}
