using System;

namespace NHibernate.Test.GenericTest.EnumGeneric
{
	public class A
	{
		private Int64 id;
		private B? nullableValue;

		public virtual Int64 Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual B? NullableValue
		{
			get { return nullableValue; }
			set { nullableValue = value; }
		}
	}
}
