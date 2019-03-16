using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH2053
{
	public class Entity
	{
		public virtual int Id { get; protected set; }

		public virtual int Status { get; set; }

		public virtual string Description { get; set; }
	}
}
