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
			"_name"
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
			private int _id = 5;
			private string _name =string.Empty;

			public int Id => _id;

			public string Name => _name;
		}
	}
}
