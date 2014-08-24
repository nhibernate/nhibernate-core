using System.Text;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1710
{
	public class A
	{
		public virtual decimal? Amount { get; set; }
	}

	public abstract class BaseFixture
	{
		protected const string TestNameSpace = "NHibernate.Test.NHSpecificTest.NH1710.";
		protected Configuration cfg;
		protected ISessionFactoryImplementor factory;
		private string expectedExportString;

		[TestFixtureSetUp]
		public void Config()
		{
			cfg = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);

			cfg.AddResource(GetResourceFullName(), GetType().Assembly);

			factory = (ISessionFactoryImplementor)cfg.BuildSessionFactory();

			expectedExportString = GetDialect().GetTypeName(NHibernateUtil.Decimal.SqlType, 0, 5, 2);
		}

		[Test]
		public void NotIgnorePrecisionScaleInSchemaExport()
		{
			var script = new StringBuilder();
			new SchemaExport(cfg).Create(sl => script.AppendLine(sl), true);
			Assert.That(script.ToString(), Is.StringContaining(expectedExportString));
			new SchemaExport(cfg).Drop(false, true);
		}

		private Dialect.Dialect GetDialect()
		{
			return Dialect.Dialect.GetDialect(cfg.Properties);
		}

		protected abstract string GetResourceName();

		private string GetResourceFullName()
		{
			return TestNameSpace + GetResourceName();
		}
	}

	[TestFixture]
	public class FixtureWithExplicitDefinedType : BaseFixture
	{
		protected override string GetResourceName()
		{
			return "Heuristic.hbm.xml";
		}
	}

	[TestFixture]
	public class FixtureWithHeuristicDefinedType : BaseFixture
	{
		protected override string GetResourceName()
		{
			return "Defined.hbm.xml";
		}
	}

	[TestFixture]
	public class FixtureWithInLineDefinedType : BaseFixture
	{
		protected override string GetResourceName()
		{
			return "InLine.hbm.xml";
		}
	}

	[TestFixture]
	public class FixtureWithColumnNode : BaseFixture
	{
		protected override string GetResourceName()
		{
			return "WithColumnNode.hbm.xml";
		}
	}
}