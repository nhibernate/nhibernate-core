using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// Represents directionality of the foreign key constraint
	/// </summary>
	public abstract class ForeignKeyDirection
	{
		/// <summary></summary>
		protected ForeignKeyDirection()
		{
		}

		private class ForeignKeyToParentClass : ForeignKeyDirection
		{
			public override bool CascadeNow(CascadePoint cascadePoint)
			{
				return cascadePoint != CascadePoint.BeforeInsertAfterDelete;
			}
		}

		private class ForeignKeyFromParentClass : ForeignKeyDirection
		{
			public override bool CascadeNow(CascadePoint cascadePoint)
			{
				return cascadePoint != CascadePoint.AfterInsertBeforeDelete;
			}
		}

		/// <summary>
		/// Should we cascade at this cascade point?
		/// </summary>
		public abstract bool CascadeNow(CascadePoint cascadePoint);

		/// <summary>
		/// A foreign key from child to parent
		/// </summary>
		public static ForeignKeyDirection ForeignKeyToParent = new ForeignKeyToParentClass();

		/// <summary>
		/// A foreign key from parent to child
		/// </summary>
		public static ForeignKeyDirection ForeignKeyFromParent = new ForeignKeyFromParentClass();
	}
}