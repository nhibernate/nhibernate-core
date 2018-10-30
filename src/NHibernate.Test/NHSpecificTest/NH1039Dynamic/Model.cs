using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1039Dynamic
{
	public class Person
	{
		public Person()
		{
		}

		public Person(string id)
		{
			ID = id;
		}

		public virtual string ID { get; set; }

		public virtual string Name { get; set; }

		public virtual dynamic Properties { get; set; } = new ExpandoObject();
	}
}
