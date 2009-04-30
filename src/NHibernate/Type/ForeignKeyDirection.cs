using System;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// Represents directionality of the foreign key constraint
	/// </summary>
	[Serializable]
	public abstract class ForeignKeyDirection
	{
		/// <summary>
		/// A foreign key from parent to child
		/// </summary>
		public static ForeignKeyDirection ForeignKeyFromParent = new ForeignKeyFromParentClass();

		/// <summary>
		/// A foreign key from child to parent
		/// </summary>
		public static ForeignKeyDirection ForeignKeyToParent = new ForeignKeyToParentClass();

		/// <summary>
		/// Should we cascade at this cascade point?
		/// </summary>
		public abstract bool CascadeNow(CascadePoint cascadePoint);

		#region Nested type: ForeignKeyFromParentClass

		[Serializable]
		private class ForeignKeyFromParentClass : ForeignKeyDirection
		{
			public override bool CascadeNow(CascadePoint cascadePoint)
			{
				return cascadePoint != CascadePoint.AfterInsertBeforeDelete;
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as ForeignKeyFromParentClass);
			}

			public bool Equals(ForeignKeyFromParentClass other)
			{
				return !ReferenceEquals(null, other);
			}

			public override int GetHashCode()
			{
				return 37;
			}
		}

		#endregion

		#region Nested type: ForeignKeyToParentClass

		[Serializable]
		private class ForeignKeyToParentClass : ForeignKeyDirection
		{
			public override bool CascadeNow(CascadePoint cascadePoint)
			{
				return cascadePoint != CascadePoint.BeforeInsertAfterDelete;
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as ForeignKeyToParentClass);
			}

			public bool Equals(ForeignKeyToParentClass other)
			{
				return !ReferenceEquals(null, other);
			}

			public override int GetHashCode()
			{
				return 17;
			}
		}

		#endregion
	}
}