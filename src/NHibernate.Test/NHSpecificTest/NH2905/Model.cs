using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2905
{
	[Serializable]
	public class Entity1
	{
		public virtual Guid Id { get; set; }
		public virtual Entity2 Entity2 { get; set; }
	}

	[Serializable]
	public class Entity2
	{
		public Entity2()
		{
			Entity3s = new List<Entity3>();
		}

		public virtual Guid Id { get; set; }
		public virtual ICollection<Entity3> Entity3s { get; set; }
	}

	[Serializable]
	public class Entity3
	{
		public virtual Guid Id { get; set; }
		public virtual Entity2 Entity2 { get; set; }
	}
}