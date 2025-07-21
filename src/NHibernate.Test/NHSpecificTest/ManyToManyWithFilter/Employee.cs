using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.ManyToManyWithFilter
{
	public class Employee : BaseClass
	{
		public virtual ISet<Department> Departments { get; set; } = new HashSet<Department>();
	}
}
