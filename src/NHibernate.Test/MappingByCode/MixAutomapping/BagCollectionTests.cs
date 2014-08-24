using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
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

		private class Parent
		{
			public int Id { get; set; }
			public IEnumerable<Child> NickNames { get; set; }
		}
		private class Child
		{
			public int Id { get; set; }
			public Parent AParent { get; set; }
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

		[Test]
		public void WhenSetKeyThroughEventThenUseEvent()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.BeforeMapBag += (insp, prop, map) => map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "Id"));

			var hbmMapping = mapper.CompileMappingFor(new[] {typeof(Parent)});
			var hbmBag = hbmMapping.RootClasses[0].Properties.OfType<HbmBag>().Single();
			hbmBag.Key.Columns.Single().name.Should().Be("ParentId");
		}
	}
}