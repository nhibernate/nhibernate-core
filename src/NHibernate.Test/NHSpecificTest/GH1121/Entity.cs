using System;

namespace NHibernate.Test.NHSpecificTest.GH1121
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual long Color { get; set; }
	}
	public enum Colors
	{
		Red = 1,
		Green = 2,
		Blue = 3,
	}
}
