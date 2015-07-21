using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	[TestFixture]
	public class AggregateReturnTypesFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"QueryTest.Aggregated.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnSetUp()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Aggregated agg = new Aggregated();
				agg.AByte = 10;
				agg.AShort = 20;
				agg.AnInt = 30;
				agg.ALong = 40;
				agg.AFloat = 50.5f;
				agg.ADouble = 60.6;
				agg.ADecimal = 70.707m;

				s.Save(agg);
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Aggregated");
				tx.Commit();
			}
		}

		private System.Type AggregateType(string expr)
		{
			using (ISession s = OpenSession())
			{
				return s.CreateQuery("select " + expr + " from Aggregated a").UniqueResult().GetType();
			}
		}

		private void CheckType(string expr, System.Type type)
		{
			Assert.AreSame(type, AggregateType(expr));
		}

		[Test]
		public void Sum()
		{
			CheckType("sum(a.AByte)", typeof(UInt64));

			CheckType("sum(a.AShort)", typeof(Int64));
			CheckType("sum(a.AnInt)", typeof(Int64));
			CheckType("sum(a.ALong)", typeof(Int64));

			CheckType("sum(a.AFloat)", typeof(Double));
			CheckType("sum(a.ADouble)", typeof(Double));

			CheckType("sum(a.ADecimal)", typeof(Decimal));
		}

		[Test]
		public void Avg()
		{
			CheckType("avg(a.AByte)", typeof(Double));

			CheckType("avg(a.AShort)", typeof(Double));
			CheckType("avg(a.AnInt)", typeof(Double));
			CheckType("avg(a.ALong)", typeof(Double));

			CheckType("avg(a.AFloat)", typeof(Double));
			CheckType("avg(a.ADouble)", typeof(Double));

			CheckType("avg(a.ADecimal)", typeof(Double));
		}

		[Test]
		public void Min()
		{
			CheckType("min(a.AByte)", typeof(Byte));

			CheckType("min(a.AShort)", typeof(Int16));
			CheckType("min(a.AnInt)", typeof(Int32));
			CheckType("min(a.ALong)", typeof(Int64));

			CheckType("min(a.AFloat)", typeof(Single));
			CheckType("min(a.ADouble)", typeof(Double));

			CheckType("min(a.ADecimal)", typeof(Decimal));
		}

		[Test]
		public void Max()
		{
			CheckType("max(a.AByte)", typeof(Byte));

			CheckType("max(a.AShort)", typeof(Int16));
			CheckType("max(a.AnInt)", typeof(Int32));
			CheckType("max(a.ALong)", typeof(Int64));

			CheckType("max(a.AFloat)", typeof(Single));
			CheckType("max(a.ADouble)", typeof(Double));

			CheckType("max(a.ADecimal)", typeof(Decimal));
		}
	}
}