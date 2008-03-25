using System;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A formula is a derived column value.
	/// </summary>
	[Serializable]
	public class Formula : ISelectable
	{
		private static int formulaUniqueInteger = 0;

		private string formula;
		private readonly int uniqueInteger;

		/// <summary></summary>
		public Formula()
		{
			uniqueInteger = formulaUniqueInteger++;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="functionRegistry"></param>
		/// <returns></returns>
		public string GetTemplate(Dialect.Dialect dialect, SQLFunctionRegistry functionRegistry)
		{
			return Template.RenderWhereStringTemplate(formula, dialect, functionRegistry);
		}

		/// <summary></summary>
		public string FormulaString
		{
			get { return formula; }
			set { formula = value; }
		}

		public string GetText(Dialect.Dialect dialect)
		{
			return FormulaString;
		}

		public string Text
		{
			get { return FormulaString; }
		}

		public string GetAlias(Dialect.Dialect dialect)
		{
			return string.Format("formula{0}{1}", uniqueInteger, '_');
		}

		public string GetAlias(Dialect.Dialect dialect, Table table)
		{
			return GetAlias(dialect);
		}

		public bool IsFormula
		{
			get { return true; }
		}

		public override string ToString()
		{
			return GetType().FullName + "( " + formula + " )";
		}
	}
}