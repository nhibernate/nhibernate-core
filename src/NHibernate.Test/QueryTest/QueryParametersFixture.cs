using NHibernate.Engine;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	[TestFixture]
	public class QueryParametersFixture
	{
		[Test]
		public void ValidateNullParameters()
		{
			QueryParameters qp = new QueryParameters(null, null);
			qp.ValidateParameters();
		}

		[Test]
		public void ValidateOk()
		{
			QueryParameters qp = new QueryParameters(
				new IType[] {NHibernateUtil.String},
				new object[] {"string"});

			qp.ValidateParameters();
		}

		[Test]
		public void ValidateFailureDifferentLengths()
		{
			QueryParameters qp = new QueryParameters(
				new IType[] {NHibernateUtil.String},
				new object[] {});

			Assert.Throws<QueryException>(() => qp.ValidateParameters());
		}
	}
}