using System;

namespace NHibernate.Test.NHSpecificTest.NH2033
{
	public class Customer : IEquatable<Customer>
	{
		public virtual int AssignedId { get; set; }
		public virtual string Name { get; set; }

		public virtual bool Equals(Customer other)
		{
			// I know that you're not supposed to implement Equals using the Id, but since this is
			// an assigned Id, not auto-generated, it should be safe.
			return ReferenceEquals(this, other)
				   || (other != null && AssignedId == other.AssignedId);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Customer);
		}

		public override int GetHashCode()
		{
			return AssignedId.GetHashCode();
		}
	}
}