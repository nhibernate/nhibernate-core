using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
// Simulates compiler-generated, non-namespaced anonymous type
internal class AnonymousType1<T>
{
	public T Name { get; set; }
}

namespace NHibernate.DomainModel
{
	// Produces an Expression that has the above AnonymousType1 embedded in it
	public static class AnonymousTypeExpressionProviderFromNHibernateDomainModelAssembly
	{
		public static Expression GetExpression()
		{
			Expression<Func<IList<AnonymousType1<string>>>> exp = () =>
				Enumerable.Empty<Custom>()
					.Select(c => new AnonymousType1<string> { Name = c.Name })
					.ToList();

			return exp;
		}
	}
}
