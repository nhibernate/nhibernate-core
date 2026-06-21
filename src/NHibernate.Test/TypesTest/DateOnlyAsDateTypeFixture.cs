#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Type;

namespace NHibernate.Test.TypesTest
{
	public class DateOnlyAsDateTypeFixture : GenericTypeFixtureBase<DateOnly, DateOnlyAsDateType>
	{
		protected override IReadOnlyList<DateOnly> TestValues =>
		[
			DateOnly.FromDateTime(Sfi.ConnectionProvider.Driver.MinDate.AddDays(1)),
			DateOnly.FromDateTime(DateTime.Now),
			DateOnly.MaxValue.AddDays(-1)
		];

		protected override IList<Expression<Func<DateOnly, object>>> PropertiesToTestWithLinq =>
		[
			(DateOnly x) => x.Year,
			(DateOnly x) => x.Month,
			(DateOnly x) => x.Day
		];
	}
}
#endif
