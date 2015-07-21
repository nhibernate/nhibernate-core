using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3324
{
	internal class ChildEntity
	{
		public virtual int? Id { get; set; }
		public virtual string Name { get; set; }
	}
}