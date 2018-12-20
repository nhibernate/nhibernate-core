using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	[TestFixture, Explicit]
	public class BasicPropertyAccessorPerformanceFixture : AccessorPerformanceFixture<BasicPropertyAccessorPerformanceFixture.A>
	{
		protected override string AccessorType => "property";

		protected override string Path => "Id";

		protected override object GetValue()
		{
			return 5;
		}

		public class A
		{
			public int Id { get; set; }
		}
	}
}
