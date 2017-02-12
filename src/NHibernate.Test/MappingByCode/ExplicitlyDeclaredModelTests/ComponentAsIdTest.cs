using System;
using System.Linq;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public abstract class Parent
	{
		public virtual ParentId Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Address Address { get; set; }
	}

	public class ParentId
	{
		public string Key1 { get; set; }
		public string Key2 { get; set; }

		public override bool Equals(object obj)
		{
			var pk = obj as ParentId;

			if (obj == null)
				return false;

			return (Key1 == pk.Key1 && Key2 == pk.Key2);
		}

		public override int GetHashCode()
		{
			return (Key1 + "|" + Key2).GetHashCode();
		}
	}

	public class Child1 : Parent
	{
	}

	public class Child2 : Parent
	{
	}

	class Child1Map : ClassMapping<Child1>
	{
		public Child1Map()
		{
			Table("Child1");

			ComponentAsId<ParentId>(x => x.Id, pk =>
			{
				pk.Property(x => x.Key1, x => x.Column("key1"));
				pk.Property(x => x.Key2, x =>
				{
					x.Column("key2");
				});
			});

			Property(x => x.Name);

			Component<Address>(x => x.Address, map =>
			{
				map.Property(y => y.City, mc => mc.Column("city1"));
			});
		}
	}

	class Child2Map : ClassMapping<Child2>
	{
		public Child2Map()
		{
			Table("Child2");

			ComponentAsId<ParentId>(x => x.Id, pk =>
			{
				pk.Property(x => x.Key1, x => x.Column("key1"));
				pk.Property(x => x.Key2, x =>
				{
					x.Column("key2__");
				});
			});

			Property(x => x.Name);

			Component<Address>(x => x.Address, map =>
			{
				map.Property(y => y.City, mc => mc.Column("city2"));
			});
		}
	}

	public class Address
	{
		public virtual string City { get; set; }
	}

	[TestFixture]
	public class ComponentAsIdTest
	{
		[Test]
		public void CanHaveSameComponentAsIdMultipleTimesWithDifferentColumnNamesForSameProperty()
		{
			//NH-3650
			var model = new ModelMapper();
			model.AddMapping<Child1Map>();
			model.AddMapping<Child2Map>();

			var mappings = model.CompileMappingForEach(new[] { typeof(Child1), typeof(Child2) });

			var child1Mapping = mappings.ElementAt(0);
			Assert.AreEqual("city1", child1Mapping.RootClasses[0].Properties.OfType<HbmComponent>().First().Properties.OfType<HbmProperty>().Single().column);
			//next one fails
			Assert.AreEqual("key2", child1Mapping.RootClasses[0].CompositeId.Items.OfType<HbmKeyProperty>().Last().column1);

			var child2Mapping = mappings.ElementAt(1);
			Assert.AreEqual("city2", child2Mapping.RootClasses[0].Properties.OfType<HbmComponent>().First().Properties.OfType<HbmProperty>().Single().column);
			Assert.AreEqual("key2__", child2Mapping.RootClasses[0].CompositeId.Items.OfType<HbmKeyProperty>().Last().column1);
		}
	}
}
