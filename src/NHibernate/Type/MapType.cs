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
	public class MapType : CollectionType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="MapType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		public MapType( string role, string propertyRef )
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
			return new PersistentMap( session );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( IDictionary ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override ICollection GetElementsCollection( object collection )
		{
			return ( ( IDictionary ) collection ).Values;
		}

		/// <summary>
		/// Wraps an <see cref="IDictionary"/> in a <see cref="PersistentMap"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped <see cref="IDictionary"/>.</param>
		/// <returns>
		/// An <see cref="PersistentMap"/> that wraps the non NHibernate <see cref="IDictionary"/>.
		/// </returns>
		public override IPersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new PersistentMap( session, ( IDictionary ) collection );
		}

		protected override void Add( ICollection collection, object element )
		{
			DictionaryEntry de = ( DictionaryEntry ) element;
			( ( IDictionary ) collection ).Add( de.Key, de.Value );
		}

		protected override void Clear( ICollection collection )
		{
			( ( IDictionary ) collection ).Clear();
		}

		protected override object CopyElement(ICollectionPersister persister, object element, ISessionImplementor session, object owner, IDictionary copiedAlready)
		{
			DictionaryEntry de = ( DictionaryEntry ) element;
			return new DictionaryEntry(
				persister.IndexType.Copy( de.Key, null, session, owner, copiedAlready ),
				persister.ElementType.Copy( de.Value, null, session, owner, copiedAlready ) );
		}
	}
}