using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;
using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2530
{
	public class Fixture: BugTestCase
	{
		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.AddAuxiliaryDatabaseObject(CreateHighLowScript(new[] { typeof(Product) }));
		}

		private IAuxiliaryDatabaseObject CreateHighLowScript(IEnumerable<System.Type> entities)
		{
			var script = new StringBuilder(1024);
			script.AppendLine("DELETE FROM NextHighVaues;");
			script.AppendLine("ALTER TABLE NextHighVaues ADD Entity VARCHAR(128) NOT NULL;");
			script.AppendLine("CREATE NONCLUSTERED INDEX IdxNextHighVauesEntity ON NextHighVaues (Entity ASC);");
			script.AppendLine("GO");
			foreach (var entity in entities)
			{
				script.AppendLine(string.Format("INSERT INTO [NextHighVaues] (Entity, NextHigh) VALUES ('{0}',1);", entity.Name));
			}
			var dialects = new HashSet<string>
							   {
								   typeof (MsSql2000Dialect).FullName,
								   typeof (MsSql2005Dialect).FullName,
								   typeof (MsSql2008Dialect).FullName,
								   typeof (MsSql2012Dialect).FullName
							   };
			return new SimpleAuxiliaryDatabaseObject(script.ToString(), null, dialects);
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return (dialect is MsSql2000Dialect);
		}

		[Test]
		public void WhenTryToGetHighThenExceptionShouldContainWhereClause()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var customer = new Customer { Name = "Mengano" };
				session.Executing(s => s.Persist(customer)).Throws().And.ValueOf.Message.Should().Contain("Entity = 'Customer'");
			}
		}
	}
}