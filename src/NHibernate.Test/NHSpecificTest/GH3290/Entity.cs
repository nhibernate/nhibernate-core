using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3290
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Entity> Parents { get; set; }
		public virtual ISet<Entity> Children { get; set; }
	}
}
