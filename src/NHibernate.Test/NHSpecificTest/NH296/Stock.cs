using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH296
{
	public class Stock : Product
	{
		private int _property;

		public int Property
		{
			get { return _property; }
			set { _property = value; }
		}
	}
}
