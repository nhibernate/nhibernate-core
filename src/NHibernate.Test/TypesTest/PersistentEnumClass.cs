using System;
using System.Collections;

namespace NHibernate.Test.TypesTest
{
	public class PersistentEnumClass
	{
		int _id;
		A _a;
		B _b;
		public int Id
		{

			get { return _id; }
			set { _id = value; }
		}

		public A A
		{
			get { return _a; }
			set { _a = value; }
		}

		public B B
		{
			get { return _b; }
			set { _b = value; }
		}

		public PersistentEnumClass()
		{
		}

		public PersistentEnumClass( A a, B b )
		{
			_a = a;
			_b = b;
		}
	}
}
