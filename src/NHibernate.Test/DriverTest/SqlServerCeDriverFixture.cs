using System;
using System.Collections;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace NHibernate.Test.DriverTest
{
	public class SqlServerCeEntity
	{
		public virtual int Id { get; set; }

		public virtual string StringProp { get; set; }
		public virtual byte[] BinaryProp { get; set; }

		public virtual string StringClob { get; set; }
		public virtual byte[] BinaryBlob { get; set; }
	}

	[TestFixture]
	public class SqlServerCeDriverFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "DriverTest.SqlServerCeEntity.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSqlCeDialect;
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from SqlServerCeEntity");
				tx.Commit();
			}
		}

		[Test]
		public void SaveLoad()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				SqlServerCeEntity entity = new SqlServerCeEntity();
				entity.StringProp = "a small string";
				entity.BinaryProp = new byte[100];

				entity.StringClob = new String('a', 8193);
				entity.BinaryBlob = new byte[8193];

				s.Save(entity);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				SqlServerCeEntity entity =
					s.CreateCriteria(typeof(SqlServerCeEntity))
						.UniqueResult<SqlServerCeEntity>();

				Assert.That(entity.StringProp, Is.EqualTo("a small string"));
				Assert.That(entity.BinaryProp.Length, Is.EqualTo(100));

				Assert.That(entity.StringClob, Is.EqualTo(new String('a', 8193)));
				Assert.That(entity.BinaryBlob.Length, Is.EqualTo(8193));
			}
		}

		[Test]
		public void Query()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				SqlServerCeEntity entity = new SqlServerCeEntity();
				entity.StringProp = "a small string";
				entity.BinaryProp = System.Text.ASCIIEncoding.ASCII.GetBytes("binary string");

				entity.StringClob = new String('a', 8193);
				entity.BinaryBlob = new byte[8193];

				s.Save(entity);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				IList<SqlServerCeEntity> entities =
					s.CreateCriteria(typeof(SqlServerCeEntity))
						.Add(Restrictions.Eq("StringProp", "a small string"))
						.Add(Restrictions.Eq("BinaryProp", System.Text.ASCIIEncoding.ASCII.GetBytes("binary string")))
						.List<SqlServerCeEntity>();

				Assert.That(entities.Count, Is.EqualTo(1));
			}
		}
	}
}