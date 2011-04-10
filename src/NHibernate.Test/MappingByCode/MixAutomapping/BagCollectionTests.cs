using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class BagCollectionTests
	{
		// match any IEnumerable
		private class Entity
		{
			private ICollection<string> emails;
			public IEnumerable<string> NickNames { get; set; }
			public byte[] Bytes { get; set; }
			public object Emails
			{
				get { return emails; }
			}

			public string Simple { get; set; }
		}

		[Test]
		public void MatchWithEnumerableProperty()
		{
			var mi = typeof(Entity).GetProperty("NickNames");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsBag(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithEnumerableField()
		{
			var mi = typeof(Entity).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsBag(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithObjectPropertyAndEnumerableField()
		{
			var mi = typeof(Entity).GetProperty("Emails");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsBag(mi).Should().Be.True();
		}

		[Test]
		public void NotMatchWithStringProperty()
		{
			var mi = typeof(Entity).GetProperty("Simple");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsBag(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithByteArrayProperty()
		{
			var mi = typeof(Entity).GetProperty("Bytes");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsBag(mi).Should().Be.False();
		}
	}
}