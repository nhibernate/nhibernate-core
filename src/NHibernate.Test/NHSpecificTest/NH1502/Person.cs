using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH1502
{
	public class Person
	{
		private int id;
		private int iq;
		private string name;
		private IList pets;
		private int shoeSize;

		public Person()
		{
			pets = new ArrayList();
		}

		public Person(string name, int iq, int shoeSize)
		{
			this.name = name;
			this.iq = iq;
			this.shoeSize = shoeSize;
			pets = new ArrayList();
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual int IQ
		{
			get { return iq; }
			set { iq = value; }
		}

		public virtual int ShoeSize
		{
			get { return shoeSize; }
			set { shoeSize = value; }
		}

	}
}