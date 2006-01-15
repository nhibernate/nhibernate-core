using System;

namespace NHibernate.Test.NHSpecificTest.NH440
{
	public class Fruit
	{
		private int id;
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		private Apple theApple;
		public Apple TheApple
		{
			get { return theApple; }
			set { theApple = value; }
		}
	}
}
