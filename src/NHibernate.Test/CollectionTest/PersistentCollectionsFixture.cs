using System.Collections.Generic;
using System.Linq;
using NHibernate.Collection.Generic;
using NUnit.Framework;

namespace NHibernate.Test.CollectionTest
{
	[TestFixture]
	public class PersistentCollectionsFixture
	{
		[Test]
		public void SelectManyWorksCorrectly()
		{
			var bags = new IEnumerable<string>[]
			{
				new List<string> {"A"},
				new PersistentGenericBag<string>(null, new[] {"B"}),
				new PersistentIdentifierBag<string>(null, new[] {"C"}),
				new PersistentGenericList<string>(null, new[] {"D"}),
				new PersistentGenericSet<string>(null, new HashSet<string> {"E"})
			};
			
			var items = bags.SelectMany(b => b).ToArray();

			Assert.That(items, Is.EqualTo(new[] {"A", "B", "C", "D", "E"}));
		}

		[Test]
		public void AddRangeWorksCorrectly()
		{
			var items = new List<string> {"A"};
			items.AddRange(new PersistentGenericBag<string>(null, new[] {"B"}));	
			items.AddRange(new PersistentIdentifierBag<string>(null, new[] {"C"}));	
			items.AddRange(new PersistentGenericList<string>(null, new[] {"D"}));	
			items.AddRange(new PersistentGenericSet<string>(null, new HashSet<string> {"E"}));	
			
			Assert.That(items, Is.EqualTo(new[] {"A", "B", "C", "D", "E"}));
		}

		[Test]
		public void SelectManyWorksCorrectlyWithIReadOnlyCollection()
		{
			var bags = new IReadOnlyCollection<string>[]
			{
				new List<string> {"A"},
				new PersistentGenericBag<string>(null, new[] {"B"}),
				new PersistentIdentifierBag<string>(null, new[] {"C"}),
				(IReadOnlyList<string>)new PersistentGenericList<string>(null, new[] {"D"}),
				new PersistentGenericSet<string>(null, new HashSet<string> {"E"})
			};
			
			var items = bags.SelectMany(b => b).ToArray();

			Assert.That(items, Is.EqualTo(new[] {"A", "B", "C", "D", "E"}));
		}
	}
}
