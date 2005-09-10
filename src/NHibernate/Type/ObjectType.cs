using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Loader;
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
	///	DiffClass, AssebmlyName			5
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

		/// <summary></summary>
		public override string Name
		{
			get { return "Object"; }
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return false; }
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
			throw new NotSupportedException( "object is a multicolumn type" );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="names"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		/// <param name="session"></param>
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

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( object ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public override SqlType[ ] SqlTypes( IMapping mapping )
		{
			return ArrayHelper.Join(
				metaType.SqlTypes( mapping ),
				identifierType.SqlTypes( mapping ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override string ToString( object value, ISessionFactoryImplementor factory )
		{
			return value == null ?
				"null" :
				NHibernateUtil.Entity( NHibernateProxyHelper.GetClass( value ) )
					.ToString( value, factory );
		}

		public override object FromString( string xml )
		{
			throw new NotSupportedException();//TODO: is this right??
		}

		/// <summary></summary>
		[Serializable]
		public sealed class ObjectTypeCacheEntry
		{
			/// <summary></summary>
			public System.Type clazz;
			/// <summary></summary>
			public object id;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="clazz"></param>
			/// <param name="id"></param>
			public ObjectTypeCacheEntry( System.Type clazz, object id )
			{
				this.clazz = clazz;
				this.id = id;
			}
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
			ObjectTypeCacheEntry e = ( ObjectTypeCacheEntry ) cached;
			return ( cached == null ) ? null : session.Load( e.clazz, e.id );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public override object Disassemble( object value, ISessionImplementor session )
		{
			return ( value == null ) ? null : new ObjectTypeCacheEntry( value.GetType(), session.GetEntityIdentifier( value ) );
		}

		/// <summary></summary>
		public override bool IsObjectType
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public Cascades.CascadeStyle Cascade( int i )
		{
			return Cascades.CascadeStyle.StyleNone;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public OuterJoinFetchStrategy EnableJoinedFetch( int i )
		{
			return OuterJoinFetchStrategy.Lazy;
		}

		private static readonly string[ ] PROPERTY_NAMES = new string[ ] {"class", "id"};

		/// <summary></summary>
		public string[ ] PropertyNames
		{
			get { return ObjectType.PROPERTY_NAMES; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="i"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object GetPropertyValue( Object component, int i, ISessionImplementor session )
		{
			return ( i == 0 ) ?
				NHibernateProxyHelper.GetClass( component ) :
				Id( component, session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object[ ] GetPropertyValues( Object component, ISessionImplementor session )
		{
			return new object[ ] { NHibernateProxyHelper.GetClass( component ), Id( component, session )};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="session"></param>
		/// <returns></returns>
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

		/// <summary></summary>
		public IType[ ] Subtypes
		{
			get { return new IType[ ] {metaType, identifierType}; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="values"></param>
		public void SetPropertyValues( object component, object[ ] values )
		{
			throw new NotSupportedException();
		}

		public object[] GetPropertyValues( object component )
		{
			throw new NotSupportedException();
		}

		/// <summary></summary>
		public override bool IsComponentType
		{
			get { return true; }
		}

		/// <summary></summary>
		public ForeignKeyType ForeignKeyType
		{
			get
			{
				//return AssociationType.FOREIGN_KEY_TO_PARENT; //TODO: this is better but causes a transient object exception... 
				return ForeignKeyType.ForeignKeyFromParent;
			}
		}

		/// <summary></summary>
		public override bool IsAssociationType
		{
			get { return true; }
		}

		/// <summary>
		/// Not really relevant to ObjectType, since it cannot be "joined"
		/// </summary>
		public bool UsePrimaryKeyAsForeignKey
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public IJoinable GetJoinable( ISessionFactoryImplementor factory )
		{
			throw new InvalidOperationException( "any types do not have a unique referenced persister" );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public string[] GetReferencedColumns( ISessionFactoryImplementor factory )
		{
			throw new InvalidOperationException( "any types do not have unique referenced columns" );
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public System.Type GetAssociatedClass( ISessionFactoryImplementor factory )
		{
			throw new InvalidOperationException( "any types do not have a unique referenced persister" );
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