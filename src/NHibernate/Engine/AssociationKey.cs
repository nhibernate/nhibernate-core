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

		public AssociationKey(EntityKey ownerKey, string propertyName)
		{
			this.ownerKey = ownerKey;
			this.propertyName = propertyName;
		}

		public override bool Equals(object that)
		{
			if(this==that) return true;

			AssociationKey key = that as AssociationKey;
			if(key==null) return false;
			return key.propertyName.Equals(propertyName) && key.ownerKey.Equals(ownerKey);
		}

		public override int GetHashCode()
		{
			return ownerKey.GetHashCode() ^ propertyName.GetHashCode();
		}
	}
}
