using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1579
{
	public abstract class Fruit : Entity
	{
		public Fruit(Entity container)
		{
			if (container == null)
				throw new ArgumentNullException("container");
			Container = container;
		}

		protected Fruit()
		{
		}

		public Entity Container { get; private set; }

	}
}
