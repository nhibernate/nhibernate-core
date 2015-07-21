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
		public void CanDetermineIsCallable()
		{
			string query = @"{ call myFunction(:name) }";

			CallableParser.Detail detail = CallableParser.Parse(query);
			Assert.That(detail.IsCallable, Is.True);
		}

		[Test]
		public void CanDetermineIsNotCallable()
		{
			string query = @"SELECT id FROM mytable";

			CallableParser.Detail detail = CallableParser.Parse(query);
			Assert.That(detail.IsCallable, Is.False);
		}

		[Test]
		public void CanFindCallableFunctionName()
		{
			string query = @"{ call myFunction(:name) }";

			CallableParser.Detail detail = CallableParser.Parse(query);
			Assert.That(detail.FunctionName, Is.EqualTo("myFunction"));
		}

		[Test]
		public void CanFindCallableFunctionNameWithoutParameters()
		{
			string query = @"{ call myFunction }";

			CallableParser.Detail detail = CallableParser.Parse(query);
			Assert.That(detail.FunctionName, Is.EqualTo("myFunction"));
		}

		[Test]
		public void CanFindCallablePackageFunctionName()
		{
			string query = @"{ call myPackage.No2_Function(:name) }";

			CallableParser.Detail detail = CallableParser.Parse(query);
			Assert.That(detail.FunctionName, Is.EqualTo("myPackage.No2_Function"));
		}

		[Test]
		public void CanDetermineHasReturn()
		{
			string query = @"{ ? = call myFunction(:name) }";

			CallableParser.Detail detail = CallableParser.Parse(query);
			Assert.That(detail.HasReturn, Is.True);
		}

		[Test]
		public void CanDetermineHasNoReturn()
		{
			string query = @"{ call myFunction(:name) }";

			CallableParser.Detail detail = CallableParser.Parse(query);
			Assert.That(detail.HasReturn, Is.False);
		}
	}
}