using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1796Generic
{
	public class Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IDictionary<string, object> DynProps { get; set; }
	}
}
