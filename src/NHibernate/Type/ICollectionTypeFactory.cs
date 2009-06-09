using System.Collections;
using System.Collections.Generic;
namespace NHibernate.Type
{
	/// <summary>
	/// Type factory for collections types.
	/// </summary>
	public interface ICollectionTypeFactory
	{
		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="System.Array"/>.
		/// </summary>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="elementClass">The <see cref="System.Type"/> to use to create the array.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// An <see cref="ArrayType"/> for the specified role.
		/// </returns>
		CollectionType Array(string role, string propertyRef, bool embedded, System.Type elementClass);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="IList"/>
		/// with bag semantics.
		/// </summary>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="BagType"/> for the specified role.
		/// </returns>
		CollectionType Bag(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an 
		/// <see cref="System.Collections.Generic.IList{T}"/> with bag semantics.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">
		/// The name of the property in the owner object containing the collection ID, 
		/// or <see langword="null" /> if it is the primary key.
		/// </param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="GenericBagType{T}"/> for the specified role.
		/// </returns>
		CollectionType Bag<T>(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="IList"/>.
		/// </summary>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="ListType"/> for the specified role.
		/// </returns>
		CollectionType List(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an 
		/// <see cref="System.Collections.Generic.IList&lt;T&gt;"/> with list 
		/// semantics.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">
		/// The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.
		/// </param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="ListType"/> for the specified role.
		/// </returns>
		CollectionType List<T>(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="IList"/>
		/// with id-bag semantics.
		/// </summary>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="IdentifierBagType"/> for the specified role.
		/// </returns>
		CollectionType IdBag(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an 
		/// <see cref="System.Collections.Generic.IList{T}"/> with identifier
		/// bag semantics.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.
		/// </param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="GenericIdentifierBagType{T}"/> for the specified role.
		/// </returns>
		CollectionType IdBag<T>(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="Iesi.Collections.ISet"/>.
		/// </summary>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="SetType"/> for the specified role.
		/// </returns>
		CollectionType Set(string role, string propertyRef, bool embedded);

		CollectionType OrderedSet(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="Iesi.Collections.ISet"/>
		/// that is sorted by an <see cref="IComparer"/>.
		/// </summary>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="comparer">The <see cref="IComparer"/> that does the sorting.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="SortedSetType"/> for the specified role.
		/// </returns>
		CollectionType SortedSet(string role, string propertyRef, bool embedded, IComparer comparer);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="Iesi.Collections.Generic.ISet{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>A <see cref="GenericSetType{T}" /> for the specified role.</returns>
		CollectionType Set<T>(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for a sorted <see cref="Iesi.Collections.Generic.ISet{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}" /> to use for the set.</param>
		/// <returns>A <see cref="GenericSetType{T}" /> for the specified role.</returns>
		CollectionType SortedSet<T>(string role, string propertyRef, bool embedded, IComparer<T> comparer);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an ordered <see cref="Iesi.Collections.Generic.ISet{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">
		/// The name of the property in the owner object containing the collection ID, 
		/// or <see langword="null" /> if it is the primary key.
		/// </param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>A <see cref="GenericSetType{T}" /> for the specified role.</returns>
		CollectionType OrderedSet<T>(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="IDictionary"/>.
		/// </summary>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">
		/// The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="MapType"/> for the specified role.
		/// </returns>
		CollectionType Map(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="IDictionary"/>
		/// that maintains insertion order of elements.
		/// </summary>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="OrderedMapType"/> for the specified role.
		/// </returns>
		CollectionType OrderedMap(string role, string propertyRef, bool embedded);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="IDictionary"/>
		/// that is sorted by an <see cref="IComparer"/>.
		/// </summary>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="comparer">The <see cref="IComparer"/> that does the sorting.</param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="SortedMapType"/> for the specified role.
		/// </returns>
		CollectionType SortedMap(string role, string propertyRef, bool embedded, IComparer comparer);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an 
		/// <see cref="System.Collections.Generic.IDictionary&lt;TKey,TValue&gt;"/>.
		/// </summary>
		/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
		/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.<
		/// /param>
		/// <param name="embedded">Is embedded in XML (not supported yet)</param>
		/// <returns>
		/// A <see cref="MapType"/> for the specified role.
		/// </returns>
		CollectionType Map<TKey, TValue>(string role, string propertyRef, bool embedded);


		CollectionType SortedDictionary<TKey, TValue>(string role, string propertyRef, bool embedded, IComparer<TKey> comparer);
		CollectionType SortedList<TKey, TValue>(string role, string propertyRef, bool embedded, IComparer<TKey> comparer);
	}
}