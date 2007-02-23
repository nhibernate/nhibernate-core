using System;
using System.Collections;

namespace Iesi.Collections
{
	/// <summary>
	/// <p>A collection that contains no duplicate elements.  This interface models the mathematical
	/// <c>Set</c> abstraction.    
	/// The order of elements in a set is dependant on (a)the data-structure implementation, and 
	/// (b)the implementation of the various <c>Set</c> methods, and thus is not guaranteed.</p>
	/// 
	/// <p>None of the <c>Set</c> implementations in this library are guranteed to be thread-safe
	/// in any way unless wrapped in a <c>SynchronizedSet</c>.</p>
	/// 
	/// <p>The following table summarizes the binary operators that are supported by the <c>Set</c> class.</p>
	/// <list type="table">
	///		<listheader>
	///			<term>Operation</term>
	///			<term>Description</term>
	///			<term>Method</term>
	///		</listheader>
	///		<item>
	///			<term>Union (OR)</term>
	///			<term>Element included in result if it exists in either <c>A</c> OR <c>B</c>.</term>
	///			<term><c>Union()</c></term>
	///		</item>
	///		<item>
	///			<term>Intersection (AND)</term>
	///			<term>Element included in result if it exists in both <c>A</c> AND <c>B</c>.</term>
	///			<term><c>InterSect()</c></term>
	///		</item>
	///		<item>
	///			<term>Exclusive Or (XOR)</term>
	///			<term>Element included in result if it exists in one, but not both, of <c>A</c> and <c>B</c>.</term>
	///			<term><c>ExclusiveOr()</c></term>
	///		</item>
	///		<item>
	///			<term>Minus (n/a)</term>
	///			<term>Take all the elements in <c>A</c>.  Now, if any of them exist in <c>B</c>, remove
	///			them.  Note that unlike the other operators, <c>A - B</c> is not the same as <c>B - A</c>.</term>
	///			<term><c>Minus()</c></term>
	///		</item>
	/// </list>
	/// </summary>
	public interface ISet : ICollection, ICloneable
	{
		/// <summary>
		/// Performs a "union" of the two sets, where all the elements
		/// in both sets are present.  That is, the element is included if it is in either <c>a</c> or <c>b</c>.
		/// Neither this set nor the input set are modified during the operation.  The return value
		/// is a <c>Clone()</c> of this set with the extra elements added in.
		/// </summary>
		/// <param name="a">A collection of elements.</param>
		/// <returns>A new <c>Set</c> containing the union of this <c>Set</c> with the specified collection.
		/// Neither of the input objects is modified by the union.</returns>
		ISet Union(ISet a);

		/// <summary>
		/// Performs an "intersection" of the two sets, where only the elements
		/// that are present in both sets remain.  That is, the element is included if it exists in
		/// both sets.  The <c>Intersect()</c> operation does not modify the input sets.  It returns
		/// a <c>Clone()</c> of this set with the appropriate elements removed.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <returns>The intersection of this set with <c>a</c>.</returns>
		ISet Intersect(ISet a);

		/// <summary>
		/// Performs a "minus" of set <c>b</c> from set <c>a</c>.  This returns a set of all
		/// the elements in set <c>a</c>, removing the elements that are also in set <c>b</c>.
		/// The original sets are not modified during this operation.  The result set is a <c>Clone()</c>
		/// of this <c>Set</c> containing the elements from the operation.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <returns>A set containing the elements from this set with the elements in <c>a</c> removed.</returns>
		ISet Minus(ISet a);

		/// <summary>
		/// Performs an "exclusive-or" of the two sets, keeping only the elements that
		/// are in one of the sets, but not in both.  The original sets are not modified
		/// during this operation.  The result set is a <c>Clone()</c> of this set containing
		/// the elements from the exclusive-or operation.
		/// </summary>
		/// <param name="a">A set of elements.</param>
		/// <returns>A set containing the result of <c>a ^ b</c>.</returns>
		ISet ExclusiveOr(ISet a);

		/// <summary>
		/// Returns <c>true</c> if this set contains the specified element.
		/// </summary>
		/// <param name="o">The element to look for.</param>
		/// <returns><c>true</c> if this set contains the specified element, <c>false</c> otherwise.</returns>
		bool Contains(object o);

		/// <summary>
		/// Returns <c>true</c> if the set contains all the elements in the specified collection.
		/// </summary>
		/// <param name="c">A collection of objects.</param>
		/// <returns><c>true</c> if the set contains all the elements in the specified collection, <c>false</c> otherwise.</returns>
		bool ContainsAll(ICollection c);

		/// <summary>
		/// Returns <c>true</c> if this set contains no elements.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Adds the specified element to this set if it is not already present.
		/// </summary>
		/// <param name="o">The object to add to the set.</param>
		/// <returns><c>true</c> is the object was added, <c>false</c> if it was already present.</returns>
		bool Add(object o);

		/// <summary>
		/// Adds all the elements in the specified collection to the set if they are not already present.
		/// </summary>
		/// <param name="c">A collection of objects to add to the set.</param>
		/// <returns><c>true</c> is the set changed as a result of this operation, <c>false</c> if not.</returns>
		bool AddAll(ICollection c);

		/// <summary>
		/// Removes the specified element from the set.
		/// </summary>
		/// <param name="o">The element to be removed.</param>
		/// <returns><c>true</c> if the set contained the specified element, <c>false</c> otherwise.</returns>
		bool Remove(object o);

		/// <summary>
		/// Remove all the specified elements from this set, if they exist in this set.
		/// </summary>
		/// <param name="c">A collection of elements to remove.</param>
		/// <returns><c>true</c> if the set was modified as a result of this operation.</returns>
		bool RemoveAll(ICollection c);


		/// <summary>
		/// Retains only the elements in this set that are contained in the specified collection.
		/// </summary>
		/// <param name="c">Collection that defines the set of elements to be retained.</param>
		/// <returns><c>true</c> if this set changed as a result of this operation.</returns>
		bool RetainAll(ICollection c);

		/// <summary>
		/// Removes all objects from the set.
		/// </summary>
		void Clear();
	}
}