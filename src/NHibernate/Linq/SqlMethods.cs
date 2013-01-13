using System;

namespace NHibernate.Linq
{
	public static class SqlMethods
	{
		/// <summary>
		/// Use the SqlMethods.Like() method in a Linq2NHibernate expression to generate
		/// an SQL LIKE expression. (Any 2-argument method named Like in a class named SqlMethods
		/// will be translated.)
		/// </summary>
		public static bool Like(string matchExpression, string pattern)
		{
			throw new NotSupportedException(
				"The NHibernate.Linq.SqlMethods.Like(string, string) method can only be used in Linq2NHibernate expressions.");
		}
	}
}
