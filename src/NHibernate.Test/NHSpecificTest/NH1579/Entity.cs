using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1579
{
	public abstract class Entity
	{
		public Entity()
		{
		}

		public Guid ID { get; private set; }
	}
}
