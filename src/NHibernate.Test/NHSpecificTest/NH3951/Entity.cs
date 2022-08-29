using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3951
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid? RelatedId { get; set; }

		public virtual ISet<Entity> Related { get; set; }
	}
}
