using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class LinkHashMapFixture
	{
		private static readonly Player[] players =
		{
			new Player("12341", "Boeta Dippenaar"),
			new Player("23432", "Gary Kirsten"),
			new Player("23411", "Graeme Smith"),
			new Player("55221", "Jonty Rhodes"),
			new Player("61234", "Monde Zondeki"),
			new Player("23415", "Paul Adams")
		};

		private static void Fill(IDictionary<string, Player> lhm)
		{
			foreach (var player in players)
				lhm.Add(player.Id, player);
		}

		[Test]
		public void Add()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			Fill(lhm);
			lhm.Add("55555", new Player("55555", "Monde Zondeki"));

			Assert.That(lhm.Count, Is.EqualTo(7));
		}

		[Test]
		public void LastKeyLastValue()
		{
			var lhm = new LinkHashMap<string, Player>();
			Fill(lhm);
			Assert.That(lhm.LastKey, Is.EqualTo(players[players.Length - 1].Id));
			Assert.That(lhm.LastValue, Is.EqualTo(players[players.Length - 1]));

			// override
			var antWithSameId = new Player("12341", "Another");
			lhm[antWithSameId.Id] = antWithSameId;
			Assert.That(lhm.LastKey, Is.EqualTo(antWithSameId.Id));
			Assert.That(lhm.LastValue, Is.EqualTo(antWithSameId));
		}

		[Test]
		public void FirstKeyFirstValue()
		{
			var lhm = new LinkHashMap<string, Player>();
			Fill(lhm);
			Assert.That(lhm.FirstKey, Is.EqualTo(players[0].Id));
			Assert.That(lhm.FirstValue, Is.EqualTo(players[0]));

			// override First
			var antWithSameId = new Player("12341", "Another");
			lhm[antWithSameId.Id] = antWithSameId;
			Assert.That(lhm.FirstKey, Is.EqualTo(players[1].Id));
			Assert.That(lhm.FirstValue, Is.EqualTo(players[1]));
		}

		[Test]
		public void Clear()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			var p = new Player("78945", "Someone");
			lhm[p.Id] = p;

			lhm.Clear();
			Assert.That(lhm, Is.Empty);

			foreach (KeyValuePair<string, Player> pair in lhm)
				Assert.Fail("Should not be any entries but found Key = " + pair.Key + " and Value = " + pair.Value);
		}

		[Test]
		public void Contains()
		{
			var lhm = new LinkHashMap<string, Player>();
			Fill(lhm);

			Assert.That(lhm.Contains("12341"), Is.True);
			Assert.That(lhm.Contains("55555"), Is.False);
		}

		[Test]
		public void GetEnumerator()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			Fill(lhm);
			int index = 0;
			foreach (KeyValuePair<string, Player> pair in lhm)
			{
				Assert.That(pair.Key, Is.EqualTo(players[index].Id));
				Assert.That(pair.Value, Is.EqualTo(players[index]));
				index++;
			}

			Assert.That(index, Is.EqualTo(6));
		}

		[Test]
		public void GetEnumeratorEmpty()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			Assert.That(lhm, Is.Empty);

			int entries = 0;
			foreach (KeyValuePair<string, Player> pair in lhm)
				entries++;
			foreach (string s in lhm.Keys)
				entries++;
			foreach (Player value in lhm.Values)
				entries++;

			Assert.That(entries, Is.Zero, "should not have any entries in the enumerators");
		}

		[Test]
		public void GetEnumeratorModifyExceptionFromAdd()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			lhm["123"] = new Player("123", "yyyyyyy");
			Assert.That(
				() =>
				{
					foreach (var pair in lhm)
					{
						lhm["78945"] = new Player("78945", "Someone");
					}
				},
				Throws.InvalidOperationException);
		}

		[Test]
		public void GetEnumeratorModifyExceptionFromRemove()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			lhm["123"] = new Player("123", "yyyyyyy");
			Assert.That(
				() =>
				{
					foreach (var pair in lhm)
					{
						lhm.Remove(pair.Key);
					}
				},
				Throws.InvalidOperationException);
		}

		[Test]
		public void GetEnumeratorModifyExceptionFromUpdate()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			lhm["123"] = new Player("123", "yyyyyyy");
			Assert.That(
				() =>
				{
					foreach (var pair in lhm)
					{
						lhm["123"] = new Player("123", "aaaaaaa");
					}
				},
				Throws.InvalidOperationException);
		}

		[Test]
		public void EnumeratorInstanceShouldNotBeNonGenericIEnumerator()
		{
			var lhm = new LinkHashMap<string, Player>();
			var enumerator = lhm.GetEnumerator();
			var enumeratorType = enumerator.GetVariableType();

			Assert.That(enumeratorType, Is.Not.EqualTo(typeof(IEnumerator)));
		}

		[Test]
		public void EnumeratorInstanceShouldBeStruct()
		{
			var lhm = new LinkHashMap<string, Player>();
			var enumerator = lhm.GetEnumerator();
			var enumeratorType = enumerator.GetVariableType();

			Assert.That(enumeratorType.IsStruct, Is.True);
		}

		[Test]
		public void KeysEnumeratorInstanceShouldBeStruct()
		{
			var lhm = new LinkHashMap<string, Player>();
			var enumerator = lhm.Keys.GetEnumerator();
			var enumeratorType = enumerator.GetVariableType();

			Assert.That(enumeratorType.IsStruct, Is.True);
		}

		[Test]
		public void ValuesEnumeratorInstanceShouldBeStruct()
		{
			var lhm = new LinkHashMap<string, Player>();
			var enumerator = lhm.Values.GetEnumerator();
			var enumeratorType = enumerator.GetVariableType();

			Assert.That(enumeratorType.IsStruct, Is.True);
		}

		[Test]
		public void Remove()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			Fill(lhm);

			// remove an item that exists
			bool removed = lhm.Remove("23411");
			Assert.That(removed, Is.True);
			Assert.That(lhm.Count, Is.EqualTo(5));

			// try to remove an item that does not exist
			removed = lhm.Remove("65432");
			Assert.That(removed, Is.False);
			Assert.That(lhm.Count, Is.EqualTo(5));
		}

		[Test]
		public void ContainsValue()
		{
			var lhm = new LinkHashMap<string, Player>();
			Fill(lhm);
			Assert.That(lhm.ContainsValue(new Player("55221", "Jonty Rhodes")), Is.True);
			Assert.That(lhm.ContainsValue(new Player("55221", "SameKeyDiffName")), Is.False);
		}

		[Test]
		public void CopyTo()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			Fill(lhm);
			var destArray = new KeyValuePair<string, Player>[lhm.Count + 1];
			destArray[0] = new KeyValuePair<string, Player>("999", new Player("999", "The number nine"));
			lhm.CopyTo(destArray, 1);

			for (var i = 1; i < destArray.Length; i++)
			{
				Assert.That(destArray[i].Key, Is.EqualTo(players[i - 1].Id));
				Assert.That(destArray[i].Value, Is.EqualTo(players[i - 1]));
			}
		}

		[Test]
		public void Keys()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			Fill(lhm);
			var index = 0;
			foreach (string s in lhm.Keys)
			{
				Assert.That(s, Is.EqualTo(players[index].Id));
				index++;
			}
		}

		[Test]
		public void Values()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			Fill(lhm);
			var index = 0;
			foreach (Player p in lhm.Values)
			{
				Assert.That(p, Is.EqualTo(players[index]));
				index++;
			}
		}

		[Test]
		public void Serialization()
		{
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			Fill(lhm);

			var stream = new MemoryStream();
			var f = new BinaryFormatter
			{
#if !NETFX
				SurrogateSelector = new SerializationHelper.SurrogateSelector()	
#endif
			};
			f.Serialize(stream, lhm);
			stream.Position = 0;

			var dlhm = (LinkHashMap<string, Player>)f.Deserialize(stream);
			stream.Close();

			Assert.That(dlhm.Count, Is.EqualTo(6));
			var index = 0;
			foreach (var pair in dlhm)
			{
				Assert.That(pair.Key, Is.EqualTo(players[index].Id));
				Assert.That(pair.Value, Is.EqualTo(players[index]));
				index++;
			}

			Assert.That(index, Is.EqualTo(6));
		}

		[Test, Explicit]
		public void ShowDiff()
		{
			IDictionary<string, Player> dict = new Dictionary<string, Player>();
			IDictionary<string, Player> lhm = new LinkHashMap<string, Player>();
			Fill(dict);
			Fill(lhm);
			// Override the first element
			var o = new Player("12341", "Override");
			dict[o.Id] = o;
			lhm[o.Id] = o;
			Console.Out.WriteLine("Dictionary order:");
			foreach (KeyValuePair<string, Player> pair in dict)
			{
				Console.Out.WriteLine("Key->{0}", pair.Key);
			}
			Console.Out.WriteLine("LinkHashMap order:");
			foreach (KeyValuePair<string, Player> pair in lhm)
			{
				Console.Out.WriteLine("Key->{0}", pair.Key);
			}
		}

		[Test, Explicit]
		public void Performance()
		{
			// Take care with this test because the result is not the same every times

			int numOfRuns = 4;

			int numOfEntries = Int16.MaxValue;

			var dictPopulateTicks = new long[numOfRuns];
			var dictItemTicks = new long[numOfRuns];

			var linkPopulateTicks = new long[numOfRuns];
			var linkItemTicks = new long[numOfRuns];

			for (var runIndex = 0; runIndex < numOfRuns; runIndex++)
			{
				string key;
				object value;
				IDictionary<string, object> dictionary = new Dictionary<string, object>();
				IDictionary<string, object> linked = new LinkHashMap<string, object>();

				long dictStart = DateTime.Now.Ticks;

				for (int i = 0; i < numOfEntries; i++)
				{
					dictionary.Add("test" + i, new object());
				}

				dictPopulateTicks[runIndex] = DateTime.Now.Ticks - dictStart;

				dictStart = DateTime.Now.Ticks;
				for (int i = 0; i < numOfEntries; i++)
				{
					key = "test" + i;
					value = dictionary[key];
				}
				dictItemTicks[runIndex] = DateTime.Now.Ticks - dictStart;

				dictionary.Clear();

				long linkStart = DateTime.Now.Ticks;

				for (int i = 0; i < numOfEntries; i++)
				{
					linked.Add("test" + i, new object());
				}

				linkPopulateTicks[runIndex] = DateTime.Now.Ticks - linkStart;

				linkStart = DateTime.Now.Ticks;
				for (int i = 0; i < numOfEntries; i++)
				{
					key = "test" + i;
					value = linked[key];
				}

				linkItemTicks[runIndex] = DateTime.Now.Ticks - linkStart;

				linked.Clear();
			}

			for (var runIndex = 0; runIndex < numOfRuns; runIndex++)
			{
				decimal linkPopulateOverhead = (linkPopulateTicks[runIndex] / (decimal)dictPopulateTicks[runIndex]);
				decimal linkItemOverhead = (linkItemTicks[runIndex] / (decimal)dictItemTicks[runIndex]);

				string message = string.Format("LinkHashMap vs Dictionary (Run-{0}) :",runIndex+1);
				message += "\n POPULATE:";
				message += "\n\t linked took " + linkPopulateTicks[runIndex] + " ticks.";
				message += "\n\t dictionary took " + dictPopulateTicks[runIndex] + " ticks.";
				message += "\n\t for an overhead of " + linkPopulateOverhead;
				message += "\n RETRIVE:";
				message += "\n\t linked took " + linkItemTicks[runIndex] + " ticks.";
				message += "\n\t dictionary took " + dictItemTicks[runIndex] + " ticks.";
				message += "\n\t for an overhead of " + linkItemOverhead;

				Console.Out.WriteLine(message);
				Console.Out.WriteLine();
			}
		}
	}

	internal static class LinkHashMapFixtureHelpers
	{
		public static System.Type GetVariableType<T>(this T _) => typeof(T);

		public static bool IsStruct(this System.Type type)
		{
			return !type.IsClass && !type.IsInterface;
		}
	}

	[Serializable]
	public class Player
	{
		private string id;
		private string name;

		public Player(string id, string name)
		{
			if (string.IsNullOrEmpty(id))
				throw new ArgumentNullException(nameof(id));

			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			this.id = id;
			this.name = name;
		}

		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public override int GetHashCode()
		{
			return id.GetHashCode() ^ name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Player that = obj as Player;
			if (that == null) return false;
			return id.Equals(that.id) && name.Equals(that.name);
		}

		public override string ToString()
		{
			return $"<{id}>{name}";
		}
	}
}
