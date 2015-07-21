using System;
using System.Collections;


namespace NHibernate.Test.NHSpecificTest.NH1280
{
	public class Person
	{
		private int id;
		private int iq;
		private int shoeSize;
		private string name;

		public Person()
		{

		}

		public Person(string name, int iq, int shoeSize)
		{
			this.name = name;
			this.iq = iq;
			this.shoeSize = shoeSize;
		}

		virtual public int Id
		{
			get { return id; }
			set { id = value; }
		}

		virtual public string Name
		{
			get { return name; }
			set { name = value; }
		}

		virtual public int IQ
		{
			get { return iq; }
			set { iq = value; }
		}

		virtual public int ShoeSize
		{
			get { return shoeSize; }
			set { shoeSize = value; }
		}
	}
}