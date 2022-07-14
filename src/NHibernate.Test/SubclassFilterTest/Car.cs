using System.Collections.Generic;

namespace NHibernate.Test.SubclassFilterTest
{
	public class Car
	{
		public virtual int Id { get; set; }
		
		public virtual string LicensePlate { get; set; }
		
		public virtual IList<Employee> Drivers { get; set; }
	}
}
