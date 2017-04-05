﻿using System;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2660And2661
{
	[TestFixture]
	public class Test : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				DomainClass entity = new DomainClass { Id = 1, Data = DateTime.Parse("10:00") };
				session.Save(entity);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
								session.CreateQuery("delete from DomainClass").ExecuteUpdate();
				session.Flush();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2008Dialect;
		}

				protected override void Configure(Configuration configuration)
				{
					// to be sure we are using the new drive
					base.Configure(configuration);
					configuration.DataBaseIntegration(x=> x.Driver<Sql2008ClientDriver>());
				}

		[Test]
		public void ShouldBeAbleToQueryEntity()
		{
			using (ISession session = OpenSession())
			{
			   var query =
					session.CreateQuery(
						@"from DomainClass entity where Data = :data");
				query.SetParameter("data", DateTime.Parse("10:00"), NHibernateUtil.Time);
				Assert.That(() => query.List(), Throws.Nothing);
			}
		}
	}
}
