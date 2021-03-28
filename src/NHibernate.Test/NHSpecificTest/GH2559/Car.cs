using System;

namespace NHibernate.Test.NHSpecificTest.GH2559
{
	public class Car
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual Person Owner { get; set; }
	}
}
