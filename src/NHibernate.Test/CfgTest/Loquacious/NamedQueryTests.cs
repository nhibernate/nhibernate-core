using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.CfgTest.Loquacious
{
	public class NamedQueryTests
	{
		[Test]
		public void AddSimpleNamedQuery()
		{
			var configure = new Configuration();
			configure.AddNamedQuery("aQuery", b =>
			{
				b.Query = "from System.Object o";
			});

			configure.NamedQueries.Should().Have.Count.EqualTo(1);
			configure.NamedQueries.Keys.Single().Should().Be("aQuery");
			configure.NamedQueries.Values.Single().Query.Should().Be("from System.Object o");
		}

		[Test]
		public void WhenSetInvalidFetchSizeThenLeaveDefault()
		{
			var configure = new Configuration();
			configure.AddNamedQuery("aQuery", b =>
			{
				b.Query = "from System.Object o";
				b.FetchSize = 0;
			});

			configure.NamedQueries.Values.Single().FetchSize.Should().Be(-1);
		}

		[Test]
		public void WhenSetValidFetchSizeThenSetValue()
		{
			var configure = new Configuration();
			configure.AddNamedQuery("aQuery", b =>
			{
				b.Query = "from System.Object o";
				b.FetchSize = 15;
			});

			configure.NamedQueries.Values.Single().FetchSize.Should().Be(15);
		}

		[Test]
		public void WhenSetInvalidTimeoutThenLeaveDefault()
		{
			var configure = new Configuration();
			configure.AddNamedQuery("aQuery", b =>
			{
				b.Query = "from System.Object o";
				b.Timeout = 0;
			});

			configure.NamedQueries.Values.Single().Timeout.Should().Be(-1);
		}

		[Test]
		public void WhenSetValidTimeoutThenSetValue()
		{
			var configure = new Configuration();
			configure.AddNamedQuery("aQuery", b =>
			{
				b.Query = "from System.Object o";
				b.Timeout = 123;
			});

			configure.NamedQueries.Values.Single().Timeout.Should().Be(123);
		}
	}
}