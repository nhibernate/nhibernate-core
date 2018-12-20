using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	[TestFixture, Explicit]
	public class FieldAccessorPerformanceFixture : AccessorPerformanceFixture<FieldAccessorPerformanceFixture.A>
	{
		protected override string AccessorType => "field";

		protected override string Path => "_id";

		protected override object GetValue()
		{
			return 10;
		}

		public class A
		{
			private int _id = 5;

			public int Id => _id;
		}
	}
}
