namespace NHibernate.Test.NHSpecificTest.NH3046
{
	using System;
	using System.Collections.Generic;

	public class Parent
	{
		public Parent()
		{
			Childs = new List<Child>();
		}

		public virtual long Id { get; set; }
		public virtual IList<Child> Childs { get; set; }
	}

	public class Child
	{
		public virtual long Id { get; set; }
	}
}
