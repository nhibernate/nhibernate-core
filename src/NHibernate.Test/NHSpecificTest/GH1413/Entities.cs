using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1413
{
	public class EntityChild
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class EntityParent
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<EntityChild> Children { get; set; } = new List<EntityChild>();
	}
}
