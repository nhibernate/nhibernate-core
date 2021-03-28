using System;

namespace NHibernate.Test.MultiTenancy
{
	[Serializable]
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
