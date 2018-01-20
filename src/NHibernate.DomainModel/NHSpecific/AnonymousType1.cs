﻿using System;
using System.Linq;
using System.Linq.Expressions;

// Simulates compiler-generated, non-namespaced anonymous type with one property
internal class AnonymousType1<TProp1>
{
	public TProp1 Prop1 { get; set; }
}

namespace NHibernate.DomainModel.NHSpecific
{
	public class AnonymousTypeQueryExpressionProviderFromNHibernateDomainModelAssembly
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
		
		public Expression GetExpressionOfTypeBinary()
		{
			return _provider.GetExpressionOfTypeBinary();
		}
	}

	public class TypedQueryExpressionProvider<T> where T : new ()
	{
		public System.Type GetSuppliedType()
		{
			return typeof(T);
		}

		public Expression GetExpressionOfMethodCall()
		{
			Expression<Func<object>> exp = () =>
				Enumerable.Empty<object>().Select(o => (T)o).ToList();

			return exp;
		}

		public Expression GetExpressionOfNew()
		{
			// adds .GetHashCode to make sure the .ToList is always of same generic type
			// so that the only variable part is the 'new T()'
			Expression<Func<object>> exp = () =>
				Enumerable.Empty<object>().Select(o => new T().GetHashCode()).ToList();

			return exp;
		}

		public Expression GetExpressionOfTypeBinary()
		{
			Expression<Func<object>> exp = () =>
				Enumerable.Empty<object>().Select(o => o is T).ToList();

			return exp;
		}
	}
}
