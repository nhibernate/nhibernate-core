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
			nameof(A.Name)
		};

		protected override object GetValue(int i)
		{
			switch (i)
			{
				case 0:
					return 5;
				case 1:
					return "name";
			}

			return null;
		}

		public class A
		{
			public int Id { get; set; }

			public string Name { get; set; }
		}
	}
}
