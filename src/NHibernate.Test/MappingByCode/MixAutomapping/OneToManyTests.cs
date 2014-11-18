using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class OneToManyTests
	{
		private class MyClass
		{
			public string Something { get; set; }
			public IEnumerable<Related> Relateds { get; set; }
			public IEnumerable<Bidirectional> Children { get; set; }
			public IEnumerable<Component> Components { get; set; }
			public IEnumerable<string> Elements { get; set; }
			public IDictionary<string, Related> DicRelateds { get; set; }
			public IDictionary<string, Bidirectional> DicChildren { get; set; }
		}

		private class Related
		{

		}

		private class Bidirectional
		{
			public MyClass MyClass { get; set; }
		}

		private class Component
		{
		}

		private IModelInspector GetConfiguredInspector()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyClass>(x => { });
			mapper.Class<Related>(x => { });
			mapper.Class<Bidirectional>(x => { });
			return autoinspector;
		}

		[Test]
		public void WhenNoCollectionPropertyThenNoMatch()
		{
			var pi = Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(x => x.Something);

			var inspector = GetConfiguredInspector();
			Assert.That(inspector.IsOneToMany(pi), Is.False);
		}

		[Test]
		public void WhenCollectionOfComponentsThenNoMatch()
		{
			var pi = Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(x => x.Components);

			var inspector = GetConfiguredInspector();
			Assert.That(inspector.IsOneToMany(pi), Is.False);
		}

		[Test]
		public void WhenCollectionBidirectionalThenMatch()
		{
			var pi = Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(x => x.Children);

			var inspector = GetConfiguredInspector();
			Assert.That(inspector.IsOneToMany(pi), Is.True);
		}

		[Test]
		public void WhenCollectionOfElementsThenNoMatch()
		{
			var pi = Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(x => x.Elements);

			var inspector = GetConfiguredInspector();
			Assert.That(inspector.IsOneToMany(pi), Is.False);
		}

		[Test]
		public void WhenCollectionUnidirectionalThenMatch()
		{
			var pi = Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(x => x.Relateds);

			var inspector = GetConfiguredInspector();
			Assert.That(inspector.IsOneToMany(pi), Is.True);
		}

		[Test]
		public void WhenDictionaryBidirectionalThenMatch()
		{
			var pi = Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(x => x.DicChildren);

			var inspector = GetConfiguredInspector();
			Assert.That(inspector.IsOneToMany(pi), Is.True);
		}

		[Test]
		public void WhenDictionaryUnidirectionalThenMatch()
		{
			var pi = Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(x => x.DicRelateds);

			var inspector = GetConfiguredInspector();
			Assert.That(inspector.IsOneToMany(pi), Is.True);
		}

		[Test]
		public void WhenCollectionUnidirectionalDeclaredManyToManyThenNoMatch()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyClass>(map => map.Bag(x => x.Relateds, cm => { }, relMap => relMap.ManyToMany()));
			mapper.Class<Related>(x => { });
			mapper.Class<Bidirectional>(x => { });
			var inspector = (IModelInspector) autoinspector;

			var pi = Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(x => x.Relateds);

			Assert.That(inspector.IsOneToMany(pi), Is.False);
		}

		[Test]
		public void WhenDictionaryUnidirectionalDeclaredManyToManyThenNoMatch()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyClass>(map => map.Bag(x => x.DicRelateds, cm => { }, relMap => relMap.ManyToMany()));
			mapper.Class<Related>(x => { });
			mapper.Class<Bidirectional>(x => { });
			var inspector = (IModelInspector)autoinspector;

			var pi = Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(x => x.DicRelateds);

			Assert.That(inspector.IsOneToMany(pi), Is.False);
		}
	}
}