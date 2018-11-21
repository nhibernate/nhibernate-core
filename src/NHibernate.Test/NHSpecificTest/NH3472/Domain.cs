using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3472
{
	public class Cat
	{
		public virtual int Id { get; set; }

		public virtual string Color { get; set; }
		public virtual int Age { get; set; }

		public virtual IList<Cat> Children { get; set; } = new List<Cat>();
	}
}
