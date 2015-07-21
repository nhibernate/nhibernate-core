using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.FilterTest
{
	[TestFixture]
	public class FilterBinaryParameterTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"FilterTest.BinaryFiltered.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void NH882()
		{
			using (ISession session = OpenSession())
			{
				byte[] binValue = {0xFF, 0xFF, 0xFF};
				session.EnableFilter("BinaryFilter").SetParameter("BinaryValue", binValue);
				IQuery query = session.CreateQuery("from BinaryFiltered");
				query.List();
			}
		}
	}
}