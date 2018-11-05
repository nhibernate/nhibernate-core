using System.Collections.Generic;
using System.Linq;
using NHibernate.Collection.Generic;
using NUnit.Framework;

namespace NHibernate.Test.CollectionTest
{
	[TestFixture]
	public class PersistentGenericMapFixture
	{
		[Test]
		public void SelectManyWorksCorrectly()
		{
			var bags = new[]
			{
				new PersistentGenericMap<string, string>(
					null,
					new Dictionary<string, string> {{"A", "B"}}),
				new PersistentGenericMap<string, string>(
					null,
					new Dictionary<string, string> {{"C", "D"}})
			};

			var items = bags.SelectMany(b => b).ToArray();

			Assert.That(
				items,
				Is.EquivalentTo(new[] {new KeyValuePair<string, string>("A", "B"), new KeyValuePair<string, string>("C", "D")}));
		}

		[Test]
		public void AddRangeWorksCorrectly()
		{
			var items = new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>("A", "B")};

			items.AddRange(
				new PersistentGenericMap<string, string>(
					null,
					new Dictionary<string, string> {{"C", "D"}}));

			Assert.That(
				items,
				Is.EquivalentTo(new[] {new KeyValuePair<string, string>("A", "B"), new KeyValuePair<string, string>("C", "D")}));
		}
	}
}
