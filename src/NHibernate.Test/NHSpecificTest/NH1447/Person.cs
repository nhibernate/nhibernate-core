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

		public Person(string name, bool wantsNewsLetter)
		{
			this.Name = name;
			this.WantsNewsletter = wantsNewsLetter;
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
