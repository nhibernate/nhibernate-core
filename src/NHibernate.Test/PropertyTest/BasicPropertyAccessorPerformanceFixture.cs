using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	[TestFixture, Explicit]
	public class BasicPropertyAccessorPerformanceFixture : AccessorPerformanceFixture<BasicPropertyAccessorPerformanceFixture.A>
	{
		protected override string AccessorType => "property";

		protected override List<string> PropertyNames => new List<string>
		{
			nameof(A.Id),
			nameof(A.Name),
			nameof(A.Date),
			nameof(A.Decimal)
		};

		protected override object[] GetValues()
		{
			return new object[] { 5, "name", DateTime.MaxValue, 1.5m };
		}

		public class A
		{
			public int Id { get; set; }

			public string Name { get; set; }

			public DateTime Date { get; set; }

			public decimal? Decimal { get; set; }
		}
	}
}
