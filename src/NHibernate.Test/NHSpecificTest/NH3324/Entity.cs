using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3324
{
	internal class Entity
	{
		public Entity()
		{
			Children = new List<ChildEntity>();
		}

		public virtual int? Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<ChildEntity> Children { get; set; }
	}
}