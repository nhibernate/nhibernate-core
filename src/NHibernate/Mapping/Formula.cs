using System;
using System.Collections;
using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Mapping 
{
	public class Formula 
	{
		private static int formulaUniqueInteger=0;
		private string formula;
		private int uniqueInteger;
		public Formula() 
		{
			uniqueInteger = formulaUniqueInteger++;
		}
		public String getTemplate(Dialect.Dialect dialect) 
		{
			return Template.RenderWhereStringTemplate(formula, dialect);
		}
		public String getAlias() 
		{
			return "f" + uniqueInteger.ToString() + StringHelper.Underscore;
		}
		public string Formul
		{
			get
			{
				return formula;
			}
			set
			{
				this.formula = value;
			}
		}
	}
}