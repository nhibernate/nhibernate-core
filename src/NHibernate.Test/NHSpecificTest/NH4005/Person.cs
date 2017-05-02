using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH4005
{
	class Person
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IDictionary Attributes { get; set; }
	}
}