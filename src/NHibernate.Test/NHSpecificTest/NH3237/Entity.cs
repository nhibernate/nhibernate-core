using System;

namespace NHibernate.Test.NHSpecificTest.NH3237
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual DateTimeOffset DateTimeOffsetValue { get; set; }
		public virtual TestEnum EnumValue { get; set; }
		public virtual int IntValue { get; set; }
		public virtual long LongValue { get; set; }
		public virtual decimal DecimalValue { get; set; }
		public virtual double DoubleValue { get; set; }
		public virtual float FloatValue { get; set; }
		public virtual string StringValue { get; set; }
		public virtual DateTime DateTimeValue { get; set; }
	}

	public enum TestEnum
	{
		Zero = 0,
		One = 1,
		Two = 2
	}
}
