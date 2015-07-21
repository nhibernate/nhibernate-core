using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH901
{
	public class Person
	{
		protected Person() { }

		public Person(string name) { this._Name = name; }

		private string _Name;
		public virtual string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		private Address _Address;
		public virtual Address Address
		{
			get { return _Address; }
			set { _Address = value; }
		}
	}

	public struct Address
	{
		private string _Street;
		public string Street
		{
			get { return _Street; }
			set { _Street = value; }
		}

		private string _City;
		public string City
		{
			get { return _City; }
			set { _City = value; }
		}
	}
}
