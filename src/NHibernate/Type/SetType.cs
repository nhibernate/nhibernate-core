using System;
using System.Collections;
using Iesi.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="Iesi.Collections.ISet"/> collection
	/// to the database.
	/// </summary>
	[Serializable]
	public class SetType : CollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="SetType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		public SetType(string role, string propertyRef)
			: base(role, propertyRef)
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the set.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the set.</param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentSet(session);
		}

		/// <summary>
		/// <see cref="AbstractType.ReturnedClass"/>
		/// </summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(ISet); }
		}

		/// <summary>
		/// Wraps an <see cref="Iesi.Collections.ISet"/> in a <see cref="PersistentSet"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="Iesi.Collections.ISet"/>.</param>
		/// <returns>
		/// An <see cref="PersistentSet"/> that wraps the non NHibernate <see cref="Iesi.Collections.ISet"/>.
		/// </returns>
		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentSet(session, (ISet) collection);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override ICollection GetElementsCollection(object collection)
		{
			return (ICollection) collection;
		}

		protected override void Add(object collection, object element)
		{
			((ISet) collection).Add(element);
		}

		protected override void Clear(object collection)
		{
			((ISet) collection).Clear();
		}

		public override object Instantiate()
		{
			return new HashedSet();
		}
	}
}