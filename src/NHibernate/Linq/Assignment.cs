using System.Linq.Expressions;

namespace NHibernate.Linq
{
	/// <summary>
	/// Specifies one assignment in an update or insert query
	/// </summary>
	public class Assignment
	{
		/// <summary>
		/// The assigned property.
		/// </summary>
		public string PropertyPath { get; set; }
		/// <summary>
		/// The value to assign.
		/// </summary>
		public Expression Expression { get; set; }

		public Assignment(string propertyPath, Expression expression)
		{
			PropertyPath = propertyPath;
			Expression = expression;
		}
	}
}