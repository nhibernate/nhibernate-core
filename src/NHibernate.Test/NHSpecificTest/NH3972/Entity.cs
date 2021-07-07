using System;

namespace NHibernate.Test.NHSpecificTest.NH3972
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual int Version { get; set; }
	}
}
