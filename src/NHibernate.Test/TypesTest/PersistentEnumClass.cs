using System;

namespace NHibernate.Test.TypesTest
{
	public class PersistentEnumHolder
	{
		public PersistentEnumHolder(A a, B b)
		{
		}
	}

	public class PersistentEnumClass
	{
		private int _id;
		private A _a;
		private B _b;

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

		public PersistentEnumClass(int id, A a, B b)
		{
			_id = id;
			_a = a;
			_b = b;
		}
	}
}