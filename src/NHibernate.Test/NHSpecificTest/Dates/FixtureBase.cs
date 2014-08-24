using System;
using System.Collections;
using System.Data;
using NHibernate.Dialect;
using NHibernate.Util;
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
			var typeNames = (TypeNames)typeof(Dialect.Dialect).GetField("_typeNames", ReflectHelper.AnyVisibilityInstance).GetValue(Dialect);
			try
			{
				var value = AppliesTo();

				if (value == null) return true;
				
				typeNames.Get(value.Value);
			}
			catch (ArgumentException)
			{
				return false;
			}
			catch (Exception)
			{
				Assert.Fail("Probably a bug in the test case.");
			}

			return true;
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

		protected virtual DbType? AppliesTo() 
		{
			return null;
		}
	}
}