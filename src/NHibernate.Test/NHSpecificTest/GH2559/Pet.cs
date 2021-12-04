using System;

namespace NHibernate.Test.NHSpecificTest.GH2559
{
	public class Pet
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual Child Owner { get; set; }
	}
}
