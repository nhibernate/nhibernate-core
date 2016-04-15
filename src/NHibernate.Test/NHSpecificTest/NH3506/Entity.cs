using System;

namespace NHibernate.Test.NHSpecificTest.NH3506
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

        public virtual DateTime? Deleted { get; set; }
	}

    class Person : Entity
    {
        public virtual Employer Employer { get; set; }
    }

    class Employer : Entity
    {
    }
}