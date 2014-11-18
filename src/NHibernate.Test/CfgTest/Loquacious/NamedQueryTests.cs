using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;

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

			Assert.That(configure.NamedQueries, Has.Count.EqualTo(1));
			Assert.That(configure.NamedQueries.Keys.Single(), Is.EqualTo("aQuery"));
			Assert.That(configure.NamedQueries.Values.Single().Query, Is.EqualTo("from System.Object o"));
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

			Assert.That(configure.NamedQueries.Values.Single().FetchSize, Is.EqualTo(-1));
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

			Assert.That(configure.NamedQueries.Values.Single().FetchSize, Is.EqualTo(15));
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

			Assert.That(configure.NamedQueries.Values.Single().Timeout, Is.EqualTo(-1));
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

			Assert.That(configure.NamedQueries.Values.Single().Timeout, Is.EqualTo(123));
		}
	}
}