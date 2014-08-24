using System;
using NHibernate.Dialect.Function;

namespace NHibernate.Mapping
{
	public interface ISelectable
	{
		string GetAlias(Dialect.Dialect dialect);
		string GetAlias(Dialect.Dialect dialect, Table table);
		bool IsFormula { get; }
		string GetTemplate(Dialect.Dialect dialect, SQLFunctionRegistry functionRegistry);
		string GetText(Dialect.Dialect dialect);
		string Text { get; }
	}
}