using System.Collections;
using System.Data;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// PersistentCollectionType.
	/// </summary>
	public abstract class PersistentCollectionType : AbstractType, IAssociationType
	{
		private readonly string role;
		private static readonly SqlType[ ] NoSqlTypes = {};

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		protected PersistentCollectionType( string role )
		{
			this.role = role;
		}

		/// <summary></summary>
		public virtual string Role
		{
			get { return role; }
		}

		/// <summary></summary>
		public override bool IsPersistentCollectionType
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override sealed bool Equals( object x, object y )
		{
			// proxies? - comment in h2.0.3 also
			return x == y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public abstract PersistentCollection Instantiate( ISessionImplementor session, CollectionPersister persister );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object NullSafeGet( IDataReader rs, string name, ISessionImplementor session, object owner )
		{
			throw new AssertionFailure( "bug in PersistentCollectionType" );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object NullSafeGet( IDataReader rs, string[ ] name, ISessionImplementor session, object owner )
		{
			return ResolveIdentifier( Hydrate( rs, name, session, owner ), session, owner );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		/// <param name="session"></param>
		public override void NullSafeSet( IDbCommand cmd, object value, int index, ISessionImplementor session )
		{
		}

		/*
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="owner"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public virtual object GetCollection( object id, object owner, ISessionImplementor session )
		{
			// added the owner
			PersistentCollection collection = session.GetLoadingCollection( role, id );
			if( collection != null )
			{
				return collection.GetCachedValue();
			} //TODO: yuck... call another method - H2.0.3comment

			CollectionPersister persister = session.Factory.GetCollectionPersister( role );
			collection = persister.GetCachedCollection( id, owner, session );
			if( collection != null )
			{
				session.AddInitializedCollection( collection, persister, id );
				return collection.GetCachedValue();
			}
			else
			{
				collection = Instantiate( session, persister );
				session.AddUninitializedCollection( collection, persister, id );
				return collection.GetInitialValue( persister.IsLazy );
			}

		}
		*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public override SqlType[ ] SqlTypes( IMapping session )
		{
			return NoSqlTypes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public override int GetColumnSpan( IMapping session )
		{
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override string ToXML( object value, ISessionFactoryImplementor factory )
		{
			return ( value == null ) ? null : value.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object DeepCopy( object value )
		{
			return value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return ReturnedClass.Name; }
		}


		/// <summary>
		/// Returns a reference to the elements in the collection.  
		/// </summary>
		/// <param name="collection">The object that holds the ICollection.</param>
		/// <returns>An ICollection of the Elements(classes) in the Collection.</returns>
		/// <remarks>
		/// By default the parameter <c>collection</c> is just cast to an ICollection.  Collections
		/// such as Maps and Sets should override this so that the Elements are returned - not a
		/// DictionaryEntry.
		/// </remarks>
		public virtual ICollection GetElementsCollection( object collection )
		{
			return ( ( ICollection ) collection );
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Disassemble( object value, ISessionImplementor session )
		{
			return null;
			// commented out in h2.0.3 also
			//			if (value==null) {
			//				return null;
			//			}
			//			else {
			//				object id = session.GetLoadedCollectionKey( (PersistentCollection) value );
			//				if (id==null)
			//					throw new AssertionFailure("Null collection id");
			//				return id;
			//			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cached"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Assemble( object cached, ISessionImplementor session, object owner )
		{
			object id = session.GetEntityIdentifier( owner );
			if( id == null )
			{
				throw new AssertionFailure( "bug re-assembling collection reference" );
			}
			return ResolveIdentifier( id, session, owner );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="old"></param>
		/// <param name="current"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override bool IsDirty( object old, object current, ISessionImplementor session )
		{
			System.Type ownerClass = session.Factory.GetCollectionPersister( role ).OwnerClass;

			if( !session.Factory.GetPersister( ownerClass ).IsVersioned )
			{
				// collections don't dirty an unversioned parent entity
				return false;
			}
			else
			{
				return base.IsDirty( old, current, session );
			}
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public abstract PersistentCollection Wrap( ISessionImplementor session, object collection );

		// Note: return true because this type is castable to IAssociationType. Not because
		// all collections are associations.
		/// <summary></summary>
		public override bool IsAssociationType
		{
			get { return true; }
		}

		/// <summary></summary>
		public virtual ForeignKeyType ForeignKeyType
		{
			get { return ForeignKeyType.ForeignKeyToParent; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Hydrate( IDataReader rs, string[ ] name, ISessionImplementor session, object owner )
		{
			return session.GetEntityIdentifier( owner );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object ResolveIdentifier( object value, ISessionImplementor session, object owner )
		{
			if( value == null )
			{
				return null;
			}
			else
			{
				return session.GetCollection( role, value, owner );
			}
		}

		/// <summary></summary>
		public virtual bool IsArrayType
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <param name="disassembled"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public abstract PersistentCollection AssembleCachedCollection( ISessionImplementor session, CollectionPersister persister, object disassembled, object owner );
	}
}