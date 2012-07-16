using System;

namespace NHibernate.Test.NHSpecificTest.NH3221
{
	public class Todo : IEquatable<Todo>
	{
		protected Todo() { }

		public Todo(Person personNeedsTodo)
		{
			Person = personNeedsTodo;
		}

		public virtual string Name { get; set; }

		public virtual Person Person { get; protected set; }

		public virtual bool Equals(Todo other)
		{
			if (other == null) return false;
			return Name.Equals(other.Name) && Person.Equals(other.Person);
		}

		public static bool operator ==(Todo left, Todo right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Todo left, Todo right)
		{
			return !Equals(left, right);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Todo)) return false;
			return Equals((Todo) obj);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}
	}
}