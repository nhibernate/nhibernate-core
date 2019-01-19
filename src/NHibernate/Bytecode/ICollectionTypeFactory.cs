using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.Bytecode
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
		/// <returns>
		/// An <see cref="ArrayType"/> for the specified role.
		/// </returns>
		[Obsolete("Use Array method with propertyName, entityName, isNullable")]
		CollectionType Array(string role, string propertyRef, System.Type elementClass);
		
		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="System.Array"/>.
		/// </summary>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="elementClass">The <see cref="System.Type"/> to use to create the array.</param>
		/// <param name="entityName">Entity Name</param>
		/// <param name="propertyName">Name of Property that hold this collection</param>
		/// <param name="isNullable">To check is value can be null</param>
		/// <returns>
		/// An <see cref="ArrayType"/> for the specified role.
		/// </returns>
		CollectionType Array(string role, string propertyRef, System.Type elementClass, string entityName, string propertyName, bool isNullable);

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
		/// <returns>
		/// A <see cref="GenericBagType{T}"/> for the specified role.
		/// </returns>
		[Obsolete("Use Bag method with propertyName, entityName, isNullable")]
		CollectionType Bag<T>(string role, string propertyRef);
		
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
		/// <param name="entityName">Entity Name</param>
		/// <param name="propertyName">Name of Property that hold this collection</param>
		/// <param name="isNullable">To check is value can be null</param>
		/// <returns>
		/// A <see cref="GenericBagType{T}"/> for the specified role.
		/// </returns>
		CollectionType Bag<T>(string role, string propertyRef, string entityName, string propertyName, bool isNullable);

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
		/// <returns>
		/// A <see cref="GenericListType&lt;T&gt;"/> for the specified role.
		/// </returns>
		[Obsolete("Use List method with propertyName, entityName, isNullable")]
		CollectionType List<T>(string role, string propertyRef);
		
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
		/// <param name="entityName">Entity Name</param>
		/// <param name="propertyName">Name of Property that hold this collection</param>
		/// <param name="isNullable">To check is value can be null</param>
		/// <returns>
		/// A <see cref="GenericListType&lt;T&gt;"/> for the specified role.
		/// </returns>
		CollectionType List<T>(string role, string propertyRef, string entityName, string propertyName, bool isNullable);

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
		/// <returns>
		/// A <see cref="GenericIdentifierBagType{T}"/> for the specified role.
		/// </returns>
		[Obsolete("Use IdBag method with propertyName, entityName, isNullable")]
		CollectionType IdBag<T>(string role, string propertyRef);
		
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
		/// <param name="entityName">Entity Name</param>
		/// <param name="propertyName">Name of Property that hold this collection</param>
		/// <param name="isNullable">To check is value can be null</param>
		/// <returns>
		/// A <see cref="GenericIdentifierBagType{T}"/> for the specified role.
		/// </returns>
		CollectionType IdBag<T>(string role, string propertyRef, string entityName, string propertyName, bool isNullable);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="ISet{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <returns>A <see cref="GenericSetType{T}" /> for the specified role.</returns>
		[Obsolete("Use Set method with propertyName, entityName, isNullable")]
		CollectionType Set<T>(string role, string propertyRef);
		
		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an <see cref="ISet{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="entityName">Entity Name</param>
		/// <param name="propertyName">Name of Property that hold this collection</param>
		/// <param name="isNullable">To check is value can be null</param>
		/// <returns>A <see cref="GenericSetType{T}" /> for the specified role.</returns>
		CollectionType Set<T>(string role, string propertyRef, string entityName, string propertyName, bool isNullable);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for a sorted <see cref="ISet{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}" /> to use for the set.</param>
		/// <returns>A <see cref="GenericSetType{T}" /> for the specified role.</returns>
		[Obsolete("Use SortedSet method with propertyName, entityName, isNullable")]
		CollectionType SortedSet<T>(string role, string propertyRef, IComparer<T> comparer);
		
		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for a sorted <see cref="ISet{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}" /> to use for the set.</param>
		/// <param name="entityName">Entity Name</param>
		/// <param name="propertyName">Name of Property that hold this collection</param>
		/// <param name="isNullable">To check is value can be null</param>
		/// <returns>A <see cref="GenericSetType{T}" /> for the specified role.</returns>
		CollectionType SortedSet<T>(string role, string propertyRef, IComparer<T> comparer, string entityName, string propertyName, bool isNullable);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an ordered <see cref="ISet{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">
		/// The name of the property in the owner object containing the collection ID, 
		/// or <see langword="null" /> if it is the primary key.
		/// </param>
		/// <returns>A <see cref="GenericSetType{T}" /> for the specified role.</returns>
		[Obsolete("Use OrderedSet method with propertyName, entityName, isNullable")]
		CollectionType OrderedSet<T>(string role, string propertyRef);
		
		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an ordered <see cref="ISet{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">
		/// The name of the property in the owner object containing the collection ID, 
		/// or <see langword="null" /> if it is the primary key.
		/// </param>
		/// <param name="entityName">Entity Name</param>
		/// <param name="propertyName">Name of Property that hold this collection</param>
		/// <param name="isNullable">To check is value can be null</param>
		/// <returns>A <see cref="GenericSetType{T}" /> for the specified role.</returns>
		CollectionType OrderedSet<T>(string role, string propertyRef, string entityName, string propertyName, bool isNullable);

		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an 
		/// <see cref="System.Collections.Generic.IDictionary&lt;TKey,TValue&gt;"/>.
		/// </summary>
		/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
		/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.
		/// </param>
		/// <returns>
		/// A <see cref="GenericMapType{TKey, TValue}"/> for the specified role.
		/// </returns>
		[Obsolete("Use Map method with propertyName, entityName, isNullable")]
		CollectionType Map<TKey, TValue>(string role, string propertyRef);
		
		/// <summary>
		/// Creates a new <see cref="CollectionType"/> for an 
		/// <see cref="System.Collections.Generic.IDictionary&lt;TKey,TValue&gt;"/>.
		/// </summary>
		/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
		/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
		/// <param name="role">The role the collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.
		/// </param>
		/// <param name="entityName">Entity Name</param>
		/// <param name="propertyName">Name of Property that hold this collection</param>
		/// <param name="isNullable">To check is value can be null</param>
		/// <returns>
		/// A <see cref="GenericMapType{TKey, TValue}"/> for the specified role.
		/// </returns>
		CollectionType Map<TKey, TValue>(string role, string propertyRef, string entityName, string propertyName, bool isNullable);

		[Obsolete("Use SortedDictionary method with propertyName, entityName, isNullable")]
		CollectionType SortedDictionary<TKey, TValue>(string role, string propertyRef, IComparer<TKey> comparer);
		
		CollectionType SortedDictionary<TKey, TValue>(string role, string propertyRef, IComparer<TKey> comparer, string entityName, string propertyName, bool isNullable);


		[Obsolete("Use SortedList method with propertyName, entityName, isNullable")]
		CollectionType SortedList<TKey, TValue>(string role, string propertyRef, IComparer<TKey> comparer);
		CollectionType SortedList<TKey, TValue>(string role, string propertyRef, IComparer<TKey> comparer, string entityName, string propertyName, bool isNullable);
	}
}