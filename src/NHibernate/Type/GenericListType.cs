using System;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="IList{T}"/> collection
	/// to the database using list semantics.
	/// </summary>
	[Serializable]
	public class GenericListType<T> : CollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="GenericListType{T}"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		public GenericListType(string role, string propertyRef)
			: base(role, propertyRef, false)
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the list.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the list.</param>
		/// <param name="persister">The current <see cref="ICollectionPersister" /> for the list.</param>
		/// <param name="key"></param>
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister, object key)
		{
			return new PersistentGenericList<T>(session);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(IList<T>); }
		}

		/// <summary>
		/// Wraps an <see cref="IList{T}"/> in a <see cref="PersistentGenericList{T}"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IList{T}"/>.</param>
		/// <returns>
		/// An <see cref="PersistentGenericList{T}"/> that wraps the non NHibernate <see cref="IList{T}"/>.
		/// </returns>
		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentGenericList<T>(session, (IList<T>) collection);
		}

		protected override void Add(object collection, object element)
		{
			((IList<T>) collection).Add((T) element);
		}

		protected override void Clear(object collection)
		{
			((IList<T>) collection).Clear();
		}

		//TODO: Add() & Clear() methods - need to see if these should be refactored back into
		// their own version of Copy or a DoCopy.  The Copy() method used to be spread out amongst
		// the various collections, but since they all had common code Add() and Clear() were made
		// virtual since that was where most of the logic was.  A different/better way might be to
		// have a Copy on the base collection that handles the standard checks and then a DoCopy
		// that performs the actual copy.

		public override object Instantiate(int anticipatedSize)
		{
			return anticipatedSize <= 0 ? new List<T>() : new List<T>(anticipatedSize + 1);
		}

		public override object IndexOf(object collection, object element)
		{
			var list = (IList<T>)collection;
			int i = list.IndexOf((T) element);
			if (i < 0)
				return null;
			return i;
		}
	}
}
