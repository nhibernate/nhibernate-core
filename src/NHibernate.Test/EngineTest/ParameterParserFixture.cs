using NHibernate.Engine.Query;
using NUnit.Framework;

namespace NHibernate.Test.EngineTest
{
	[TestFixture]
	public class ParameterParserFixture
	{
		[Test]
		public void CanFindParameterAfterComment()
		{
			string query =
	@"
SELECT id 
FROM tablea 
/* Comment with ' number 2 */ 
WHERE Name = :name 
ORDER BY Name";

			var recognizer = new ParamLocationRecognizer();
			ParameterParser.Parse(query, recognizer);
			ParamLocationRecognizer.NamedParameterDescription p;
			Assert.DoesNotThrow(() => p = recognizer.NamedParameterDescriptionMap["name"]);
		}

		[Test]
		public void CanFindParameterAfterInlineComment()
		{
			string query =
	@"
SELECT id 
FROM tablea 
-- Comment with ' number 1 
WHERE Name = :name 
ORDER BY Name";

			var recognizer = new ParamLocationRecognizer();
			ParameterParser.Parse(query, recognizer);
			ParamLocationRecognizer.NamedParameterDescription p;
			Assert.DoesNotThrow(() => p = recognizer.NamedParameterDescriptionMap["name"]);
		}

		[Test]
		public void CanFindParameterAfterAnyComment()
		{
			string query =
	@"
SELECT id 
FROM tablea 
-- Comment with ' number 1 
WHERE Name = :name 
/* Comment with ' number 2 */ 
ORDER BY Name + :pizza";

			var recognizer = new ParamLocationRecognizer();
			ParameterParser.Parse(query, recognizer);
			ParamLocationRecognizer.NamedParameterDescription p;
			Assert.DoesNotThrow(() => p = recognizer.NamedParameterDescriptionMap["name"]);
			Assert.DoesNotThrow(() => p = recognizer.NamedParameterDescriptionMap["pizza"]);
		}

	}
}