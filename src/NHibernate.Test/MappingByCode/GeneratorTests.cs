using System.Linq;
using NHibernate.Id;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode
{
	public class A { public int Id { get; set; } }

	public class B { public int Id { get; set; } }

	public class C { public int Id { get; set; } }

	public class D { public int Id { get; set; } }

	public class E { public int Id { get; set; } }

	public class F { public int Id { get; set; } }
	
	public class G { public int Id { get; set; } }
	
	public class H { public int Id { get; set; } }

	public class I { public int Id { get; set; } }

	public class GeneratorTests
	{
		[Test]
		public void TestUUIDHexWithParameters()
		{
			//NH-3759
			var mapper = new ModelMapper();

			mapper.Class<A>(e => { e.Id(c => c.Id, c => c.Generator(Generators.UUIDHex("X", "."))); });
			mapper.Class<B>(e => { e.Id(c => c.Id, c => c.Generator(Generators.UUIDHex("X"))); });

			var hbmMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var aGenerator = hbmMapping.RootClasses.Single(x => x.Name == typeof(A).Name).Id.generator;
			Assert.AreEqual("X", aGenerator.param[0].Text[0]);
			Assert.AreEqual(".", aGenerator.param[1].Text[0]);

			var bGenerator = hbmMapping.RootClasses.Single(x => x.Name == typeof(B).Name).Id.generator;
			Assert.AreEqual("X", bGenerator.param[0].Text[0]);
		}

		[Test]
		public void TestGenerators()
		{
			var mapper = new ModelMapper();

			mapper.Class<A>(e => { e.Id(c => c.Id, c => c.Generator(Generators.Counter)); });
			mapper.Class<B>(e => { e.Id(c => c.Id, c => c.Generator(Generators.UUIDHex())); });
			mapper.Class<C>(e => { e.Id(c => c.Id, c => c.Generator(Generators.UUIDString)); });
			mapper.Class<D>(e => { e.Id(c => c.Id, c => c.Generator(Generators.Increment)); });
			mapper.Class<E>(e => { e.Id(c => c.Id, c => c.Generator(Generators.Select)); });
			mapper.Class<F>(e => { e.Id(c => c.Id, c => c.Generator(Generators.SequenceHiLo)); });
			mapper.Class<G>(e => { e.Id(c => c.Id, c => c.Generator(Generators.SequenceIdentity)); });
			mapper.Class<H>(e => { e.Id(c => c.Id, c => c.Generator(Generators.Table)); });
			mapper.Class<I>(e => { e.Id(c => c.Id, c => c.Generator(Generators.TriggerIdentity)); });

			var hbmMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
			Assert.AreEqual(hbmMapping.RootClasses.Single(x => x.Name == typeof(A).Name).Id.generator.@class, Generators.Counter.Class);
			Assert.AreEqual(hbmMapping.RootClasses.Single(x => x.Name == typeof(B).Name).Id.generator.@class, Generators.UUIDHex().Class);
			Assert.AreEqual(hbmMapping.RootClasses.Single(x => x.Name == typeof(C).Name).Id.generator.@class, Generators.UUIDString.Class);
			Assert.AreEqual(hbmMapping.RootClasses.Single(x => x.Name == typeof(D).Name).Id.generator.@class, Generators.Increment.Class);
			Assert.AreEqual(hbmMapping.RootClasses.Single(x => x.Name == typeof(E).Name).Id.generator.@class, Generators.Select.Class);
			Assert.AreEqual(hbmMapping.RootClasses.Single(x => x.Name == typeof(F).Name).Id.generator.@class, Generators.SequenceHiLo.Class);
			Assert.AreEqual(hbmMapping.RootClasses.Single(x => x.Name == typeof(G).Name).Id.generator.@class, Generators.SequenceIdentity.Class);
			Assert.AreEqual(hbmMapping.RootClasses.Single(x => x.Name == typeof(H).Name).Id.generator.@class, Generators.Table.Class);
			Assert.AreEqual(hbmMapping.RootClasses.Single(x => x.Name == typeof(I).Name).Id.generator.@class, Generators.TriggerIdentity.Class);
		}
	}
}