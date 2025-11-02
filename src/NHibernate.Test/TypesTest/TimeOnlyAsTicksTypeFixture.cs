#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture(false)]
	[TestFixture(true)]
	public class TimeOnlyAsTicksTypeFixture : AbstractTimeOnlyTypeWithScaleFixture<TimeOnlyAsTicksType>
	{
		public TimeOnlyAsTicksTypeFixture(bool setMaxScale) : base(setMaxScale)
		{
		}

		protected override long MaxTimestampResolutionInTicks => 1L;

		protected override IEnumerable<Expression<Func<TimeOnly, object>>> PropertiesToTestWithLinq => null;
	}
}
#endif
