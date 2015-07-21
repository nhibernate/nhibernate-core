using System;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="ISet{T}"/> collection
	/// to the database.
	/// </summary>
	[Serializable]
	public class GenericSetType<T> : CollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="GenericSetType{T}"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		public GenericSetType(string role, string propertyRef)
			: base(role, propertyRef, false) { }

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the set.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the set.</param>
		/// <param name="persister">The current <see cref="ICollectionPersister" /> for the set.</param>
		/// <param name="key"></param>
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister, object key)
		{
			return new PersistentGenericSet<T>(session);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(ISet<T>); }
		}

		/// <summary>
		/// Wraps an <see cref="IList{T}"/> in a <see cref="PersistentGenericSet&lt;T&gt;"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IList{T}"/>.</param>
		/// <returns>
		/// An <see cref="PersistentGenericSet&lt;T&gt;"/> that wraps the non NHibernate <see cref="IList{T}"/>.
		/// </returns>
		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			var set = collection as ISet<T>;
			if (set == null)
			{
				var stronglyTypedCollection = collection as ICollection<T>;
				if (stronglyTypedCollection == null)
					throw new HibernateException(Role + " must be an implementation of ISet<T> or ICollection<T>");
				set = new HashSet<T>(stronglyTypedCollection);
			}
			return new PersistentGenericSet<T>(session, set);
		}

		protected override void Add(object collection, object element)
		{
			((ISet<T>)collection).Add((T)element);
		}

		protected override void Clear(object collection)
		{
			((ISet<T>)collection).Clear();
		}

		public override object Instantiate(int anticipatedSize)
		{
			return new HashSet<T>();
		}
	}
}
