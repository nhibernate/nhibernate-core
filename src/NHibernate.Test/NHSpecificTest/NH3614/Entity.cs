using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3614
{
	public class Entity
	{
		public virtual int Id { get; protected set; }
		public virtual IList<string> SomeStrings { get; set; }
	}
}
