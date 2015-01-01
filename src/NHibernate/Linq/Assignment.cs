using System.Linq.Expressions;

namespace NHibernate.Linq
{
	/// <summary>
	/// Specifies one assignment in an update or insert query
	/// </summary>
	public class Assignment
	{
		public string PropertyPath { get; set; }
		public Expression Expression { get; set; }

		public Assignment(string propertyPath,Expression expression)
		{
			PropertyPath = propertyPath;
			Expression = expression;
		}
	}
}