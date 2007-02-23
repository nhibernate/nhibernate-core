using System;
using NHibernate.Type;

namespace NHibernate.Test.TypesTest
{
	public class EnumStringClass
	{
		private int _id;
		private SampleEnum _enumValue;

		public EnumStringClass()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public SampleEnum EnumValue
		{
			get { return _enumValue; }
			set { _enumValue = value; }
		}
	}

	public enum SampleEnum
	{
		On,
		Off,
		Dimmed
	}

	public class SampleEnumType : EnumStringType
	{
		public SampleEnumType()
			: base(typeof(SampleEnum), 10)
		{
		}
	}
}