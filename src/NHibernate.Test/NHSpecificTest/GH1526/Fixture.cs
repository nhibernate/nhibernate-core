using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.DomainModel.NHSpecific;
using NHibernate.Linq.Visitors;
using NHibernate.Param;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1526
{
	[TestFixture]
	public class Fixture
	{
		private readonly AnonymousTypeQueryExpressionProviderFromNHibernateTestAssembly _providerFromNHTest
			= new AnonymousTypeQueryExpressionProviderFromNHibernateTestAssembly();

		private readonly AnonymousTypeQueryExpressionProviderFromNHibernateDomainModelAssembly _providerFromNHDoMo
			= new AnonymousTypeQueryExpressionProviderFromNHibernateDomainModelAssembly();

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			// all the tests in this fixture run under condition, that
			// the types used from the two providers are different,
			// but have exactly the same System.Type.FullName when inspected

			var type1 = _providerFromNHTest.GetAnonymousType();
			var type2 = _providerFromNHDoMo.GetAnonymousType();
			
			Assert.That(type1.FullName, Is.EqualTo(type2.FullName),
				"The two tested types must have the same FullName for demonstrating the bug.");

			Assert.That(type1, Is.Not.EqualTo(type2),
				"The two tested types must not be the same for demonstrating the bug.");
		}

		[Test]
		public void ShouldCreateDifferentKeys_MethodCallExpression()
		{
			var exp1 = _providerFromNHTest.GetExpressionOfMethodCall();
			var exp2 = _providerFromNHDoMo.GetExpressionOfMethodCall();

			var key1 = GetCacheKey(exp1);
			var key2 = GetCacheKey(exp2);

			Assert.That(key1, Is.Not.EqualTo(key2));
		}

		[Test]
		public void ShouldCreateDifferentKeys_NewExpression()
		{
			var exp1 = _providerFromNHTest.GetExpressionOfNew();
			var exp2 = _providerFromNHDoMo.GetExpressionOfNew();

			var key1 = GetCacheKey(exp1);
			var key2 = GetCacheKey(exp2);

			Assert.That(key1, Is.Not.EqualTo(key2));
		}

		private static string GetCacheKey(Expression exp)
		{
			return ExpressionKeyVisitor.Visit(exp, new Dictionary<ConstantExpression, NamedParameter>());
		}
	}
}
