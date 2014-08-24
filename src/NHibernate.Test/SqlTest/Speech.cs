using System;

namespace NHibernate.Test.SqlTest
{
	public class Speech
	{
		private int id;
		private string name;
		private double length;

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

		public virtual double Length
		{
			get { return length; }
			set { length = value; }
		}
	}
}