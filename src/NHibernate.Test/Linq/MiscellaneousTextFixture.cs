using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class MiscellaneousTextFixture : LinqTestCase
	{
		[Category("WHERE")]
		[Test(Description = "This sample uses WHERE to filter for Shippers using a Guid property.")]
		public void WhereUsingGuidProperty()
		{
			var q =
				from s in db.Shippers
				where s.Reference == new Guid("6DFCD0D7-4D2E-4525-A502-3EA9AA52E965")
				select s;

			AssertByIds(q, new[] { 2 }, x => x.ShipperId);
		}

		[Category("COUNT/SUM/MIN/MAX/AVG")]
        [Test(Description = "This sample uses Count to find the number of Orders placed before yesterday in the database.")]
        public void CountWithWhereClause()
        {
            var q = from o in db.Orders where o.OrderDate <= DateTime.Today.AddDays(-1) select o;

            int count = q.Count();

            Console.WriteLine(count);
        }
	}
}
