using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.NHSpecificTest.GH2414
{
	class EntityWithGuidKey
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
