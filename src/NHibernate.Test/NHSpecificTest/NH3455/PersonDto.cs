using System;

namespace NHibernate.Test.NHSpecificTest.NH3455
{
	class PersonDto
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int Age { get; set; }
		public virtual int Weight { get; set; }
		public virtual Address Address { get; set; }
	}
}