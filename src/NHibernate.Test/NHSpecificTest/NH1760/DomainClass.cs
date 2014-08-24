using System;

namespace NHibernate.Test.NHSpecificTest.NH1760
{
	public class Customer
	{
		public virtual Int32 Id { get; set; }
		public virtual String Name { get; set; }
	}

	public class TestClass
	{
		public virtual TestClassId Id { get; set; }
		public virtual String Value { get; set; }
	}

	public class TestClassId
	{
		public virtual Customer Customer { get; set; }
		public virtual Int32 SomeInt { get; set; }

		public virtual bool Equals(TestClassId other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Customer, Customer) && other.SomeInt == SomeInt;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof (TestClassId))
			{
				return false;
			}
			return Equals((TestClassId) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Customer != null ? Customer.GetHashCode() : 0) * 397) ^ SomeInt;
			}
		}
	}
}