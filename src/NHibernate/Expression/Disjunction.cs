using System;

namespace NHibernate.Expression
{
	/// <summary>
	/// Summary description for Disjunction.
	/// </summary>
	public class Disjunction : Junction
	{
		protected override string Op {
			get { return " or "; }
		}
	}
}
