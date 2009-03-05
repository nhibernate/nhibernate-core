using NHibernate.Engine;

namespace NHibernate.Type
{
	using System;

	/// <summary>
	/// Represents directionality of the foreign key constraint
	/// </summary>
	[Serializable]
	public abstract class ForeignKeyDirection
	{
		/// <summary></summary>
		protected ForeignKeyDirection()
		{
		}
		[Serializable]
		private class ForeignKeyToParentClass : ForeignKeyDirection
		{
			public override bool CascadeNow(CascadePoint cascadePoint)
			{
				return cascadePoint != CascadePoint.BeforeInsertAfterDelete;
			}
		}
		[Serializable]
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