using System;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class Formula
	{
		private static int formulaUniqueInteger = 0;

		private string formula;

		private int uniqueInteger;

		/// <summary></summary>
		public Formula()
		{
			uniqueInteger = formulaUniqueInteger++;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public String GetTemplate( Dialect.Dialect dialect )
		{
			return Template.RenderWhereStringTemplate( formula, dialect );
		}

		/// <summary></summary>
		public String Alias
		{
			get { return "f" + uniqueInteger.ToString() + StringHelper.Underscore; }
		}

		/// <summary></summary>
		public string FormulaString
		{
			get { return formula; }
			set { this.formula = value; }
		}
	}
}