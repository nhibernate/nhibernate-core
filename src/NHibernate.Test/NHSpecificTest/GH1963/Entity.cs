using System;

namespace NHibernate.Test.NHSpecificTest.GH1963
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual bool Flag { get; set; }
	}
}
