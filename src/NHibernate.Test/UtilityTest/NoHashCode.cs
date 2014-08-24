using System;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// The IdentityMap should not ever call the GetHashCode() because that
	/// will have side effects on Collections/Entities.
	/// </summary>
	[Serializable]
	public class NoHashCode
	{
		public override int GetHashCode()
		{
			throw new NotImplementedException("This method should not get called during test");
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
	}
}