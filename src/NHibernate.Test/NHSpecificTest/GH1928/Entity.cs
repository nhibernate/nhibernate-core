using System;

namespace NHibernate.Test.NHSpecificTest.GH1928
{
	class EntityX
	{
		public virtual Guid Id { get; set; }
		public virtual EntityY Y { get; set; }
	}

	class EntityY
	{
		public virtual Guid Id { get; set; }
		public virtual EntityZ Z { get; set; }
	}

	class EntityZ
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
