using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
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

	public abstract class TypeSqlTypeFixture
	{
		protected const string TestNameSpace = "NHibernate.Test.TypesTest.";
		protected Configuration cfg;
		protected ISessionFactoryImplementor factory;

		[TestFixtureSetUp]
		public void Config()
		{
			cfg = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);
			Dialect.Dialect dialect = Dialect.Dialect.GetDialect(cfg.Properties);
			if (!AppliesTo(dialect))
			{
				Assert.Ignore(GetType() + " does not apply to " + dialect);
			}

			cfg.AddResource(GetResourceFullName(), GetType().Assembly);

			factory = (ISessionFactoryImplementor) cfg.BuildSessionFactory();
		}

		protected virtual bool AppliesTo(Dialect.Dialect dialect)
		{
			return true;
		}

		[Test]
		public void NotIgnoreSqlTypeDef()
		{
			var pc = factory.GetEntityPersister(typeof(MultiTypeEntity).FullName);

			var type = pc.GetPropertyType("StringProp");
			Assert.That(type.SqlTypes(factory)[0].Length, Is.EqualTo(100));

			type = pc.GetPropertyType("AnsiStringProp");
			Assert.That(type.SqlTypes(factory)[0].Length, Is.EqualTo(101));

			type = pc.GetPropertyType("Decimal");
			Assert.That(type.SqlTypes(factory)[0].Precision, Is.EqualTo(5));
			Assert.That(type.SqlTypes(factory)[0].Scale, Is.EqualTo(2));

			type = pc.GetPropertyType("Currency");
			Assert.That(type.SqlTypes(factory)[0].Precision, Is.EqualTo(10));
			Assert.That(type.SqlTypes(factory)[0].Scale, Is.EqualTo(3));

			type = pc.GetPropertyType("Double");
			Assert.That(type.SqlTypes(factory)[0].Precision, Is.EqualTo(11));
			Assert.That(type.SqlTypes(factory)[0].Scale, Is.EqualTo(4));

			type = pc.GetPropertyType("Float");
			Assert.That(type.SqlTypes(factory)[0].Precision, Is.EqualTo(6));
			Assert.That(type.SqlTypes(factory)[0].Scale, Is.EqualTo(3));

			type = pc.GetPropertyType("BinaryBlob");
			Assert.That(type.SqlTypes(factory)[0].Length, Is.EqualTo(1000));

			type = pc.GetPropertyType("Binary");
			Assert.That(type.SqlTypes(factory)[0].Length, Is.EqualTo(1001));

			type = pc.GetPropertyType("StringClob");
			Assert.That(type.SqlTypes(factory)[0].Length, Is.EqualTo(1002));
		}

		protected abstract string GetResourceName();

		private string GetResourceFullName()
		{
			return TestNameSpace + GetResourceName();
		}
	}

	[TestFixture]
	public class FixtureWithExplicitDefinedType : TypeSqlTypeFixture
	{
		protected override string GetResourceName()
		{
			return "MultiTypeEntity_Defined.hbm.xml";
		}
	}

	[TestFixture]
	public class FixtureWithHeuristicDefinedType : TypeSqlTypeFixture
	{
		protected override string GetResourceName()
		{
			return "MultiTypeEntity_Heuristic.hbm.xml";
		}
	}

	[TestFixture]
	public class FixtureWithInLineDefinedType : TypeSqlTypeFixture
	{
		protected override string GetResourceName()
		{
			return "MultiTypeEntity_InLine.hbm.xml";
		}
	}

	[TestFixture]
	public class FixtureWithColumnNode : TypeSqlTypeFixture
	{
		protected override string GetResourceName()
		{
			return "MultiTypeEntity_WithColumnNode.hbm.xml";
		}
	}


	[TestFixture, Ignore("Not fixed yet.")]
	public class FixtureWithSqlType : TypeSqlTypeFixture
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}
		protected override string GetResourceName()
		{
			return "MultiTypeEntity_WithSqlType.hbm.xml";
		}
	}
}