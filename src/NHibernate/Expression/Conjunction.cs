using System;

namespace NHibernate.Expression
{
	/// <summary>
	/// Summary description for Conjunction.
	/// </summary>
	public class Conjunction : Junction
	{
		protected override string Op {
			get { return " and "; }
		}
	}
}
