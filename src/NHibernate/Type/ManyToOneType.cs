using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// A many-to-one association to an entity
	/// </summary>
	public class ManyToOneType : EntityType, IAssociationType
	{
		private IType GetReferencedType( IMapping mapping )
		{
			if( uniqueKeyPropertyName == null )
			{
				return mapping.GetIdentifierType( AssociatedClass );
			}
			else
			{
				return mapping.GetPropertyType( AssociatedClass, uniqueKeyPropertyName );
			}
		}

		public override int GetColumnSpan( IMapping mapping )
		{
			return GetReferencedType( mapping ).GetColumnSpan( mapping );
		}

		public override SqlType[ ] SqlTypes( IMapping mapping )
		{
			return GetReferencedType( mapping ).SqlTypes( mapping );
		}

		public ManyToOneType( System.Type persistentClass ) : this( persistentClass, null )
		{
		}

		public ManyToOneType( System.Type persistentClass, string uniqueKeyPropertyName )
			: base( persistentClass, uniqueKeyPropertyName )
		{
		}

		public override void NullSafeSet( IDbCommand cmd, object value, int index, ISessionImplementor session )
		{
			GetIdentifierOrUniqueKeyType( session.Factory )
				.NullSafeSet( cmd, GetIdentifier( value, session ), index, session );
		}

		public override bool IsOneToOne
		{
			get { return false; }
		}

		public override ForeignKeyType ForeignKeyType
		{
			get { return ForeignKeyType.ForeignKeyFromParent; }
		}

		/// <summary>
		/// Hydrates the Identifier from <see cref="IDataReader"/>.
		/// </summary>
		/// <param name="rs">The <see cref="IDataReader"/> that contains the query results.</param>
		/// <param name="names">A string array of column names to read from.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> this is occuring in.</param>
		/// <param name="owner">The object that this Entity will be a part of.</param>
		/// <returns>
		/// An instantiated object that used as the identifier of the type.
		/// </returns>
		public override object Hydrate( IDataReader rs, string[ ] names, ISessionImplementor session, object owner )
		{
			object id = GetIdentifierOrUniqueKeyType( session.Factory )
				.NullSafeGet( rs, names, session, owner );

			if ( id != null )
			{
				session.ScheduleBatchLoad( AssociatedClass, id );
			}
			return id;
		}

		/// <summary>
		/// Resolves the Identifier to the actual object.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected override object ResolveIdentifier( object id, ISessionImplementor session )
		{
			return session.InternalLoad( AssociatedClass, id );
		}

		public override bool UsePrimaryKeyAsForeignKey
		{
			get { return false; }
		}

		public override bool IsModified(object old, object current, ISessionImplementor session)
		{
			if ( current == null )
			{
				return old != null;
			}
			if ( old == null )
			{
				return current != null ;
			}
			return GetIdentifierOrUniqueKeyType( session.Factory ).IsModified( old, GetIdentifier( current, session ), session );
		}

		public override object Disassemble( object value, ISessionImplementor session )
		{
			if ( value == null )
			{
				return null;
			}
			else
			{
				// cache the actual id of the object, not the value of the
				// property-ref, which might not be initialized
				object id = session.GetEntityIdentifierIfNotUnsaved( value );
				if ( id == null )
				{
					throw new AssertionFailure( "cannot cache a reference to an object with a null id: " + AssociatedClass.Name );
				}
				return GetIdentifierType( session ).Disassemble( id, session );
			}
		}

		public override object Assemble( object oid, ISessionImplementor session, object owner )
		{
			object id = GetIdentifierType( session ).Assemble( oid, session, owner );
			if ( id == null )
			{
				return null;
			}
			else
			{
				return ResolveIdentifier( id, session );
			}
		}
	}
}