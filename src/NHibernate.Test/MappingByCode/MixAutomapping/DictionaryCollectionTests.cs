using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class DictionaryCollectionTests
	{
		private class Entity
		{
			private ICollection<string> others = null;
			private IDictionary<string, string> emails = null;
			public IDictionary<string, string> NickNames { get; set; }

			public ICollection<KeyValuePair<string, string>> Emails
			{
				get { return emails; }
			}

			public ICollection<string> Others
			{
				get { return others; }
			}
		}

		[Test]
		public void MatchWithDictionaryProperty()
		{
			var mi = typeof(Entity).GetProperty("NickNames");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsDictionary(mi), Is.True);
		}

		[Test]
		public void MatchWithDictionaryField()
		{
			var mi = typeof(Entity).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsDictionary(mi), Is.True);
		}

		[Test]
		public void MatchWithCollectionPropertyAndDictionaryField()
		{
			var mi = typeof(Entity).GetProperty("Emails");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsDictionary(mi), Is.True);
		}

		[Test]
		public void NotMatchWithCollectionField()
		{
			var mi = typeof(Entity).GetField("others", BindingFlags.NonPublic | BindingFlags.Instance);
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsDictionary(mi), Is.False);
		}

		[Test]
		public void NotMatchWithCollectionProperty()
		{
			var mi = typeof(Entity).GetProperty("Others");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsDictionary(mi), Is.False);
		}
	}
}