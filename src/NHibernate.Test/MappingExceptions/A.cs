using System;

namespace NHibernate.Test.MappingExceptions
{
	/// <summary>
	/// Summary description for A.
	/// </summary>
	public class A
	{
		private int _id;
		private string _name;

		private A()
		{
			
		}

		public int Id 
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name 
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}
