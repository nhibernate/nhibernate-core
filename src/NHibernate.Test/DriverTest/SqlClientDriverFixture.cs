using System;
using System.Collections;
using NHibernate.Dialect;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.DriverTest
{
	public class MultiTypeEntity
	{
		public MultiTypeEntity()
		{
			DateTimeProp = DateTime.Now;
		}
		public virtual int Id { get; set; }
		public virtual string StringProp { get; set; }
		public virtual string AnsiStringProp { get; set; }
		public virtual decimal Decimal { get; set; }
		public virtual decimal Currency { get; set; }
		public virtual double Double { get; set; }
		public virtual float Float { get; set; }
		public virtual byte[] BinaryBlob { get; set; }
		public virtual byte[] Binary { get; set; }
		public virtual string StringClob { get; set; }
		public virtual DateTime DateTimeProp { get; set; }
	}

	[TestFixture]
	public class SqlClientDriverFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "DriverTest.MultiTypeEntity.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2008Dialect;
		}

		[Test]
		public void Crud()
		{
			// Should use default dimension for CRUD op because the mapping does not 
			// have dimensions specified.
			object savedId;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				savedId = s.Save(new MultiTypeEntity
									{
										StringProp = "a",
										StringClob = "a",
										BinaryBlob = new byte[]{1,2,3},
										Binary = new byte[] { 4, 5, 6 },
										Currency = 123.4m,
										Double = 123.5d,
										Decimal = 789.5m
									});
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var m = s.Get<MultiTypeEntity>(savedId);
				m.StringProp = "b";
				m.StringClob = "b";
				m.BinaryBlob = new byte[] {4,5,6};
				m.Binary = new byte[] {7,8,9};
				m.Currency = 456.78m;
				m.Double = 987.6d;
				m.Decimal = 1323456.45m;
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from MultiTypeEntity").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void QueryPlansAreReused()
		{
			if (!(Sfi.ConnectionProvider.Driver is SqlClientDriver))
				Assert.Ignore("Test designed for SqlClientDriver only");

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				// clear the existing plan cache
				s.CreateSQLQuery("DBCC FREEPROCCACHE").ExecuteUpdate();
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var countPlansCommand = s.CreateSQLQuery("SELECT COUNT(*) FROM sys.dm_exec_cached_plans");

				var beforeCount = countPlansCommand.UniqueResult<int>();

				var insertCount = 10;
				for (var i=0; i<insertCount; i++)
				{
					s.Save(new MultiTypeEntity() { StringProp = new string('x', i + 1) });
					s.Flush();
				}

				var afterCount = countPlansCommand.UniqueResult<int>();

				Assert.That(afterCount - beforeCount, Is.LessThan(insertCount - 1),
					string.Format("Excessive query plans created: before={0} after={1}", beforeCount, afterCount));

				t.Rollback();
			}
		}
	}
}