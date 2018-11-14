using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1754
{
	class Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Entity> Children { get; set; } = new HashSet<Entity>();
	}
}
