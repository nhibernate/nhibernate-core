using System.Collections.Generic;

namespace NHibernate.Test.LazyOneToOne
{
	public class Employee
	{
		protected Employee()
		{
			Employments = new List<Employment>();
		}

		public Employee(Person person)
			: this()
		{
			Person = person;
			PersonName = person.Name;
			Person.Employee = this;
		}

		public virtual string PersonName { get; protected set; }

		public virtual Person Person { get; protected set; }

		public virtual ICollection<Employment> Employments { get; set; }
	}
}