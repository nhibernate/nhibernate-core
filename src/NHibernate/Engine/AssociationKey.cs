using System;

namespace NHibernate.Engine
{
	/// <summary> 
	/// Identifies a named association belonging to a particular
	/// entity instance. Used to record the fact that an association
	/// is null during loading. 
	/// </summary>
	[Serializable]
	internal sealed class AssociationKey
	{
		private readonly EntityKey ownerKey;
		private readonly string propertyName;
		private readonly int hashCode;

		public AssociationKey(EntityKey ownerKey, string propertyName)
		{
			this.ownerKey = ownerKey;
			this.propertyName = propertyName;
			hashCode = ownerKey.GetHashCode() ^ propertyName.GetHashCode() ^ ownerKey.EntityName.GetHashCode();
		}

		public override bool Equals(object that)
		{
			// NH : Different behavior for NH-1584
			if (this == that)
			{
				return true;
			}

			var key = that as AssociationKey;
			if (key == null)
			{
				return false;
			}
			return key.propertyName.Equals(propertyName) && key.ownerKey.Equals(ownerKey)
			       && key.ownerKey.EntityName.Equals(ownerKey.EntityName);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}