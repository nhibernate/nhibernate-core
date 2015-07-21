using System;
using log4net;

namespace NHibernate.Test.NHSpecificTest.NH1810
{
	public class Child : IComparable<Child>
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(Fixture));

		int id;
		int age;
		Parent parent;

		public virtual int Id
		{
			get { return id; }
		}

		public virtual int Age
		{
			get { return age; }
			set { age = value; }
		}

		public virtual Parent Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public virtual bool Equals(Child other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Age == Age && Equals(other.Parent, Parent);
		}

		public virtual int CompareTo(Child other)
		{
			return Id.CompareTo(other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Child)) return false;
			return Equals((Child) obj);
		}

		int? hashCode;

		public override int GetHashCode()
		{
			Log.Debug("Child.GetHashCode()");

			if (!hashCode.HasValue)
				unchecked
				{
					hashCode = (Age*397) ^ (Parent != null ? Parent.GetHashCode() : 0);
				}

			return hashCode.Value;
		}
	}
}
