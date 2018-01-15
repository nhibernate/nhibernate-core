using System;
using NHibernate.DomainModel;
using NHibernate.Linq.Visitors;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1526
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void ShouldCreateDifferentKeyForAnonymousTypesFromDifferentAssemblies()
		{
			var exp1 = AnonymousTypeExpressionProviderFromNHibernateTestAssembly.GetExpression();
			var exp2 = AnonymousTypeExpressionProviderFromNHibernateDomainModelAssembly.GetExpression();

			var key1 = ExpressionKeyVisitor.Visit(exp1, null);
			var key2 = ExpressionKeyVisitor.Visit(exp2, null);

			Assert.That(key1, Is.Not.EqualTo(key2));
		}
	}
}
