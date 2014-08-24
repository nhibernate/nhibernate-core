using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1478
{
	public class Person
	{
		public Person()
		{

		}

		public Person(string name, string bio)
		{
			this.Name = name;
			this.Biography = bio;
		}

		public virtual int Id
		{
			get;
			set;
		}

		public virtual string Name
		{
			get;
			set;
		}
		public virtual string Biography
		{
			get;
			set;
		}

	}
}
