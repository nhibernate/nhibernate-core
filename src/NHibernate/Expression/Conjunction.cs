using System;

namespace NHibernate.Expression
{
	/// <summary>
	/// An Expression that Junctions together multiple Expressions with an <c>and</c>
	/// </summary>
	public class Conjunction : Junction
	{
		protected override string Op 
		{
			get { return " and "; }
		}
	}
}
