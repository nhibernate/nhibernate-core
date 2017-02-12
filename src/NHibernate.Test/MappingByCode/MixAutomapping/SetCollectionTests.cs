using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class SetCollectionTests
	{
		private class EntityWithSets
		{
			private ICollection<string> others;
			private ISet<string> emails;
			public ISet<string> NickNames { get; set; }

			public ICollection<string> Emails
			{
				get { return emails; }
			}

			public ICollection<string> Others
			{
				get { return others; }
			}
		}

		[Test]
		public void MatchWithSetProperty()
		{
			var mi = typeof(EntityWithSets).GetProperty("NickNames");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsSet(mi), Is.True);
		}

		[Test]
		public void MatchWithSetField()
		{
			var mi = typeof(EntityWithSets).GetField("emails", BindingFlags.NonPublic | BindingFlags.Instance);
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsSet(mi), Is.True);
		}

		[Test]
		public void MatchWithCollectionPropertyAndSetField()
		{
			var mi = typeof(EntityWithSets).GetProperty("Emails");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsSet(mi), Is.True);
		}

		[Test]
		public void NotMatchWithCollectionField()
		{
			var mi = typeof(EntityWithSets).GetField("others", BindingFlags.NonPublic | BindingFlags.Instance);
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsSet(mi), Is.False);
		}

		[Test]
		public void NotMatchWithCollectionProperty()
		{
			var mi = typeof(EntityWithSets).GetProperty("Others");
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsSet(mi), Is.False);
		}

		[Test]
		public void WhenExplicitDeclaredThenMatchWithCollectionProperty()
		{
			var mi = typeof(EntityWithSets).GetProperty("Others");
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<EntityWithSets>(map => map.Set(x => x.Others, x => { }, y=> {}));

			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsSet(mi), Is.True);
		}
	}
}