using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	[TestFixture]
	public class ArrayCollectionTests
	{
		private class Entity
		{
			// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			private ICollection<string> others;
			private string[] emails;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
			public string[] NickNames { get; set; }
			public byte[] Photo { get; set; }

			public ICollection<string> Emails
			{
				get { return emails; }
			}

			public ICollection<string> Others
			{
				get { return others; }
			}
			public IList<string> NoArray
			{
				get { return null; }
			}
			public string Simple { get; set; }
		}

		[Test]
		public void MatchWithArrayProperty()
		{
			var mi = typeof(Entity).GetProperty("NickNames");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsArray(mi), Is.True);
		}

		[Test]
		public void MatchWithArrayField()
		{
			var mi = typeof(Entity).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsArray(mi), Is.True);
		}

		[Test]
		public void MatchWithCollectionPropertyAndArrayField()
		{
			var mi = typeof(Entity).GetProperty("Emails");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsArray(mi), Is.True);
		}

		[Test]
		public void NotMatchWithCollectionField()
		{
			var mi = typeof(Entity).GetField("others", BindingFlags.NonPublic | BindingFlags.Instance);
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsArray(mi), Is.False);
		}

		[Test]
		public void NotMatchWithCollectionProperty()
		{
			var mi = typeof(Entity).GetProperty("Others");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsArray(mi), Is.False);
		}

		[Test]
		public void NotMatchWithNoArrayCollectionProperty()
		{
			var mi = typeof(Entity).GetProperty("NoArray");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsArray(mi), Is.False);
		}

		[Test]
		public void NotMatchWithStringProperty()
		{
			var mi = typeof(Entity).GetProperty("Simple");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsArray(mi), Is.False);
		}

		[Test]
		public void NotMatchWithByteArrayProperty()
		{
			var mi = typeof(Entity).GetProperty("Photo");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsArray(mi), Is.False);
		}
	}
}
