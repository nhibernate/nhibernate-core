using System;

namespace NHibernate.Test.NHSpecificTest.NH369
{
	public class KeyManyToOneClass
	{
		private int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}
}