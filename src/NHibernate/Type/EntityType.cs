using System.Data;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// A reference to an entity class
	/// </summary>
	public abstract class EntityType : AbstractType
	{
		private readonly System.Type persistentClass;
		private readonly bool niceEquals;

		/// <summary></summary>
		public override sealed bool IsEntityType
		{
			get { return true; }
		}

		/// <summary></summary>
		public System.Type PersistentClass
		{
			get { return persistentClass; }
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
		protected EntityType( System.Type persistentClass )
		{
			this.persistentClass = persistentClass;
			this.niceEquals = !ReflectHelper.OverridesEquals( persistentClass );
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

		/**
		* This returns the wrong class for an entity with a proxy. Theoretically
		* it should return the proxy class, but it doesn't.
		*/

		/// <summary></summary>
		public override sealed System.Type ReturnedClass
		{
			get { return persistentClass; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected object GetIdentifier( object value, ISessionImplementor session )
		{
			return session.GetEntityIdentifierIfNotUnsaved( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override string ToXML( object value, ISessionFactoryImplementor factory )
		{
			IClassPersister persister = factory.GetPersister( persistentClass );
			return ( value == null ) ? null : persister.IdentifierType.ToXML( persister.GetIdentifier( value ), factory );
		}

		/// <summary></summary>
		public override string Name
		{
			get { return persistentClass.Name; }
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
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Disassemble( object value, ISessionImplementor session )
		{
			if( value == null )
			{
				return null;
			}
			else
			{
				object id = session.GetIdentifier( value );
				if( id == null )
				{
					throw new AssertionFailure( "cannot cache a reference to an object with a null id" );
				}
				return GetIdentifierType( session ).Disassemble( id, session );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		protected IType GetIdentifierType( ISessionImplementor session )
		{
			return session.Factory.GetIdentifierType( persistentClass );
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
	}
}