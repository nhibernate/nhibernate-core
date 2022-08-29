using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3952
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid? ParentId { get; set; }
		public virtual ISet<Entity> Children { get; set; }
		public virtual string[] Hobbies { get; set; }
	}
}
