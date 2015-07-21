using System;

namespace NHibernate.Test.NHSpecificTest.NH3401
{
	class Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual bool YesNo { get; set; }
	}
}