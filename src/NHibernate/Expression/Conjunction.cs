using System;

namespace NHibernate.Expression
{
	/// <summary>
	/// An Expression that represents a "between" constraint.
	/// </summary>
	public class Conjunction : Junction
	{
		protected override string Op {
			get { return " and "; }
		}
	}
}
