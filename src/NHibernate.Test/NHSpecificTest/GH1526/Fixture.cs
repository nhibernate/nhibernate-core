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
		[Test]
		public void ShouldCreateDifferentKeyForSameNamedAnonymousTypesFromDifferentAssemblies()
		{
			// preconditions of the test
			var type1 = AnonymousTypeQueryExpressionProviderFromNHibernateTestAssembly
				.GetAnonymousType();
			var type2 = AnonymousTypeQueryExpressionProviderFromNHibernateDomainModelAssembly
				.GetAnonymousType();
			
			Assert.That(type1.FullName, Is.EqualTo(type2.FullName),
				"The two tested types must have the same FullName for demonstrating the bug.");

			Assert.That(type1, Is.Not.EqualTo(type2),
				"The two tested types must not be the same for demonstrating the bug.");

			// the test
			var exp1 = AnonymousTypeQueryExpressionProviderFromNHibernateTestAssembly
				.GetQueryExpression();
			var exp2 = AnonymousTypeQueryExpressionProviderFromNHibernateDomainModelAssembly
				.GetQueryExpression();

			var key1 = ExpressionKeyVisitor.Visit(exp1, new Dictionary<ConstantExpression, NamedParameter>());
			var key2 = ExpressionKeyVisitor.Visit(exp2, new Dictionary<ConstantExpression, NamedParameter>());

			Assert.That(key1, Is.Not.EqualTo(key2));
		}
	}
}
