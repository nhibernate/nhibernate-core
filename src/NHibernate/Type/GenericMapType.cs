using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="IDictionary{TKey,TValue}"/> collection
	/// to the database.
	/// </summary>
	[Serializable]
	public partial class GenericMapType<TKey, TValue> : CollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="GenericMapType{TKey, TValue}"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		public GenericMapType(string role, string propertyRef)
			: base(role, propertyRef)
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the map.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the map.</param>
		/// <param name="persister"></param>
		/// <param name="key">Not used.</param>
		/// <returns></returns>
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister, object key)
		{
			return new PersistentGenericMap<TKey, TValue>(session);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(IDictionary<TKey, TValue>); }
		}


		public override IEnumerable GetElementsIterator(object collection)
		{
			return ((IDictionary<TKey, TValue>) collection).Values;
		}

		/// <summary>
		/// Wraps an <see cref="IDictionary&lt;TKey,TValue&gt;"/> in a <see cref="PersistentGenericMap&lt;TKey,TValue&gt;"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IDictionary&lt;TKey,TValue&gt;"/>.</param>
		/// <returns>
		/// An <see cref="PersistentGenericMap&lt;TKey,TValue&gt;"/> that wraps the 
		/// non NHibernate <see cref="IDictionary&lt;TKey,TValue&gt;"/>.
		/// </returns>
		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentGenericMap<TKey, TValue>(session, (IDictionary<TKey, TValue>) collection);
		}

		protected override void Add(object collection, object element)
		{
			((IDictionary<TKey, TValue>) collection).Add((KeyValuePair<TKey, TValue>) element);
		}

		protected override void Clear(object collection)
		{
			((IDictionary) collection).Clear();
		}

		public override object ReplaceElements(object original, object target, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			var cp = session.Factory.GetCollectionPersister(Role);

			var targetPc = target as IPersistentCollection;
			var originalPc = original as IPersistentCollection;
			var iterOriginal = (IDictionary<TKey, TValue>)original;
			var clearTargetsDirtyFlag = ShouldTargetsDirtyFlagBeCleared(targetPc, originalPc, iterOriginal);

			var result = (IDictionary<TKey, TValue>)target;
			result.Clear();

			foreach (var me in iterOriginal)
			{
				var key = (TKey)cp.IndexType.Replace(me.Key, null, session, owner, copyCache);
				var value = (TValue)cp.ElementType.Replace(me.Value, null, session, owner, copyCache);
				result[key] = value;
			}

			if (clearTargetsDirtyFlag)
			{
				targetPc.ClearDirty();
			}

			return result;
		}

		public override object Instantiate(int anticipatedSize)
		{
			return anticipatedSize <= 0 ? new Dictionary<TKey, TValue>() : new Dictionary<TKey, TValue>(anticipatedSize + 1);
		}

		public override object IndexOf(object collection, object element)
		{
			var dictionary = (IDictionary<TKey, TValue>) collection;

			return dictionary
				.Where(pair => Equals(pair.Value, element))
				.Select(pair => pair.Key)
				.FirstOrDefault();
		}

		protected override bool AreCollectionElementsEqual(IEnumerable original, IEnumerable target)
		{
			var first = (IDictionary<TKey, TValue>)original;
			var second = (IDictionary<TKey, TValue>)target;
			return first.Count == second.Count && first.All(keyValue => second.Contains(keyValue));
		}
	}
}
