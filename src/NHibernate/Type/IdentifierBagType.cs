using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="IList"/> collection
	/// using bag semantics with an identifier to the database.
	/// </summary>
	[Serializable]
	public class IdentifierBagType : CollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="IdentifierBagType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="isEmbeddedInXML"></param>
		public IdentifierBagType(string role, string propertyRef, bool isEmbeddedInXML)
			: base(role, propertyRef, isEmbeddedInXML) { }

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the identifier bag.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the identifier bag.</param>
		/// <param name="persister"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister, object key)
		{
			return new PersistentIdentifierBag(session);
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(ICollection); }
		}

		/// <summary>
		/// Wraps an <see cref="IList"/> in a <see cref="PersistentIdentifierBag"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IList"/>.</param>
		/// <returns>
		/// An <see cref="PersistentIdentifierBag"/> that wraps the non NHibernate <see cref="IList"/>.
		/// </returns>
		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentIdentifierBag(session, (ICollection) collection);
		}

		protected override void Clear(object collection)
		{
			((IList) collection).Clear();
		}

		protected override void Add(object collection, object element)
		{
			((IList) collection).Add(element);
		}

		public override object Instantiate(int anticipatedSize)
		{
			return anticipatedSize <= 0 ? new ArrayList() : new ArrayList(anticipatedSize + 1);
		}
	}
}