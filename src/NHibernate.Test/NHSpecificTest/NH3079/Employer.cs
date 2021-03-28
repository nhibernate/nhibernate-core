using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3079
{
	public class Employer
	{
		public virtual EmployerCpId CpId { get; set; }

		public virtual string Name { get; set; }

		public virtual ICollection<Employment> EmploymentList { get; set; }
	}
}
