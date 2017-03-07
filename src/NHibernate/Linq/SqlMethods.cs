﻿using System;

namespace NHibernate.Linq
{
	public static class SqlMethods
	{
		/// <summary>
		/// Use this method in a Linq2NHibernate expression to generate
		/// an SQL LIKE expression. (If you want to avoid depending on the NHibernate.Linq namespace,
		/// you can define your own replica of this method. Any 2-argument method named Like in a class named SqlMethods
		/// will be translated.) This method can only be used in Linq2NHibernate expressions, and will throw
		/// if called directly.
		/// </summary>
		public static bool Like(this string matchExpression, string sqlLikePattern)
		{
			throw new NotSupportedException(
				"The NHibernate.Linq.SqlMethods.Like(string, string) method can only be used in Linq2NHibernate expressions.");
		}

		/// <summary>
		/// Use this method in a Linq2NHibernate expression to generate
		/// an SQL LIKE expression with an escape character defined. (If you want to avoid depending on the NHibernate.Linq namespace,
		/// you can define your own replica of this method. Any 3-argument method named Like in a class named SqlMethods
		/// will be translated.) This method can only be used in Linq2NHibernate expressions, and will throw
		/// if called directly.
		/// </summary>
		public static bool Like(this string matchExpression, string sqlLikePattern, char escapeCharacter)
		{
			throw new NotSupportedException(
				"The NHibernate.Linq.SqlMethods.Like(string, string, char) method can only be used in Linq2NHibernate expressions.");
		}
	}
}
