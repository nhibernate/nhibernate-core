using System;

namespace NHibernate.Test.NHSpecificTest.NH464
{
	public class Container
	{
		private int id;
		private Component component;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public Component Component
		{
			get { return component; }
			set { component = value; }
		}
	}
}
