using System;
using System.Collections.Generic;

namespace NHibernate.Test.Insertordering
{
	public class AddressM2M
	{
		public virtual Guid Id { get; set; }

		public virtual ISet<PersonM2M> Persons { get; set; } = new HashSet<PersonM2M>();

		public virtual void AddPerson(PersonM2M person)
		{
			Persons.Add(person);
			person.Addresses.Add(this);
		}
	}

	public class PersonM2M
	{
		public virtual Guid Id { get; set; }

		public virtual ISet<AddressM2M> Addresses { get; set; } = new HashSet<AddressM2M>();
	}

	public class AddressM2O
	{
		public virtual Guid Id { get; set; }

		public virtual ISet<PersonM2O> Persons { get; set; } = new HashSet<PersonM2O>();

		public virtual void AddPerson(PersonM2O person)
		{
			Persons.Add(person);
			person.Address = this;
		}
	}

	public class PersonM2O
	{
		public virtual Guid Id { get; set; }

		public virtual AddressM2O Address { get; set; }
	}

	public class AddressO2O
	{
		public virtual Guid Id { get; set; }

		public virtual PersonO2O Person { get; set; }

		public virtual void SetPerson(PersonO2O person)
		{
			Person = person;
			person.Address = this;
		}
	}

	public class PersonO2O
	{
		public virtual Guid Id { get; set; }

		public virtual AddressO2O Address { get; set; }
	}

	public class AddressTrueO2O
	{
		public virtual Guid Id { get; set; }

		public virtual PersonTrueO2O Person { get; set; }

		public virtual void SetPerson(PersonTrueO2O person)
		{
			Person = person;
			person.Address = this;
		}
	}

	public class PersonTrueO2O
	{
		public virtual Guid Id { get; set; }

		public virtual AddressTrueO2O Address { get; set; }
	}

	public class AddressJI
	{
		public virtual Guid Id { get; set; }
	}

	public class PersonJI
	{
		public virtual Guid Id { get; set; }

		public virtual ISet<AddressJI> Addresses { get; set; } = new HashSet<AddressJI>();

		public virtual void AddAddress(AddressJI address)
		{
			Addresses.Add(address);
		}
	}

	public class SpecialPersonJI : PersonJI
	{
		public virtual string Special { get; set; }
	}

	public class AddressJim
	{
		public virtual Guid Id { get; set; }
	}

	public class OfficeJim
	{
		public virtual Guid Id { get; set; }
	}

	public class PersonJim
	{
		public virtual Guid Id { get; set; }
	}

	public class SpecialPersonJim : PersonJim
	{
		public virtual string Special { get; set; }

		public virtual ISet<AddressJim> Addresses { get; set; } = new HashSet<AddressJim>();

		public virtual void AddAddress(AddressJim address)
		{
			Addresses.Add(address);
		}
	}

	public class AnotherPersonJim : PersonJim
	{
		public virtual bool Working { get; set; }
		public virtual OfficeJim Office { get; set; }
	}

	public class PresidentJim : SpecialPersonJim
	{
		public virtual decimal Salary { get; set; }
	}

	public class AddressSti
	{
		public virtual Guid Id { get; set; }
	}

	public class PersonSti
	{
		public virtual Guid Id { get; set; }

		public virtual ISet<AddressSti> Addresses { get; set; } = new HashSet<AddressSti>();

		public virtual void AddAddress(AddressSti address)
		{
			Addresses.Add(address);
		}
	}

	public class SpecialPersonSti : PersonSti
	{
		public virtual string Special { get; set; }
	}

	public class AddressTpc
	{
		public virtual Guid Id { get; set; }
	}

	public class PersonTpc
	{
		public virtual Guid Id { get; set; }

		public virtual ISet<AddressTpc> Addresses { get; set; } = new HashSet<AddressTpc>();

		public virtual void AddAddress(AddressTpc address)
		{
			Addresses.Add(address);
		}
	}

	public class AddressUO2O
	{
		public virtual Guid Id { get; set; }

		public virtual PersonUO2O Person { get; set; }

		public virtual void SetPerson(PersonUO2O person)
		{
			Person = person;
		}
	}

	public class PersonUO2O
	{
		public virtual Guid Id { get; set; }
	}

	public class SpecialPersonTpc : PersonTpc
	{
		public virtual string Special { get; set; }
	}

	public class Task
	{
		public virtual Guid Id { get; set; }
		public virtual ISet<Category> Categories { get; set; } = new HashSet<Category>();
	}

	public enum Category
	{
		A,
		B
	}
}
