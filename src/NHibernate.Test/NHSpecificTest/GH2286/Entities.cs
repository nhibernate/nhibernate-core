using System;

namespace NHibernate.Test.NHSpecificTest.GH2286
{
	class Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}

	class Customer : Entity
	{
	}

	class IndividualCustomer : Entity
	{
	}
}
