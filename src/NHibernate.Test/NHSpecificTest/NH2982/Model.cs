using System;
using System.Collections.Generic;
using System.Globalization;

namespace NHibernate.Test.NHSpecificTest.NH2982
{
	public class Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public override string ToString()
		{
			throw new InvalidOperationException(".ToString() is called which can result in lazy loading side effects.");
		}
	}
}
