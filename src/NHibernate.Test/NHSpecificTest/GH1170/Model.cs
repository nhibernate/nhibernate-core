using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1170
{
	public class Parent
	{
		public virtual ICollection<ChildComponent> ChildComponents { get; set; } = new List<ChildComponent>();
		public virtual Guid Id { get; set; }
	}

	public class ChildComponent
	{
		public virtual bool SomeBool { get; set; }
		public virtual string SomeString { get; set; }

		protected bool Equals(ChildComponent other)
		{
			return SomeBool == other.SomeBool && string.Equals(SomeString, other.SomeString);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is ChildComponent other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (SomeBool.GetHashCode() * 397) ^ (SomeString != null ? SomeString.GetHashCode() : 0);
			}
		}
	}
}
