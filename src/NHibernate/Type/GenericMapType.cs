#if NET_2_0

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that maps an <see cref="IDictionary&lt;TKey,Tvalue&gt;"/> collection
	/// to the database.
	/// </summary>
	public class GenericMapType<TKey, TValue> : MapType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="GenericMapType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		public GenericMapType( string role, string propertyRef )
			: base( role, propertyRef )
		{
		}

		/// <summary>
		/// Instantiates a new <see cref="IPersistentCollection"/> for the map.
		/// </summary>
		/// <param name="session">The current <see cref="ISessionImplementor"/> for the map.</param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public override IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			return new PersistentGenericMap<TKey, TValue>( session );
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( IDictionary<TKey, TValue> ); }
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
		public override IPersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new PersistentGenericMap<TKey, TValue>( session, ( IDictionary<TKey, TValue> ) collection );
		}

		//TODO: Add() & Clear() methods - need to see if these should be refactored back into
		// their own version of Copy or a DoCopy.  The Copy() method used to be spread out amongst
		// the various collections, but since they all had common code Add() and Clear() were made
		// virtual since that was where most of the logic was.  A different/better way might be to
		// have a Copy on the base collection that handles the standard checks and then a DoCopy
		// that performs the actual copy.

		public override object Instantiate()
		{
			return new Dictionary<TKey, TValue>();
		}
	}
}
#endif
