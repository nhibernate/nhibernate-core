using NHibernate.Engine;
using NHibernate.Persister;

namespace NHibernate.Type
{
	/// <summary>
	/// Represents directionality of the foreign key constraint
	/// </summary>
	public abstract class ForeignKeyType
	{
		/// <summary></summary>
		protected ForeignKeyType()
		{
		}

		private class ForeignKeyToParentClass : ForeignKeyType
		{
			public override bool CascadeNow( CascadePoint cascadePoint )
			{
				return cascadePoint != CascadePoint.CascadeBeforeInsertAfterDelete;
			}
		}

		private class ForeignKeyFromParentClass : ForeignKeyType
		{
			public override bool CascadeNow( CascadePoint cascadePoint )
			{
				return cascadePoint != CascadePoint.CascadeAfterInsertBeforeDelete;
			}
		}

		/// <summary>
		/// Should we cascade at this cascade point?
		/// </summary>
		public abstract bool CascadeNow( CascadePoint cascadePoint );

		/// <summary>
		/// A foreign key from child to parent
		/// </summary>
		public static ForeignKeyType ForeignKeyToParent = new ForeignKeyToParentClass();

		/// <summary>
		/// A foreign key from parent to child
		/// </summary>
		public static ForeignKeyType ForeignKeyFromParent = new ForeignKeyFromParentClass();

	}

	/// <summary>
	/// An <see cref="IType"/> that represents some kind of association between entities.
	/// </summary>
	public interface IAssociationType : IType
	{
		/// <summary>
		/// When implemented by a class, gets the type of foreign key directionality 
		/// of this association.
		/// </summary>
		/// <value>The <see cref="ForeignKeyType"/> of this association.</value>
		ForeignKeyType ForeignKeyType { get; }

		/// <summary>
		/// Is the foreign key the primary key of the table?
		/// </summary>
		bool UsePrimaryKeyAsForeignKey { get; }

		/// <summary>
		/// Get the "persister" for this association - a class or collection persister
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		IJoinable GetJoinable( ISessionFactoryImplementor factory );

		/// <summary>
		/// Get the columns referenced by this association.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		string[] GetReferencedColumns( ISessionFactoryImplementor factory );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		System.Type GetAssociatedClass( ISessionFactoryImplementor factory );
	}
}