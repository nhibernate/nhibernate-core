using System;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion"/> that Junctions together multiple 
	/// <see cref="ICriterion"/>s with an <c>or</c>
	/// </summary>
	[Serializable]
	public class Disjunction : Junction
	{
		/// <summary>
		/// Get the Sql operator to put between multiple <see cref="ICriterion"/>s.
		/// </summary>
		/// <value>The string "<c> or </c>"</value>
		protected override string Op
		{
			get { return " or "; }
		}

		protected override SqlString EmptyExpression
		{
			get { return new SqlString("1=0"); }
		}
	}
}