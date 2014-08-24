using System;

namespace NHibernate.Test.NHSpecificTest.NH2347
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual int Quantity { get; set; }
	}
}