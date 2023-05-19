using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3218
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Child> List { get; set; }
	}

	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual CompositeKey Component { get; set; }
	}
	
	public class CompositeKey
	{
		public int Id1 { get; set; }
		public int Id2 { get; set; }

		public override bool Equals(object obj)
		{
			var key = obj as CompositeKey;
			return key != null
			       && Id1 == key.Id1
			       && Id2 == key.Id2;
		}

		public override int GetHashCode()
		{
			var hashCode = -1596524975;
			hashCode = hashCode * -1521134295 + Id1.GetHashCode();
			hashCode = hashCode * -1521134295 + Id2.GetHashCode();
			return hashCode;
		}
	}
}
