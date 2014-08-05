using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3472
{

	public class Cat
	{
		private int id;
		private IList<Cat> children = new List<Cat>();

		public virtual string Color { get; set; }
		public int Age { get; set; }

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public IList<Cat> Children
		{
			get { return children; }
			set { children = value; }
		}
	}
}