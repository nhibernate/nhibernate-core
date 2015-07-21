using System;

namespace NHibernate.Test.NHSpecificTest.NH3377
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Age { get; set; }
		public virtual string Solde { get; set; }
	}
}