using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3864
{
	public class Person
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Person Parent { get; set; }

		public virtual ISet<Person> Children { get; set; }
	}
}