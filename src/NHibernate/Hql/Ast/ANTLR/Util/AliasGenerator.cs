using System;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	/// <summary>
	/// Generates class/table/column aliases during semantic analysis and SQL rendering.
	/// Its essential purpose is to keep an internal counter to ensure that the
	/// generated aliases are unique.
	/// </summary>
	public class AliasGenerator
	{
		private int next;

		private int nextCount()
		{
			return next++;
		}

		public string CreateName(string name)
		{
			return StringHelper.GenerateAlias(name, nextCount());
		}
	}
}
