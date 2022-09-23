using System;

namespace NHibernate.Test.NHSpecificTest.GH3005
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual TimeSpan Duration { get; set; }
	}
}
