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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public override int GetColumnSpan( IMapping session )
		{
			return session.GetIdentifierType( PersistentClass ).GetColumnSpan( session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public override SqlType[ ] SqlTypes( IMapping session )
		{
			return session.GetIdentifierType( PersistentClass ).SqlTypes( session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		public ManyToOneType( System.Type persistentClass ) : base( persistentClass )
		{
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
			session.Factory.GetIdentifierType( PersistentClass )
				.NullSafeSet( cmd, GetIdentifier( value, session ), index, session );
		}

		/// <summary></summary>
		public override bool IsOneToOne
		{
			get { return false; }
		}

		/// <summary></summary>
		public virtual ForeignKeyType ForeignKeyType
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
			return session.Factory.GetIdentifierType( PersistentClass )
				.NullSafeGet( rs, names, session, owner );
		}

		/// <summary>
		/// Resolves the Identifier to the actual object.
		/// </summary>
		/// <param name="value">The identifier object.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> this is occurring in.</param>
		/// <param name="owner"></param>
		/// <returns>
		/// The object that is identified by the parameter <c>value</c> or <c>null</c> if the parameter
		/// <c>value</c> is also <c>null</c>. 
		/// </returns>
		public override object ResolveIdentifier( object value, ISessionImplementor session, object owner )
		{
			if( value == null )
			{
				return null;
			}
			else
			{
				return session.InternalLoad( PersistentClass, value );
			}
		}
	}
}