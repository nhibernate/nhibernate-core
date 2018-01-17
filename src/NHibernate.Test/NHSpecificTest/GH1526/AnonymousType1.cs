using System.Linq.Expressions;
using NHibernate.DomainModel.NHSpecific;

// Simulates a compiler-generated, non-namespaced anonymous type with one property
// Exactly the same as the one in NHibernate.DomainModel.NHSpecific
internal class AnonymousType1<TProp1>
{
	public TProp1 Prop1 { get; set; }
}

namespace NHibernate.Test.NHSpecificTest.GH1526
{
	// Produces an Expression that has the above AnonymousType1 embedded in it
	public class AnonymousTypeQueryExpressionProviderFromNHibernateTestAssembly
	{
		private readonly TypedQueryExpressionProvider<AnonymousType1<string>> _provider
			= new TypedQueryExpressionProvider<AnonymousType1<string>>();

		public System.Type GetAnonymousType()
		{
			return _provider.GetSuppliedType();
		}

		public Expression GetExpressionOfMethodCall()
		{
			return _provider.GetExpressionOfMethodCall();
		}

		public Expression GetExpressionOfNew()
		{
			return _provider.GetExpressionOfNew();
		}
	}
}
