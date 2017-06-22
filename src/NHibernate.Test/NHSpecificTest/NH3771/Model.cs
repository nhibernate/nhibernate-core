using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3771
{
	public class Singer
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int Version { get; set; }
	}
}
