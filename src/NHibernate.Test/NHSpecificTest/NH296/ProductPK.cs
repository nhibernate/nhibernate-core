using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH296
{
	public class ProductPK
	{
		int _type;
		int _number;

		public int Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public int Number
		{
			get { return _number; }
			set { _number = value; }
		}
	}
}
