using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	[TestFixture]
	public class QueryParametersFixture
	{
		[Test]
		public void NullParameters()
		{
			QueryParameters qp = new QueryParameters( null, null );
			qp.ValidateParameters();
		}
	}
}
