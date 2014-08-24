using System;
using System.Collections;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;
using SharpTestsEx;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.DriverTest
{
	public class EntityForMs2008
	{
		public virtual int Id { get; set; }
		public virtual DateTime DateTimeProp { get; set; }
		public virtual TimeSpan TimeSpanProp { get; set; }
	}

	public class Sql2008DateTime2Test : TestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.PrepareSql, "true");
		}
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "DriverTest.EntityForMs2008.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2008Dialect;
		}

		[Test]
		public void Crud()
		{
			var expectedMoment = new DateTime(1848, 6, 1, 12, 00, 00, 123);
			var expectedLapse = new TimeSpan((DateTime.Now - expectedMoment).Ticks);
			object savedId;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				savedId = s.Save(new EntityForMs2008
				                 	{
				                 		DateTimeProp = expectedMoment,
														TimeSpanProp = expectedLapse,
													});
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var m = s.Get<EntityForMs2008>(savedId);
				m.DateTimeProp.Should().Be(expectedMoment);
				m.TimeSpanProp.Should().Be(expectedLapse);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from EntityForMs2008").ExecuteUpdate();
				t.Commit();
			}
		}

	}
}