using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH883
{
	public class Cat
	{
		private int id;
		private IList<Cat> children = new List<Cat>();
		private Cat mother;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public Cat Mother
		{
			get { return mother; }
			set { mother = value; }
		}

		public IList<Cat> Children
		{
			get { return children; }
			set { children = value; }
		}
	}
}