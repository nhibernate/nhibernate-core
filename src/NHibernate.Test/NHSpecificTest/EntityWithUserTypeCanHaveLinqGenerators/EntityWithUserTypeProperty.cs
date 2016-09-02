using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	public class EntityWithUserTypeProperty
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IExample Example { get; set; }
	}


}
