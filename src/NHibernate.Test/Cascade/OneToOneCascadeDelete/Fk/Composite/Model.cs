using System;

namespace NHibernate.Test.Cascade.OneToOneCascadeDelete.Fk.Composite
{
	public class Employee
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual EmployeeInfo Info { get; set; }

		public Employee()
		{

		}
	}

	public class EmployeeInfo
	{
		public class Identifier
		{
			public virtual long CompanyId { get; set; }
			public virtual long PersonId { get; set; }

			public Identifier()
			{

			}

			public Identifier(long companyId, long personId)
			{
				this.CompanyId = companyId;
				this.PersonId = personId;
			}


			public override bool Equals(Object o)
			{
				if (this == o)
				{
					return true;
				}

				var t = this.GetType();
				var u = o.GetType();


				if (o == null || !t.IsAssignableFrom(u) || !u.IsAssignableFrom(t))
				{
					return false;
				}

				var id = o as Identifier;

				return CompanyId.Equals(id.CompanyId)
						&& PersonId.Equals(id.PersonId);

			}

			public override int GetHashCode()
			{
				return (31 * CompanyId.GetHashCode()) + PersonId.GetHashCode();
			}
		}

		public virtual Identifier Id { get; set; }
		public virtual Employee Employee { get; set; }

		public EmployeeInfo()
		{

		}

		public EmployeeInfo(long companyId, long personId)
		{
			this.Id = new Identifier(companyId, personId);
		}

		public EmployeeInfo(Identifier id)
		{
			this.Id = id;
		}
	}
}
