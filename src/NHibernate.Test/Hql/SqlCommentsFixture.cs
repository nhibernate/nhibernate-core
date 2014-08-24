using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Hql
{
	[TestFixture]
	public class SqlCommentsFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Hql.Animal.hbm.xml" }; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.UseSqlComments, "true");
		}

		[Test]
		public void CommentsInQuery()
		{
			using (ISession s = OpenSession())
			{
				using (var sl = new SqlLogSpy())
				{
					s.CreateQuery("from Animal").SetComment("This is my query").List();
					string sql = sl.Appender.GetEvents()[0].RenderedMessage;
					Assert.That(sql.IndexOf("This is my query"), Is.GreaterThan(0));
				}
			}
		}
	}
}