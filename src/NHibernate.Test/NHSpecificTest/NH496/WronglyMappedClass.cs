using System;

namespace NHibernate.Test.NHSpecificTest.NH496
{
	public class WronglyMappedClass
	{
		private int id;
		private int someInt;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public int SomeInt
		{
			get { return someInt; }
			set { someInt = value; }
		}
	}
}