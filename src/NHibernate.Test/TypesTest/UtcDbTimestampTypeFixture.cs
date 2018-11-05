using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class UtcDbTimestampTypeFixture : AbstractDateTimeTypeFixture
	{
		protected override string TypeName => "UtcDbTimestamp";
		protected override AbstractDateTimeType Type => NHibernateUtil.UtcDbTimestamp;
		protected override DateTime Now => (DateTime) Type.Seed(_session?.GetSessionImplementation());
		private ISession _session;

		protected override void OnSetUp()
		{
			_session = OpenSession();
			base.OnSetUp();
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			_session.Dispose();
			_session = null;
		}
	}
}
