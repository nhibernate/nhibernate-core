using System;
using System.Collections;

namespace NHibernate.Odmg
{
	/// <summary>
	/// The base interface for all ODMG collections.
	/// The ODMG collections are based on .NET Framework's collection interface.
	/// All of the operations defined by the .NET Framework's ICollection
	/// interface are supported by an ODMG implementation of DCollection;
	/// the exception UnsupportedOperationException is not thrown when a
	/// call is made to any of the ICollection methods.
	/// IDCollection contains methods used to perform queries on the collection.
	/// The OQL query predicate is given as a string with the syntax of the
	/// "where" clause of OQL. The predefined OQL variable "this"
	/// "is" used inside the predicate to denote the current element of the collection.
	/// Author: edgar.sanchez@objeq.com
	/// Version: ODMG 3.0
	/// </summary>
	public interface IDCollection : ICollection {
		/// <summary>
		/// Selects the single element of the collection for which the provided OQL query
		/// predicate is true.
		/// </summary>
		/// <param name="predicate">An OQL boolean query predicate.</param>
		/// <returns>The element that evaluates to true for the predicate. If no element
		/// evaluates to true, null is returned.</returns>
		/// <exception cref="NHibernate.Odmg.QueryInvalidException">The query predicate is invalid.</exception>
		object SelectElement(string predicate);

		/// <summary>
		/// Access all of the elements of the collection that evaluate to true for the
		/// provided query predicate.
		/// </summary>
		/// <param name="predicate">An OQL boolean query predicate.</param>
		/// <returns>An iterator used to iterate over the elements that evaluated true for the predicate.</returns>
		/// <exception cref="NHibernate.Odmg.QueryInvalidException">The query predicate is invalid.</exception>
		IEnumerator Select(string predicate);

		/// <summary>
		/// Evaluate the boolean query predicate for each element of the collection and
		/// return a new collection that contains each element that evaluated to true.
		/// </summary>
		/// <param name="predicate">An OQL boolean query predicate.</param>
		/// <returns>A new collection containing the elements that evaluated true for the predicate.</returns>
		/// <exception cref="NHibernate.Odmg.QueryInvalidException">The query predicate is invalid.</exception>
		IDCollection Query(string predicate);

		/// <summary>
		/// Determines whether there is an element of the collection that evaluates to true
		/// for the predicate.
		/// </summary>
		/// <param name="predicate">An OQL boolean query predicate.</param>
		/// <returns>True if there is an element of the collection that evaluates to true
		/// for the predicate, otherwise false.</returns>
		/// <exception cref="NHibernate.Odmg.QueryInvalidException">The query predicate is invalid.</exception>
		bool ExistsElement(string predicate);
	}
}
