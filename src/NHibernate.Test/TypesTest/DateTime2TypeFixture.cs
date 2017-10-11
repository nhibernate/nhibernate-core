using System;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// TestFixtures for the <see cref="DateTimeType"/>.
	/// </summary>
	[TestFixture]
	[Obsolete]
	public class DateTime2TypeFixture : AbstractDateTimeTypeFixture
	{
		protected override bool AppliesTo(Dialect.Dialect dialect) =>
			TestDialect.SupportsSqlType(SqlTypeFactory.DateTime2);

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory) =>
			// Cannot handle DbType.DateTime2 via .Net ODBC.
			!(factory.ConnectionProvider.Driver.IsOdbcDriver());

		protected override string TypeName => "DateTime2";
		protected override AbstractDateTimeType Type => NHibernateUtil.DateTime2;

		[Test]
		public void ObsoleteMessage()
		{
			using (var spy = new LogSpy(typeof(TypeFactory)))
			{
				TypeFactory.Basic(NHibernateUtil.DateTime2.Name);
				Assert.That(
					spy.GetWholeLog(),
					Does.Contain($"{NHibernateUtil.DateTime2.Name} is obsolete. Use DateTimeType instead").IgnoreCase);
			}
		}
	}

	[TestFixture]
	[Obsolete]
	public class DateTime2TypeWithScaleFixture : DateTimeTypeWithScaleFixture
	{
		protected override bool AppliesTo(Dialect.Dialect dialect) =>
			TestDialect.SupportsSqlType(SqlTypeFactory.DateTime2);

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory) =>
			// Cannot handle DbType.DateTime2 via .Net ODBC.
			!(factory.ConnectionProvider.Driver.IsOdbcDriver());

		protected override string TypeName => "DateTime2WithScale";
		protected override AbstractDateTimeType Type => (AbstractDateTimeType)TypeFactory.GetDateTime2Type(3);
	}
}
