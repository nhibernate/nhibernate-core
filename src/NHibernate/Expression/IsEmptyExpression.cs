using System;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents empty association constraint.
	/// </summary>
	[Serializable]
	public class IsEmptyExpression : AbstractEmptinessExpression
	{
		public IsEmptyExpression(string propertyName)
			: base(propertyName)
		{
		}

		protected override bool ExcludeEmpty
		{
			get { return false; }
		}
	}
}