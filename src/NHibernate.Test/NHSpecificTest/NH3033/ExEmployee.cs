using System;

namespace NHibernate.Test.NHSpecificTest.NH3033
{
	public class ExEmployee
	{
		protected ExEmployee() : this(string.Empty)
		{
		}

		public ExEmployee(string name)
		{
			Name = name;
		}

		public virtual Guid Id { get; protected set; }

		public virtual string Name { get; protected set; }

		public virtual Company Company { get; protected set; }

		public virtual void HasWorkedIn(Company company)
		{
			Company = company;
		}
	}
}
