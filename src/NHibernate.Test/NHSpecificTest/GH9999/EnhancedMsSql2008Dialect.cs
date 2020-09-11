using System;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace NHibernate.Test.NHSpecificTest.GH9999
{
	public class EnhancedMsSql2008Dialect : MsSql2008Dialect
	{
		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();

			RegisterFunction(
				nameof(DateTime.DayOfYear),
				new SQLFunctionTemplate(
					NHibernateUtil.Int32,
					"datepart(dy, ?1)"
				)
			);

			RegisterFunction(
				nameof(DateTime.AddYears),
				new SQLFunctionTemplate(
					NHibernateUtil.DateTime,
					"dateadd(year,?2,?1)"
				)
		   );
		}
	}
}
