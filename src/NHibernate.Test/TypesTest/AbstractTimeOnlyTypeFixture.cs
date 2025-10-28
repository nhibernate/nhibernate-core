#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Type;

namespace NHibernate.Test.TypesTest
{
	public class AbstractTimeOnlyTypeFixture<TType> : GenericTypeFixtureBase<TimeOnly, TType> where TType : IType
	{
		protected override IReadOnlyList<TimeOnly> TestValues => [new(12, 13, 14), new(23, 59, 59), new(0, 0, 0)];
		protected override IEnumerable<Expression<Func<TimeOnly, object>>> PropertiesToTestWithLinq
		{
			get
			{
				yield return (TimeOnly x) => x.Hour;
				yield return (TimeOnly x) => x.Minute;
				yield return (TimeOnly x) => x.Second;
			}
		}
	}
}
#endif
