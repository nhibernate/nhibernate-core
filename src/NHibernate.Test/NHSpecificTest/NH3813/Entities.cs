using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3813
{
	public class AssociationTable
	{
		public virtual FirstTable FirstTable { get; set; }
		public virtual OtherTable OtherTable { get; set; }
		public virtual string Name { get; set; }

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class FirstTable
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public virtual IList<AssociationTable> AssociationTableCollection { get; set; } = new List<AssociationTable>();
	}

	public class OtherTable
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}
