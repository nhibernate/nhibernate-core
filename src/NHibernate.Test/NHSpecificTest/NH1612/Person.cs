using System;

namespace NHibernate.Test.NHSpecificTest.NH1612
{
	public class Person
	{
		public virtual Guid PersonId { get; protected set; }
		public virtual string Name { get; protected set; }
		public virtual int Version { get; protected set; }

		protected Person() {}

		public Person(string name)
		{
			PersonId = Guid.NewGuid();
			Name = name;
		}

		public static bool operator ==(Person left, Person right)
		{
			if (ReferenceEquals(left, right))
			{
				return true;
			}
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
			{
				return true;
			}
			return left.PersonId == right.PersonId;
		}

		public static bool operator !=(Person left, Person right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			return this == (Person) obj;
		}

		public override int GetHashCode()
		{
			return PersonId.GetHashCode();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}