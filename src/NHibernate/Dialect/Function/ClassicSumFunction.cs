using System;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Classic SUM sqlfunction that return types as it was done in Hibernate 3.1
	/// </summary>
	public class ClassicSumFunction: StandardSQLFunction
	{
		public ClassicSumFunction() : base("sum") { }
	}
}
