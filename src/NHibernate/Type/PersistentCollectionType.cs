using System;
using System.Collections;
using System.Data;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
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
			return x == y ||
				( x is PersistentCollection && ( (PersistentCollection) x ).IsWrapper( y ) ) ||
				( y is PersistentCollection && ( (PersistentCollection) y ).IsWrapper( x ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public abstract PersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister );

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

		private bool IsOwnerVersioned( ISessionImplementor session )
		{
			System.Type ownerClass = session.Factory.GetCollectionPersister( role ).OwnerClass;

			return session.Factory.GetPersister( ownerClass ).IsVersioned;
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
			// collections don't dirty an unversioned parent entity

			// TODO: I don't like this implementation; it would be better if this was handled by SearchForDirtyCollections();
			return IsOwnerVersioned( session ) && base.IsDirty( old, current, session );
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return false; }
		}

		/// <summary>
		/// Wraps a collection from System.Collections or Iesi.Collections inside one of the 
		/// NHibernate collections.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped collection.</param>
		/// <returns>
		/// A subclass of <see cref="PersistentCollection"/> that wraps the non NHibernate collection.
		/// </returns>
		public abstract PersistentCollection Wrap( ISessionImplementor session, object collection );

		// Note: return true because this type is castable to IAssociationType. Not because
		// all collections are associations.
		/// <summary></summary>
		public override bool IsAssociationType
		{
			get { return true; }
		}

		/// <summary></summary>
		public bool UsePrimaryKeyAsForeignKey
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public IJoinable GetJoinable( ISessionFactoryImplementor factory )
		{
			return (IJoinable) factory.GetCollectionPersister( role );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public string[] GetReferencedColumns( ISessionFactoryImplementor factory )
		{
			//I really, really don't like the fact that a Type now knows about column mappings!
			//bad seperation of concerns ... could we move this somehow to Joinable interface??
			return GetJoinable( factory ).JoinKeyColumns ;
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
		/// <param name="old"></param>
		/// <param name="current"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override bool IsModified(object old, object current, ISessionImplementor session)
		{
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public System.Type GetAssociatedClass( ISessionFactoryImplementor factory )
		{
			try
			{
				IQueryableCollection collectionPersister = (IQueryableCollection) factory.GetCollectionPersister( role );
				if ( collectionPersister.ElementType.IsEntityType )
				{
					throw new MappingException( string.Format( "collection was not an association: {0}", collectionPersister.Role ) ) ;
				}
				return collectionPersister.ElementPersister.MappedClass;
			}
			catch ( InvalidCastException ice)
			{
				throw new MappingException( "collection role is not queryable", ice );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="original"></param>
		/// <param name="target"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <param name="copiedAlready"></param>
		/// <returns></returns>
		public override object Copy( object original, object target, ISessionImplementor session, object owner, IDictionary copiedAlready )
		{
			if ( original == null )
			{
				return null;
			}
			if ( !NHibernateUtil.IsInitialized( original ) )
			{
				return target;
			}

			IList originalCopy = new ArrayList( (IList) original );
			IList result = target == null ? (IList) Instantiate( session, session.Factory.GetCollectionPersister( role ) ) : (IList) target;
			result.Clear();
			IType elemType = GetElementType( session.Factory );
			foreach ( object obj in originalCopy )
			{
				result.Add( elemType.Copy( obj, null, session, owner, copiedAlready ) );
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public IType GetElementType( ISessionFactoryImplementor factory )
		{
			return factory.GetCollectionPersister( Role ).ElementType;
		}
	}
}