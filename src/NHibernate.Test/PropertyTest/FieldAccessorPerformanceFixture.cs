using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	[TestFixture, Explicit]
	public class FieldAccessorPerformanceFixture : AccessorPerformanceFixture<FieldAccessorPerformanceFixture.A>
	{
		protected override string AccessorType => "field";

		protected override List<string> PropertyNames => new List<string>
		{
			"_id",
			"_name",
			"_date",
			"_decimal",
		};

		protected override object[] GetValues()
		{
			return new object[] { 5, "name", DateTime.MaxValue, 1.5m };
		}

		public class A
		{
			private int _id = 5;
			private string _name = string.Empty;
			private DateTime _date = DateTime.MinValue;
			private decimal? _decimal = decimal.Zero;

			public int Id => _id;

			public string Name => _name;

			public DateTime Date => _date;

			public decimal? Decimal => _decimal;
		}
	}
}
