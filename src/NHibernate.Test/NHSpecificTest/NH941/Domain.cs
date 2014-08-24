using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH941
{
	public class MyClass
	{
		public virtual int Id { get; set; }
		public virtual IList<Related> Relateds { get; set; }
	}

	public class Related
	{
		public virtual int Id { get; set; }
	}
}