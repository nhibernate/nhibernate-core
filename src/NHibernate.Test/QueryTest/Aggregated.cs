using System;

namespace NHibernate.Test.QueryTest
{
	public class Aggregated
	{
		private int id;
		private byte aByte;
		private short aShort;
		private int anInt;
		private long aLong;
		private float aFloat;
		private double aDouble;
		private decimal aDecimal;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual byte AByte
		{
			get { return aByte; }
			set { aByte = value; }
		}

		public virtual short AShort
		{
			get { return aShort; }
			set { aShort = value; }
		}

		public virtual int AnInt
		{
			get { return anInt; }
			set { anInt = value; }
		}

		public virtual long ALong
		{
			get { return aLong; }
			set { aLong = value; }
		}

		public virtual float AFloat
		{
			get { return aFloat; }
			set { aFloat = value; }
		}

		public virtual double ADouble
		{
			get { return aDouble; }
			set { aDouble = value; }
		}

		public virtual decimal ADecimal
		{
			get { return aDecimal; }
			set { aDecimal = value; }
		}
	}
}