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
			return session.GetIdentifierType( AssociatedClass ).GetColumnSpan( session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public override SqlType[ ] SqlTypes( IMapping session )
		{
			return session.GetIdentifierType( AssociatedClass ).SqlTypes( session );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="uniqueKeyPropertyName"></param>
		public ManyToOneType( System.Type persistentClass, string uniqueKeyPropertyName ) : base( persistentClass, uniqueKeyPropertyName )
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
			session.Factory.GetIdentifierType( AssociatedClass )
				.NullSafeSet( cmd, GetIdentifier( value, session ), index, session );
		}

		/// <summary></summary>
		public override bool IsOneToOne
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool UsePrimaryKeyAsForeignKey
		{
			get { return false; }
		}

		/// <summary></summary>
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
			return session.Factory.GetIdentifierType( AssociatedClass )
				.NullSafeGet( rs, names, session, owner );
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
	}
}