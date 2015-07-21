using System;

namespace NHibernate.Test.NHSpecificTest.NH3221
{
	public class Stuff : IEquatable<Stuff>
	{
		protected Stuff() { }

		public Stuff(Person personNeedsTodo)
		{
			Person = personNeedsTodo;
		}

		public virtual string Name { get; set; }

		public virtual bool Equals(Stuff other)
		{
			if (other == null) return false;
			return Name.Equals(other.Name) && Person.Equals(other.Person);
		}

		public static bool operator ==(Stuff left, Stuff right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Stuff left, Stuff right)
		{
			return !Equals(left, right);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Stuff)) return false;
			return Equals((Stuff)obj);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}

		public virtual Person Person { get; protected set; }
	}
}