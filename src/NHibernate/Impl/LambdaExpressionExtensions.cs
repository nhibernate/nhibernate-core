using System;
using System.Linq.Expressions;

namespace NHibernate.Impl
{
#if NET461
	internal static class LambdaExpressionExtensions
	{
		public static Delegate Compile(this LambdaExpression expression, bool preferInterpretation) =>
		  expression.Compile(); //Concurrent Compile() call causes "Garbage Collector" suspend all threads too often
	}
#endif
}
