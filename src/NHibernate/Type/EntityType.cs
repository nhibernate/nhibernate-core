using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Persister.Entity;
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
		protected readonly string uniqueKeyPropertyName;

		public override sealed bool IsEntityType
		{
			get { return true; }
		}

		public System.Type AssociatedClass
		{
			get { return associatedClass; }
		}

		public override sealed bool Equals( object x, object y )
		{
			return x == y;
		}

		protected EntityType( System.Type persistentClass, string uniqueKeyPropertyName )
		{
			this.associatedClass = persistentClass;
			this.niceEquals = !ReflectHelper.OverridesEquals( persistentClass );
			this.uniqueKeyPropertyName = uniqueKeyPropertyName;
		}

		public override object NullSafeGet( IDataReader rs, string name, ISessionImplementor session, object owner )
		{
			return NullSafeGet( rs, new string[ ] {name}, session, owner );
		}

		// This returns the wrong class for an entity with a proxy. Theoretically
		// it should return the proxy class, but it doesn't.
		public override sealed System.Type ReturnedClass
		{
			get { return associatedClass; }
		}

		protected object GetIdentifier( object value, ISessionImplementor session )
		{
			if( uniqueKeyPropertyName == null )
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

		public override string ToLoggableString( object value, ISessionFactoryImplementor factory )
		{
			IEntityPersister persister = factory.GetPersister( associatedClass );
			if( value == null )
			{
				return "null";
			}
			StringBuilder result = new StringBuilder();
			result.Append( StringHelper.Unqualify( NHibernateProxyHelper.GetClass( value ).FullName ) );
			if( persister.HasIdentifierProperty )
			{
				result.Append( '#' )
					.Append( persister.IdentifierType.ToLoggableString( NHibernateProxyHelper.GetIdentifier( value, persister ), factory ) );
			}

			return result.ToString();
		}

		public override object FromString( string xml )
		{
			throw new NotSupportedException(); //TODO: is this correct???
		}

		public override string Name
		{
			get { return associatedClass.Name; }
		}

		public override object DeepCopy( object value )
		{
			return value; //special case ... this is the leaf of the containment graph, even though not immutable
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return false; }
		}

		public abstract bool IsOneToOne { get; }

		public override object Copy( object original, object target, ISessionImplementor session, object owner, IDictionary copyCache )
		{
			if( original == null )
			{
				return null;
			}
			
			object cached = copyCache[ original ];
			if( cached != null )
			{
				return cached;
			}
			else
			{
				if( original == target )
				{
					return target;
				}

				IType idType = GetIdentifierOrUniqueKeyType( session.Factory );
				object id = GetIdentifier( original, session );
				if( id == null )
				{
					throw new AssertionFailure( "cannot copy a reference to an object with a null id" );
				}
				
				id = idType.Copy( id, null, session, owner, copyCache );

				// Replace a property-ref'ed entity by its id
				if( uniqueKeyPropertyName != null && idType.IsEntityType )
				{
					id = ( ( EntityType ) idType ).GetIdentifier( id, session );
				}
				return ResolveIdentifier( id, session, owner );
			}
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

		public abstract override object Hydrate( IDataReader rs, string[ ] names, ISessionImplementor session, object owner );

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

		public bool IsUniqueKeyReference
		{
			get { return uniqueKeyPropertyName != null; }
		}

		public IJoinable GetAssociatedJoinable( ISessionFactoryImplementor factory )
		{
			return ( IJoinable ) factory.GetPersister( associatedClass );
		}

		public string[ ] GetReferencedColumns( ISessionFactoryImplementor factory )
		{
			//I really, really don't like the fact that a Type now knows about column mappings!
			//bad seperation of concerns ... could we move this somehow to Joinable interface??
			IJoinable joinable = GetAssociatedJoinable( factory );

			if( uniqueKeyPropertyName == null )
			{
				return joinable.KeyColumnNames;
			}
			else
			{
				return ( ( IUniqueKeyLoadable ) joinable ).GetUniqueKeyColumnNames( uniqueKeyPropertyName );
			}
		}

		protected IType GetIdentifierType( ISessionImplementor session )
		{
			return session.Factory.GetIdentifierType( associatedClass );
		}

		public IType GetIdentifierOrUniqueKeyType( ISessionFactoryImplementor factory )
		{
			if( uniqueKeyPropertyName == null )
			{
				return factory.GetIdentifierType( associatedClass );
			}
			else
			{
				return factory.GetPersister( associatedClass ).GetPropertyType( uniqueKeyPropertyName );
			}
		}

		public string GetIdentifierOrUniqueKeyPropertyName( ISessionFactoryImplementor factory )
		{
			if( uniqueKeyPropertyName == null )
			{
				return factory.GetIdentifierPropertyName( associatedClass );
			}
			else
			{
				return uniqueKeyPropertyName;
			}
		}

		/// <summary>
		/// Resolve an identifier
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected abstract object ResolveIdentifier( object id, ISessionImplementor session );

		/// <summary>
		/// Resolve an identifier or unique key value
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object ResolveIdentifier( object id, ISessionImplementor session, object owner )
		{
			if( id == null )
			{
				return null;
			}
			else
			{
				if( uniqueKeyPropertyName == null )
				{
					return ResolveIdentifier( id, session );
				}
				else
				{
					return session.LoadByUniqueKey( AssociatedClass, uniqueKeyPropertyName, id );
				}
			}
		}

		public System.Type GetAssociatedClass( ISessionFactoryImplementor factory )
		{
			return associatedClass;
		}

		/// <summary>
		/// When implemented by a class, gets the type of foreign key directionality 
		/// of this association.
		/// </summary>
		/// <value>The <see cref="ForeignKeyDirection"/> of this association.</value>
		public abstract ForeignKeyDirection ForeignKeyDirection { get; }

		/// <summary>
		/// Is the foreign key the primary key of the table?
		/// </summary>
		public abstract bool UseLHSPrimaryKey { get; }

		public string LHSPropertyName
		{
			get { return null; }
		}

		public string RHSUniqueKeyPropertyName
		{
			get { return uniqueKeyPropertyName; }
		}

		public override bool Equals(object obj)
		{
			if( !base.Equals( obj ) )
			{
				return false;
			}

			return ( (EntityType) obj ).associatedClass == associatedClass;
		}

		public override int GetHashCode()
		{
			return associatedClass.GetHashCode();
		}

	}
}