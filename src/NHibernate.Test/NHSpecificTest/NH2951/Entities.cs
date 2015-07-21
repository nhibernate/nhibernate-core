using System;

namespace NHibernate.Test.NHSpecificTest.NH2951
{
	class Entity
	{
		public virtual Guid Id { get; set; }
	}

    class Customer : Entity
    {
        public virtual string Name { get; set; }
    }

    class Invoice
    {
        public virtual Guid Id { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual decimal Amount { get; set; }
    }
}