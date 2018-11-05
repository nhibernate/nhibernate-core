using System;

namespace NHibernate.Test.NHSpecificTest.NH3898
{
	public class Employee
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual int PromotionCount { get; set; }
	}
}
