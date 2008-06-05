using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="IList"/> collection
	/// using list semantics to the database.
	/// </summary>
	[Serializable]
	public class ListType : CollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="ListType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="isEmbeddedInXML"></param>
		public ListType(string role, string propertyRef, bool isEmbeddedInXML)
			: base(role, propertyRef, isEmbeddedInXML)
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the bag.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the bag.</param>
		/// <param name="persister"></param>
		/// <param name="key"></param>
		/// <returns>A new <see cref="NHibernate.Collection.PersistentList"/>.</returns>
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister, object key)
		{
			return new PersistentList(session);
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(IList); }
		}

		/// <summary>
		/// Wraps an exist <see cref="IList"/> in a NHibernate <see cref="PersistentList"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IList"/>.</param>
		/// <returns>
		/// An <see cref="PersistentList"/> that wraps the non NHibernate <see cref="IList"/>.
		/// </returns>
		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentList(session, (IList) collection);
		}

		protected override void Add(object collection, object element)
		{
			((IList) collection).Add(element);
		}

		protected override void Clear(object collection)
		{
			((IList) collection).Clear();
		}

		public override object Instantiate(int anticipatedSize)
		{
			return anticipatedSize <= 0 ? new ArrayList() : new ArrayList(anticipatedSize + 1);
		}

		public override object IndexOf(object collection, object element)
		{
			IList list = (IList)collection;
			int i = list.IndexOf(element);
			if (i < 0) 
				return null;
			else 
				return list[i];
		}
	}
}