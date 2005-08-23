using System;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Proxy;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// A reference to an entity class
	/// </summary>
	public abstract class EntityType : AbstractType, IAssociationType
	{
		private readonly System.Type associatedClass;
		private readonly bool niceEquals;

		/// <summary></summary>
		protected readonly string uniqueKeyPropertyName;

		/// <summary></summary>
		public override sealed bool IsEntityType
		{
			get { return true; }
		}

		/// <summary></summary>
		public System.Type AssociatedClass
		{
			get { return associatedClass; }
		}

		/// <summary></summary>
		public System.Type GetAssociatedClass( ISessionFactoryImplementor factory )
		{
			return associatedClass;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override sealed bool Equals( object x, object y )
		{
			return x == y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="uniqueKeyPropertyName"></param>
		protected EntityType( System.Type persistentClass, string uniqueKeyPropertyName )
		{
			this.associatedClass = persistentClass;
			this.niceEquals = !ReflectHelper.OverridesEquals( persistentClass );
			this.uniqueKeyPropertyName = uniqueKeyPropertyName;
		}

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
			return NullSafeGet( rs, new string[ ] {name}, session, owner );
		}

		// This returns the wrong class for an entity with a proxy. Theoretically
		// it should return the proxy class, but it doesn't.
		/// <summary></summary>
		public override sealed System.Type ReturnedClass
		{
			get { return associatedClass; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected object GetIdentifier( object value, ISessionImplementor session )
		{
			if( uniqueKeyPropertyName==null )
			{
				return session.GetEntityIdentifierIfNotUnsaved( value ); //tolerates nulls
			}
			else if( value == null ) 
			{
				return null;
			}
			else 
			{
				return session.Factory
					.GetPersister( AssociatedClass )
					.GetPropertyValue( value, uniqueKeyPropertyName );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override string ToString( object value, ISessionFactoryImplementor factory )
		{
			IClassPersister persister = factory.GetPersister( associatedClass );
			if( value == null )
			{
				return "null";
			}
			StringBuilder result = new StringBuilder();
			result.Append(  StringHelper.Unqualify( NHibernateProxyHelper.GetClass( value ).FullName ) );
			if( persister.HasIdentifierProperty )
			{
				result.Append( '#' )
					.Append( persister.IdentifierType.ToString( NHibernateProxyHelper.GetIdentifier( value, persister ), factory ) );
			}

			return result.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object FromString( string xml )
		{
			throw new NotSupportedException(); //TODO: is this correct???
		}

		/// <summary></summary>
		public override string Name
		{
			get { return associatedClass.Name; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object DeepCopy( object value )
		{
			return value; //special case ... this is the leaf of the containment graph, even though not immutable
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return false; }
		}

		/// <summary></summary>
		public abstract bool IsOneToOne { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		protected IType GetIdentifierType( ISessionImplementor session )
		{
			return session.Factory.GetIdentifierType( associatedClass );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public IType GetIdentifierOrUniqueKeyType( ISessionFactoryImplementor factory )
		{
			if ( uniqueKeyPropertyName == null )
			{
				return factory.GetIdentifierType( associatedClass );
			}
			else
			{
				return factory.GetPersister( associatedClass ).GetPropertyType( uniqueKeyPropertyName );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public string GetIdentifierOrUniqueKeyPropertyName( ISessionFactoryImplementor factory )
		{
			if ( uniqueKeyPropertyName == null )
			{
				return factory.GetIdentifierPropertyName( associatedClass );
			}
			else
			{
				return uniqueKeyPropertyName;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected abstract object ResolveIdentifier( object id, ISessionImplementor session );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object ResolveIdentifier( object id, ISessionImplementor session, object owner )
		{
			if ( id == null )
			{
				return null;
			}
			else
			{
				if ( uniqueKeyPropertyName == null )
				{
					return ResolveIdentifier( id, session );
				}
				else
				{
					return session.LoadByUniqueKey( AssociatedClass, uniqueKeyPropertyName, id );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oid"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Assemble( object oid, ISessionImplementor session, object owner )
		{
			object assembledId = GetIdentifierType( session ).Assemble( oid, session, owner );

			return ResolveIdentifier( assembledId, session, owner );
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return niceEquals; }
		}

		/// <summary></summary>
		public override bool IsAssociationType
		{
			get { return true; }
		}

		/// <summary></summary>
		public bool IsUniqueKeyReference
		{
			get { return uniqueKeyPropertyName != null; }
		}

		/// <summary>
		/// Converts the id contained in the <see cref="IDataReader"/> to an object.
		/// </summary>
		/// <param name="rs">The <see cref="IDataReader"/> that contains the query results.</param>
		/// <param name="names">A string array of column names that contain the id.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> this is occurring in.</param>
		/// <param name="owner">The object that this Entity will be a part of.</param>
		/// <returns>
		/// An instance of the object or <c>null</c> if the identifer was null.
		/// </returns>
		public override sealed object NullSafeGet( IDataReader rs, string[ ] names, ISessionImplementor session, object owner )
		{
			return ResolveIdentifier( Hydrate( rs, names, session, owner ), session, owner );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="names"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public abstract override object Hydrate( IDataReader rs, string[ ] names, ISessionImplementor session, object owner );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="old"></param>
		/// <param name="current"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override bool IsDirty( object old, object current, ISessionImplementor session )
		{
			if( Equals( old, current ) )
			{
				return false;
			}

			object oldid = GetIdentifier( old, session );
			object newid = GetIdentifier( current, session );
			return !GetIdentifierType( session ).Equals( oldid, newid );
		}

		#region IAssociationType Members
		/// <summary>
		/// When implemented by a class, gets the type of foreign key directionality 
		/// of this association.
		/// </summary>
		/// <value>The <see cref="ForeignKeyType"/> of this association.</value>
		public abstract ForeignKeyType ForeignKeyType { get; }

		/// <summary>
		/// Is the foreign key the primary key of the table?
		/// </summary>
		public abstract bool UsePrimaryKeyAsForeignKey { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public IJoinable GetJoinable( ISessionFactoryImplementor factory )
		{
			return (IJoinable) factory.GetPersister( associatedClass );
		}

		/// <summary></summary>
		public string[] GetReferencedColumns( ISessionFactoryImplementor factory )
		{
			//I really, really don't like the fact that a Type now knows about column mappings!
			//bad seperation of concerns ... could we move this somehow to Joinable interface??
			IJoinable joinable = GetJoinable( factory );

			if ( uniqueKeyPropertyName == null ) 
			{
				return joinable.JoinKeyColumns ;
			}
			else 
			{
				return ( (IUniqueKeyLoadable) joinable ).GetUniqueKeyColumnNames( uniqueKeyPropertyName );
			}
		}
		#endregion
	}
}