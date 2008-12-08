using System.Collections;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;

using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1593
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void SchemaUpdateAddsIndexesThatWerentPresentYet()
		{
			Configuration cfg = new Configuration();
			Assembly assembly = Assembly.GetExecutingAssembly();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1593.TestIndex.hbm.xml", assembly);
			cfg.Configure();

			// TODO: rewrite this so we don't need to open a session just to get a reference to a DbConnection (because that's the only reason the Session is used)
			var sessionFactory = cfg.BuildSessionFactory();
			using (ISession session = sessionFactory.OpenSession())
			{
				MsSql2005Dialect dialect = new MsSql2005Dialect();

				DatabaseMetadata databaseMetaData = new DatabaseMetadata((DbConnection)session.Connection, dialect);
				string[] script = cfg.GenerateSchemaUpdateScript(dialect, databaseMetaData);

				Assert.That(ScriptContainsIndexCreationLine(script));
			}
		}

		private bool ScriptContainsIndexCreationLine(string[] script)
		{
			foreach (string s in script)
			{
				if (s.Equals("create index test_index_name on TestIndex (Name)"))
				{
					return true;
				}
			}

			return false;
		}
	}
}