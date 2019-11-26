using System.Linq;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    /// <summary>
	///     Self-contained setup
	/// </summary>
    [TestFixture]
	public class LinqQueriesReadWrite : TestCase
	{
		private static readonly ProductDefinition productDefinition1 = new ProductDefinition() { Id = 1000, MaterialDefinition = new MaterialDefinition { Id = 1 } };
		private static readonly ProductDefinition productDefinition2 = new ProductDefinition() { Id = 1001, MaterialDefinition = new MaterialDefinition { Id = 2 } };
		private static readonly Material material1 = new Material {Id = 1, MaterialDefinition = productDefinition1.MaterialDefinition, ProductDefinition = productDefinition1};
		private static readonly Material material2 = new Material {Id = 2, MaterialDefinition = productDefinition2.MaterialDefinition, ProductDefinition = productDefinition2};

		protected override string[] Mappings
		{
			get { return new[] {"ProductMaterial.hbm.xml"}; }
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession(true))
			{
				session.Save(productDefinition1);
				session.Save(productDefinition2);
				session.Save(material1);
				session.Save(material2);

				session.Transaction.Commit();
			}
			base.OnSetUp();
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession(true))
			{
				session.Delete(productDefinition1);
				session.Delete(productDefinition2);
				session.Delete(material1);
				session.Delete(material2);

				session.Transaction.Commit();
			}

			base.OnTearDown();
		}

		[Test(Description = "#2244")]
		public void LinqExpressionOnParameterEvaluated()
		{
			using (var session = OpenSession())
			{
				var selectedProducts = new[] { productDefinition1 };

				var query = session.Query<Material>()
					.Where(x => selectedProducts.Contains(x.ProductDefinition) && selectedProducts.Select(y => y.MaterialDefinition).Contains(x.MaterialDefinition));

				//var sessionImpl = session.GetSessionImplementation();
				//var factoryImplementor = sessionImpl.Factory;

				//var nhLinqExpression = new NhLinqExpression(query.Expression, factoryImplementor);
				//var translatorFactory = new ASTQueryTranslatorFactory();
				//var translator = translatorFactory.CreateQueryTranslators(nhLinqExpression, null, false, sessionImpl.EnabledFilters, factoryImplementor).First();

				var result = query.ToList();

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(material1, result.Single());
			}
		}
	}
}
