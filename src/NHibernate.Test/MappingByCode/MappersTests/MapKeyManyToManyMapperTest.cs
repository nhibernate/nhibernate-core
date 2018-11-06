using System.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	[TestFixture]
	public class MapKeyManyToManyMapperTest
	{
		[Test]
		public void CanSetColumnsAndFormulas()
		{
			var mapping = new HbmMapKeyManyToMany();
			IMapKeyManyToManyMapper mapper = new MapKeyManyToManyMapper(mapping);
			mapper.ColumnsAndFormulas(x => x.Name("pizza"), x => x.Formula("risotto"), x => x.Name("pasta"));

			Assert.That(mapping.Items, Has.Length.EqualTo(3));
			Assert.That(mapping.Items[0], Is.TypeOf<HbmColumn>(), "first");
			Assert.That(mapping.Items[1], Is.TypeOf<HbmFormula>(), "second");
			Assert.That(mapping.Items[2], Is.TypeOf<HbmColumn>(), "third");
			Assert.That(((HbmColumn)mapping.Items[0]).name, Is.EqualTo("pizza"));
			Assert.That(((HbmFormula)mapping.Items[1]).Text, Has.Length.EqualTo(1).And.One.EqualTo("risotto"));
			Assert.That(((HbmColumn)mapping.Items[2]).name, Is.EqualTo("pasta"));
			Assert.That(mapping.column, Is.Null, "column");
			Assert.That(mapping.formula, Is.Null, "formula");
		}

		[Test]
		public void CanSetMultipleFormulas()
		{
			var mapping = new HbmMapKeyManyToMany();
			IMapKeyManyToManyMapper mapper = new MapKeyManyToManyMapper(mapping);
			mapper.Formulas("formula1", "formula2", "formula3");

			Assert.That(mapping.formula, Is.Null);
			Assert.That(mapping.Items, Has.Length.EqualTo(3));
			Assert.That(
				mapping.Items.Cast<HbmFormula>().Select(f => f.Text.Single()),
				Is.EquivalentTo(new[] { "formula1", "formula2", "formula3" }));
		}
	}
}
