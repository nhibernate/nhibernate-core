using NHibernate.Engine.Query;
using NSubstitute;
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
			Assert.That(recognizer.NamedParameterDescriptionMap, Has.Count.EqualTo(1));
			Assert.That(recognizer.NamedParameterDescriptionMap, Contains.Key("name"));
		}

		[Test]
		public void CanFindParameterAfterInlineComment()
		{
			string query =
	@"
SELECT id 
FROM tablea 
-- Comment with ' :number 1 
WHERE Name = :name 
ORDER BY Name";

			var recognizer = new ParamLocationRecognizer();
			ParameterParser.Parse(query, recognizer);
			Assert.That(recognizer.NamedParameterDescriptionMap, Has.Count.EqualTo(1));
			Assert.That(recognizer.NamedParameterDescriptionMap, Contains.Key("name"));
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
			Assert.That(recognizer.NamedParameterDescriptionMap, Has.Count.EqualTo(2));
			Assert.That(recognizer.NamedParameterDescriptionMap, Contains.Key("name"));
			Assert.That(recognizer.NamedParameterDescriptionMap, Contains.Key("pizza"));
		}

		[Test]
		public void IgnoresCommentsWithinQuotes()
		{
			string query =
	@"
SELECT id 
FROM tablea WHERE Name = '
-- :comment1
' OR Name = '
/* :comment2 */
'
-- :comment3
/* :comment4 */
ORDER BY Name + :pizza -- :comment5";

			var recognizer = Substitute.For<ParameterParser.IRecognizer>();
			ParameterParser.Parse(query, recognizer);

			//Only one parameter in the query
			recognizer.ReceivedWithAnyArgs(1).NamedParameter(default, default);
			recognizer.Received(1).NamedParameter("pizza", Arg.Any<int>());

			//comment1 and comment2 are not really comments and therefore not parsed as blocks
			recognizer.DidNotReceive().Other(Arg.Is<string>(x => x.Contains("comment1")));
			recognizer.DidNotReceive().Other(Arg.Is<string>(x => x.Contains("comment2")));

			//comment 3-5 are actual comments and therefore parsed as blocks
			recognizer.Received(1).Other(Arg.Is<string>(x => x.StartsWith("-- :comment3")));
			recognizer.Received(1).Other("/* :comment4 */");
			recognizer.Received(1).Other(Arg.Is<string>(x => x.StartsWith("-- :comment5")));
		}
	}
}
