using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1447
{
	public class Person
	{
		public Person()
		{

		}

		public Person(string name, bool isPerfect)
		{
			this.Name = name;
			this.WantsNewsletter = isPerfect;
		}

		public virtual int Id
		{ get;
			set;
		}

		public virtual string Name
		{
			get;
			set;
		}
		public virtual bool WantsNewsletter
		{
			get;
			set;
		}

	}

}
