using System;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH440
{
	public class Apple
	{
		private int id;
		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	
		private Fruit theFruit;
		public Fruit TheFruit
		{
			get { return theFruit; }
			set { theFruit = value; }
		}
	}
}
