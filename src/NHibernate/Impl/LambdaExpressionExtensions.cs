using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Impl
{
#if NET461
	internal static class LambdaExpressionExtensions
	{
		//NET4.6.1 does not have this method exposed,
		//however it might be available in runtime
		private static readonly MethodInfo CompileWithPreference = typeof(LambdaExpression)
			.GetMethod("Compile", new[] { typeof(bool) });

		public static Delegate Compile(this LambdaExpression expression, bool preferInterpretation)
		{
			if (CompileWithPreference != null)
			{
				return (Delegate) CompileWithPreference.Invoke(expression, new object[] { preferInterpretation });
			}

			return expression.Compile(); //Concurrent Compile() call causes "Garbage Collector" suspend all threads too often
		}
	}
#endif
}
