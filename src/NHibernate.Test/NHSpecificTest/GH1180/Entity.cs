using System;

namespace NHibernate.Test.NHSpecificTest.GH1180
{
	internal class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual decimal Amount { get; set; }
	}
}
