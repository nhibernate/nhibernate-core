namespace NHibernate.Expression
{
	/// <summary>
	/// An Expression that Junctions together multiple Expressions with an <c>or</c>
	/// </summary>
	public class Disjunction : Junction
	{
		/// <summary></summary>
		protected override string Op
		{
			get { return " or "; }
		}
	}
}