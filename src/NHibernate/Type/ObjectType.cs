using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Proxy;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	///<summary>
	///	Handles "any" mappings and the old deprecated "object" type.
	///</summary>
	///<remarks>
	///	The identifierType is any NHibernate IType that can be serailized by default.
	///	For example, you can specify the identifierType as an Int32 or a custom identifier
	///	type that you built.  The identifierType matches to one or many columns.
	///	
	///	The metaType maps to a single column.  By default it stores the name of the Type
	///	that the Identifier identifies.  
	///	
	///	For example, we can store a link to any table.  It will have the results
	///	class_name					id_col1
	///	========================================
	///	Simple, AssemblyName			5
	///	DiffClass, AssemblyName			5
	///	Simple, AssemblyName			4
	///	
	///	You can also provide you own type that might map the name of the class to a table
	///	with a giant switch statemet or a good naming convention for your class->table.  The
	///	data stored might look like
	///	class_name					id_col1
	///	========================================
	///	simple_table					5
	///	diff_table						5
	///	simple_table					4
	///	
	///</remarks>
	public class ObjectType : AbstractType, IAbstractComponentType, IAssociationType
	{
		private readonly IType identifierType;
		private readonly IType metaType;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="metaType"></param>
		/// <param name="identifierType"></param>
		internal ObjectType( IType metaType, IType identifierType )
		{
			this.identifierType = identifierType;
			this.metaType = metaType;
		}

		/// <summary></summary>
		internal ObjectType() : this( NHibernateUtil.Class, NHibernateUtil.Serializable )
		{
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals( object x, object y )
		{
			return x == y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public override int GetColumnSpan( IMapping session )
		{
			/*
			 * This is set at 2 in Hibernate to support the old depreciated
			 * version of using type="object".  We are removing that support so
			 * I don't know if we need to keep this in.
			 */
			return 2;
		}

		public override string Name
		{
			get { return "Object"; }
		}

		public override bool HasNiceEquals
		{
			get { return false; }
		}

		public override bool IsMutable
		{
			get { return false; }
		}

		public override object NullSafeGet( IDataReader rs, string name, ISessionImplementor session, object owner )
		{
			throw new NotSupportedException( "object is a multicolumn type" );
		}

		public override object NullSafeGet( IDataReader rs, string[ ] names, ISessionImplementor session, object owner )
		{
			return Resolve(
				( System.Type ) metaType.NullSafeGet( rs, names[ 0 ], session, owner ),
				identifierType.NullSafeGet( rs, names[ 1 ], session, owner ),
				session );
		}

		public override object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			System.Type clazz = ( System.Type ) metaType.NullSafeGet( rs, names[ 0 ], session, owner );
			object id = identifierType.NullSafeGet( rs, names[ 1 ], session, owner );
			return new ObjectTypeCacheEntry( clazz, id );
		}

		public override object ResolveIdentifier(object value, ISessionImplementor session, object owner)
		{
			ObjectTypeCacheEntry holder = ( ObjectTypeCacheEntry ) value;
			return Resolve( holder.clazz, holder.id, session );
		}

		private object Resolve( System.Type clazz, object id, ISessionImplementor session )
		{
			return (clazz == null || id == null ) ?
				null :
				session.InternalLoad( clazz, id );
		}

		public override void NullSafeSet( IDbCommand st, object value, int index, ISessionImplementor session )
		{
			object id;
			System.Type clazz;

			if( value == null )
			{
				id = null;
				clazz = null;
			}
			else
			{
				id = session.GetEntityIdentifierIfNotUnsaved( value );
				clazz = NHibernateProxyHelper.GetClass( value );
			}
			metaType.NullSafeSet( st, clazz, index, session );
			identifierType.NullSafeSet( st, id, index + 1, session ); // metaType must be single-column type
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( object ); }
		}

		public override SqlType[ ] SqlTypes( IMapping mapping )
		{
			return ArrayHelper.Join(
				metaType.SqlTypes( mapping ),
				identifierType.SqlTypes( mapping ) );
		}

		public override string ToLoggableString( object value, ISessionFactoryImplementor factory )
		{
			return value == null ?
				"null" :
				NHibernateUtil.Entity( NHibernateProxyHelper.GetClass( value ) )
					.ToLoggableString( value, factory );
		}

		public override object FromString( string xml )
		{
			throw new NotSupportedException();//TODO: is this right??
		}

		[Serializable]
		public sealed class ObjectTypeCacheEntry
		{
			public System.Type clazz;
			public object id;

			public ObjectTypeCacheEntry( System.Type clazz, object id )
			{
				this.clazz = clazz;
				this.id = id;
			}
		}

		public override object Assemble( object cached, ISessionImplementor session, object owner )
		{
			ObjectTypeCacheEntry e = ( ObjectTypeCacheEntry ) cached;
			return ( cached == null ) ? null : session.Load( e.clazz, e.id );
		}

		public override object Disassemble( object value, ISessionImplementor session )
		{
			return ( value == null ) ?
				null :
				new ObjectTypeCacheEntry(
					value.GetType(),
					session.GetEntityIdentifier( value ) );
		}

		public override bool IsObjectType
		{
			get { return true; }
		}

		public Cascades.CascadeStyle Cascade( int i )
		{
			return Cascades.CascadeStyle.StyleNone;
		}

		public FetchMode GetFetchMode( int i )
		{
			return FetchMode.Select;
		}

		private static readonly string[ ] PROPERTY_NAMES = new string[ ] {"class", "id"};

		public string[ ] PropertyNames
		{
			get { return ObjectType.PROPERTY_NAMES; }
		}

		public object GetPropertyValue( Object component, int i, ISessionImplementor session )
		{
			return ( i == 0 ) ?
				NHibernateProxyHelper.GetClass( component ) :
				Id( component, session );
		}

		public object[ ] GetPropertyValues( Object component, ISessionImplementor session )
		{
			return new object[ ] { NHibernateProxyHelper.GetClass( component ), Id( component, session )};
		}

		private object Id( object component, ISessionImplementor session )
		{
			try
			{
				return session.GetEntityIdentifierIfNotUnsaved( component );
			}
			catch( TransientObjectException )
			{
				return null;
			}
		}

		public IType[ ] Subtypes
		{
			get { return new IType[ ] { metaType, identifierType }; }
		}

		public void SetPropertyValues( object component, object[ ] values )
		{
			throw new NotSupportedException();
		}

		public object[] GetPropertyValues( object component )
		{
			throw new NotSupportedException();
		}

		public override bool IsComponentType
		{
			get { return true; }
		}

		public ForeignKeyDirection ForeignKeyDirection
		{
			get
			{
				//return AssociationType.FOREIGN_KEY_TO_PARENT; //TODO: this is better but causes a transient object exception... 
				return ForeignKeyDirection.ForeignKeyFromParent;
			}
		}

		public override bool IsAssociationType
		{
			get { return true; }
		}

		/// <summary>
		/// Not really relevant to ObjectType, since it cannot be "joined"
		/// </summary>
		public bool UseLHSPrimaryKey
		{
			get { return false; }
		}

		public IJoinable GetAssociatedJoinable( ISessionFactoryImplementor factory )
		{
			throw new InvalidOperationException( "any types do not have a unique referenced persister" );
		}

		public string[] GetReferencedColumns( ISessionFactoryImplementor factory )
		{
			throw new InvalidOperationException( "any types do not have unique referenced columns" );
		}

		public override bool IsModified(object old, object current, ISessionImplementor session)
		{
			if ( current == null )
			{
				return old != null;
			}
			if ( old == null )
			{
				return current != null;
			}
			ObjectTypeCacheEntry holder = (ObjectTypeCacheEntry) old;

			return holder.clazz != NHibernateProxyHelper.GetClass( current ) ||
				identifierType.IsModified( holder.id, Id( current, session ), session );
		}

		public System.Type GetAssociatedClass( ISessionFactoryImplementor factory )
		{
			throw new InvalidOperationException( "any types do not have a unique referenced persister" );
		}

		public string LHSPropertyName
		{
			get { return null; }
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return 1; // Originally: System.identityHashCode(this);
		}
	}
}