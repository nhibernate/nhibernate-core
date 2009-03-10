using NHibernate.Engine;

namespace NHibernate.Type
{
	using System;
	using System.Runtime.Serialization;

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
				return cascadePoint != CascadePoint.BeforeInsertAfterDelete;}
			public override bool Equals(object obj)
			{
				return obj is ForeignKeyToParentClass; ;
			}
		}
		[Serializable]
		private class ForeignKeyFromParentClass : ForeignKeyDirection
		{
			public override bool CascadeNow(CascadePoint cascadePoint)
			{
				return cascadePoint != CascadePoint.AfterInsertBeforeDelete;
			}
			public override bool Equals(object obj)
			{
				return obj is ForeignKeyFromParentClass; ;
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