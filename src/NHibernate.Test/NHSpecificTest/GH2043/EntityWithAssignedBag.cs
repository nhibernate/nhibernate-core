using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH2043
{
	public class EntityWithAssignedBag
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<EntityAssigned> Children { get; set; } = new List<EntityAssigned>();
	}
}