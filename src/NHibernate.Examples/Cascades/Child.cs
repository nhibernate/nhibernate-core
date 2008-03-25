using System;

namespace NHibernate.Examples.Cascades
{
	/// <summary>
	/// Summary description for Child.
	/// </summary>
	public class Child
	{
		private int id;
		private string name;
		private Parent singleParent;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Parent SingleParent
		{
			get { return singleParent; }
			set { singleParent = value; }
		}
	}
}