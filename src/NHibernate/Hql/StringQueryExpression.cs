using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql
{
	public class StringQueryExpression : IQueryExpression
	{
		private readonly string _queryString;

		public StringQueryExpression(string queryString)
		{
			_queryString = queryString;
		}

		public IASTNode Translate(ISessionFactoryImplementor factory, bool filter)
		{
			return new HqlParseEngine(_queryString, filter, factory).Parse();
		}

		public string Key
		{
			get { return _queryString; }
		}

		public System.Type Type { get { return typeof (object); } }
		public IList<NamedParameterDescriptor> ParameterDescriptors { get; private set; }
	}

	internal static class StringQueryExpressionExtensions
	{
		public static StringQueryExpression ToQueryExpression(this string queryString)
		{
			return new StringQueryExpression(queryString);
		}
	}
}