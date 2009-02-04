using System;
using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	[TestFixture]
	public abstract class FixtureBase : TestCase
	{
		protected abstract override IList Mappings { get; }

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2008Dialect;
		}

		protected void SavingAndRetrievingAction(AllDates entity, Action<AllDates> action)
		{
			AllDates dates = entity;

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(dates);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var datesRecovered = s.CreateQuery("from AllDates").UniqueResult<AllDates>();

				action.Invoke(datesRecovered);
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var datesRecovered = s.CreateQuery("from AllDates").UniqueResult<AllDates>();
				s.Delete(datesRecovered);
				tx.Commit();
			}
		}
	}
}