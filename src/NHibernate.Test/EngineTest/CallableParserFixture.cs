using NUnit.Framework;
using NHibernate.Engine.Query;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Test.EngineTest
{
	[TestFixture]
	public class CallableParserFixture
	{
		[Test]
		public void CanFindCallableFunctionName()
		{
			string query = @"{ call myFunction(:name) }";

			SqlString sqlFunction =  CallableParser.Parse(query);
			Assert.That(sqlFunction.ToString(), Is.EqualTo("myFunction"));
		}

		[Test]
		public void CanDetermineIsNotCallable()
		{
			string query = @"SELECT id FROM mytable";

			Assert.Throws<ParserException>(() =>
				{
					SqlString sqlFunction =  CallableParser.Parse(query);
				});
		}

		[Test]
		public void CanFindCallableFunctionNameWithoutParameters()
		{
			string query = @"{ call myFunction }";

			SqlString sqlFunction =  CallableParser.Parse(query);
			Assert.That(sqlFunction.ToString(), Is.EqualTo("myFunction"));
		}
	}
}