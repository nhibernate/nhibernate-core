using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH1796
{
	public class Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IDictionary DynProps { get; set; }
	}
}