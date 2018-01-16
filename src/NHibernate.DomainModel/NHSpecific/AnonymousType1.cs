using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

// Simulates compiler-generated, non-namespaced anonymous type with one property
internal class AnonymousType1<TProp1>
{
	public TProp1 Prop1 { get; set; }
}

namespace NHibernate.DomainModel.NHSpecific
{
	// Produces an Expression that has the above AnonymousType1 embedded in it
	public static class AnonymousTypeQueryExpressionProviderFromNHibernateDomainModelAssembly
	{
		public static System.Type GetAnonymousType()
		{
			return typeof(AnonymousType1<string>);
		}

		public static Expression GetQueryExpression()
		{
			return TypedSimpleQueryExpressionProvider.GetQueryExpression<AnonymousType1<string>>();
		}
	}

	public static class TypedSimpleQueryExpressionProvider
	{
		public static Expression GetQueryExpression<T>() where T : new ()
		{
			Expression<Func<IList<T>>> exp = () =>
				Enumerable.Empty<object>().Select(o => new T()).ToList();

			return exp;
		}
	}
}
