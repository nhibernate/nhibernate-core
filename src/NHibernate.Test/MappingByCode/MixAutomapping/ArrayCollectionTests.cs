using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class ArrayCollectionTests
	{
		private class Entity
		{
			private ICollection<string> others;
			private string[] emails;
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

			inspector.IsArray(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithArrayField()
		{
			var mi = typeof(Entity).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsArray(mi).Should().Be.True();
		}

		[Test]
		public void MatchWithCollectionPropertyAndArrayField()
		{
			var mi = typeof(Entity).GetProperty("Emails");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsArray(mi).Should().Be.True();
		}

		[Test]
		public void NotMatchWithCollectionField()
		{
			var mi = typeof(Entity).GetField("others", BindingFlags.NonPublic | BindingFlags.Instance);
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsArray(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithCollectionProperty()
		{
			var mi = typeof(Entity).GetProperty("Others");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsArray(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithNoArrayCollectionProperty()
		{
			var mi = typeof(Entity).GetProperty("NoArray");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsArray(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithStringProperty()
		{
			var mi = typeof(Entity).GetProperty("Simple");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsArray(mi).Should().Be.False();
		}

		[Test]
		public void NotMatchWithByteArrayProperty()
		{
			var mi = typeof(Entity).GetProperty("Photo");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsArray(mi).Should().Be.False();
		}
	}
}