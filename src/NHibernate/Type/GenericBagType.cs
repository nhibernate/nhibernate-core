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
	/// to the database using bag semantics.
	/// </summary>
	[Serializable]
	public class GenericBagType<T> : CollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="GenericBagType{T}"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		public GenericBagType(string role, string propertyRef)
			: base(role, propertyRef, false)
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the bag.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the bag.</param>
		/// <param name="persister">The current <see cref="ICollectionPersister" /> for the bag.</param>
		/// <param name="key"></param>
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister, object key)
		{
			return new PersistentGenericBag<T>(session);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(ICollection<T>); }
		}

		/// <summary>
		/// Wraps an <see cref="IList{T}"/> in a <see cref="PersistentGenericBag{T}"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IList&lt;T&gt;"/>.</param>
		/// <returns>
		/// An <see cref="PersistentGenericBag&lt;T&gt;"/> that wraps the non NHibernate <see cref="IList&lt;T&gt;"/>.
		/// </returns>
		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentGenericBag<T>(session, (IEnumerable<T>) collection);
		}

		protected override void Add(object collection, object element)
		{
			((ICollection<T>) collection).Add((T) element);
		}

		protected override void Clear(object collection)
		{
			((ICollection<T>)collection).Clear();
		}

		public override object Instantiate(int anticipatedSize)
		{
			return anticipatedSize <= 0 ? new List<T>() : new List<T>(anticipatedSize + 1);
		}
	}
}
