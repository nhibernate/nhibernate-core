using System.Collections;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.DriverTest
{
	public class MultiTypeEntity
	{
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
	}

	[TestFixture]
	public class SqlClientDriverFixture : TestCase
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
			get { return new[] { "DriverTest.MultiTypeEntity.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void Crud()
		{
			// Should use default dimension for CRUD op and prepare_sql='true' because the mapping does not 
			// have dimensions specified.
			object savedId;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				savedId = s.Save(new MultiTypeEntity
				                 	{
				                 		StringProp = "a", StringClob = "a",BinaryBlob = new byte[]{1,2,3},
														Binary = new byte[] { 4, 5, 6 }, Currency = 123.4m, Double = 123.5d,
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
	}
}