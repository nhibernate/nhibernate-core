using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// A one-to-one association to an entity
	/// </summary>
	public class OneToOneType : EntityType, IAssociationType
	{
		private static readonly SqlType[ ] NoSqlTypes = new SqlType[0];

		private readonly ForeignKeyType foreignKeyType;

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
		/// <param name="session"></param>
		/// <returns></returns>
		public override SqlType[ ] SqlTypes( IMapping session )
		{
			return NoSqlTypes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="foreignKeyType"></param>
		/// <param name="uniqueKeyPropertyName"></param>
		public OneToOneType( System.Type persistentClass, ForeignKeyType foreignKeyType, string uniqueKeyPropertyName ) : base( persistentClass, uniqueKeyPropertyName )
		{
			this.foreignKeyType = foreignKeyType;
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
			//nothing to do
		}

		/// <summary></summary>
		public override bool IsOneToOne
		{
			get { return true; }
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
			return false;
		}

		/// <summary></summary>		
		public override bool UsePrimaryKeyAsForeignKey
		{
			get { return true; }
		}

		/// <summary></summary>
		public override ForeignKeyType ForeignKeyType
		{
			get { return foreignKeyType; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="names"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Hydrate( IDataReader rs, string[ ] names, ISessionImplementor session, object owner )
		{
			return session.GetEntityIdentifier( owner );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected override object ResolveIdentifier( object value, ISessionImplementor session )
		{
			System.Type clazz = AssociatedClass;

			return IsNullable ?
				session.InternalLoadOneToOne( clazz, value ) :
				session.InternalLoad( clazz, value );
		}

		/// <summary></summary>
		public virtual bool IsNullable
		{
			get { return foreignKeyType == ForeignKeyType.ForeignKeyToParent; }
		}

	}
}