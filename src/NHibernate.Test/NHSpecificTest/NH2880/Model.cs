using System;

namespace NHibernate.Test.NHSpecificTest.NH2880
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
		public virtual Guid Id { get; set; }
		public virtual string Text { get; set; }
	}
}
