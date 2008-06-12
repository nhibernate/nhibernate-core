using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="IDictionary"/> collection
	/// to the database.
	/// </summary>
	[Serializable]
	public class MapType : CollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="MapType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		/// <param name="isEmbeddedInXML"></param>
		public MapType(string role, string propertyRef, bool isEmbeddedInXML)
			: base(role, propertyRef, isEmbeddedInXML)
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the map.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the map.</param>
		/// <param name="persister"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister, object key)
		{
			return new PersistentMap(session);
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof(IDictionary); }
		}

		public override IEnumerable GetElementsIterator(object collection)
		{
			return ((IDictionary)collection).Values;
		}

		/// <summary>
		/// Wraps an <see cref="IDictionary"/> in a <see cref="PersistentMap"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IDictionary"/>.</param>
		/// <returns>
		/// An <see cref="PersistentMap"/> that wraps the non NHibernate <see cref="IDictionary"/>.
		/// </returns>
		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentMap(session, (IDictionary) collection);
		}

		protected override void Add(object collection, object element)
		{
			DictionaryEntry de = (DictionaryEntry) element;
			((IDictionary) collection).Add(de.Key, de.Value);
		}

		protected override void Clear(object collection)
		{
			((IDictionary) collection).Clear();
		}

		public override object ReplaceElements(object original, object target, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			ICollectionPersister cp = session.Factory.GetCollectionPersister(Role);

			IDictionary result = (IDictionary)target;
			result.Clear();

			IEnumerable iter = (IDictionary)original;
			foreach (DictionaryEntry me in iter)
			{
				object key = cp.IndexType.Replace(me.Key, null, session, owner, copyCache);
				object value = cp.ElementType.Replace(me.Value, null, session, owner, copyCache);
				result[key] = value;
			}

			return result;
		}

		public override object Instantiate(int anticipatedSize)
		{
			return new Hashtable();
		}

		public override object IndexOf(object collection, object element)
		{
			IEnumerable iter = (IDictionary)collection;
			foreach (DictionaryEntry me in iter)
			{
				if (me.Value == element)
					return me.Key;				
			}
			return null;
		}
	}
}